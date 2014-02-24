using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LitJson;
using UnityEngine;
using FPSControl;
using System.IO;
using FPSControl.Data.Config;

namespace FPSControl.Data
{
    [RequireComponent(typeof(Camera))]
    public class ScreenShotComponent : MonoBehaviour
    {
        public string id;        
        public ScreenShotConfig config;

        Camera _camera;
        GameObject _screenShotGO;
        Camera _screenShotCamera;
        public Camera OffScreenCamera { get { return _screenShotCamera; } }

        System.Action _OnCaptureComplete;
        bool _capture = false;

        RenderTexture _buffer;
        public RenderTexture OffScreenBuffer { get { return _buffer; } }

        ScreenShotDecorator[] decorators = new ScreenShotDecorator[0]{};

        void Awake()
        {
            ScreenShot.__RegisterScreenShotComponent(this);
            _camera = camera;
            if (config.decoratorPrefab)
            {
                GameObject decorator = (GameObject)Instantiate(config.decoratorPrefab);
                decorator.transform.parent = transform;
                decorators = decorator.GetComponents<ScreenShotDecorator>();
            }
        }

        internal void __Capture(System.Action onCaptureComplete)
        {
            _OnCaptureComplete = onCaptureComplete;

            if (!_screenShotGO)
            {
                _screenShotGO = new GameObject("[Screen Shot Camera]");
                _screenShotGO.hideFlags = HideFlags.HideAndDontSave;
                _screenShotCamera = _screenShotGO.AddComponent<Camera>();                
            }

            _capture = true;
            enabled = true;
        }

        void OnPreRender()
        {
            if(!_capture)
            {
                enabled = false;
                return;
            }

            _screenShotCamera.CopyFrom(_camera);
            _screenShotCamera.cullingMask = config.cameraMask.value;


            if (_buffer)
            {
                RenderTexture.ReleaseTemporary(_buffer);
                _buffer = null;
            }
            
            // Create our off-screen buffer using the config settings
            _buffer = RenderTexture.GetTemporary(
                Mathf.RoundToInt(Screen.width * config.bufferScale),
                Mathf.RoundToInt(Screen.height * config.bufferScale),
                config.bufferDepthBits, 
                RenderTextureFormat.ARGB32,
                config.bufferRWMode);

            _screenShotCamera.targetTexture = _buffer;


            if (config.replacementShader)
            {
                _screenShotCamera.SetReplacementShader(config.replacementShader, config.replacementRenderType);
            }

            _screenShotCamera.Render();


            // We've rendered our buffer, call our decorators to get extended functionality
            foreach (ScreenShotDecorator deco in decorators)
                deco.OnPreRender(this);
        }

        void OnPostRender()
        {
            // Extended functionality here
            foreach (ScreenShotDecorator deco in decorators)
                deco.OnPostRender(this);

            Texture2D temp = new Texture2D(_buffer.width, _buffer.height, TextureFormat.ARGB32, false);
            temp.ReadPixels(new Rect(0, 0, _buffer.width, _buffer.height), 0, 0, false);
            temp.Apply();

            // We no longer need the buffer, so release it
            RenderTexture.ReleaseTemporary(_buffer);
            _buffer = null;

            ScreenShot.__Save(temp);

            _capture = false;
            enabled = false;

            //if (_OnCaptureComplete != null)
            //    _OnCaptureComplete();
            //else
            //    Debug.LogWarning("_OnCaptureComplete delegate is null");

            //_OnCaptureComplete = null;
        }
    }
}
