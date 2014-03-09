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
            //if (config.decoratorPrefab)
            //{
            //    GameObject decorator = (GameObject)Instantiate(config.decoratorPrefab);
            //    decorator.transform.parent = transform;
            //    decorators = decorator.GetComponents<ScreenShotDecorator>();
            //}
        }

        internal void __Capture(System.Action onCaptureComplete)
        {
            _OnCaptureComplete = onCaptureComplete;

            if (!_screenShotGO)
            {
                _screenShotGO = new GameObject("[Screen Shot Camera]");
                _screenShotGO.hideFlags = HideFlags.HideAndDontSave;
                _screenShotCamera = _screenShotGO.AddComponent<Camera>();
                _screenShotGO.SetActive(false);
            }
            _screenShotCamera.depth = -100;

            if (_buffer)
            {
                RenderTexture.ReleaseTemporary(_buffer);
                _buffer = null;
            }

            // Create our off-screen buffer using the config settings
            _buffer = RenderTexture.GetTemporary(
                //Screen.width/4,Screen.height/4,
                Mathf.RoundToInt(Screen.width * config.bufferScale),
                Mathf.RoundToInt(Screen.height * config.bufferScale),
                config.bufferDepthBits,
                ScreenShot.RENDER_TEXTURE_FORMAT,
                config.bufferRWMode);
            _buffer.useMipMap = false;
            _buffer.anisoLevel = 0;
            _buffer.filterMode = FilterMode.Bilinear;

            _screenShotCamera.CopyFrom(camera);

            _screenShotCamera.cullingMask = config.cameraMask.value;
            _screenShotCamera.clearFlags = CameraClearFlags.Skybox;

            _screenShotCamera.targetTexture = _buffer;
            _screenShotCamera.Render();
            RenderTexture.active = _buffer;

            Texture2D temp = new Texture2D(_buffer.width, _buffer.height, ScreenShot.TEXTURE_FORMAT, false);
            temp.ReadPixels(new Rect(0, 0, _buffer.width, _buffer.height), 0, 0, false);
            temp.Apply();

            ScreenShot.__Save(temp);

            if (_OnCaptureComplete != null)
                _OnCaptureComplete();
            else
                Debug.LogWarning("_OnCaptureComplete delegate is null");

            _OnCaptureComplete = null;

        }
        /*
        void OnPreRender()
        {
            if(!_capture)
            {
                enabled = false;
                return;
            }

            if (_buffer)
            {
                RenderTexture.ReleaseTemporary(_buffer);
                _buffer = null;
            }
            
            // Create our off-screen buffer using the config settings
            _buffer = RenderTexture.GetTemporary(
                //Screen.width/4,Screen.height/4,
                Mathf.RoundToInt(Screen.width * config.bufferScale),
                Mathf.RoundToInt(Screen.height * config.bufferScale),
                config.bufferDepthBits, 
                RenderTextureFormat.ARGB32,
                config.bufferRWMode);
            _buffer.useMipMap = false;
            _buffer.anisoLevel = 0;
            _buffer.filterMode = FilterMode.Bilinear;

            //_screenShotCamera.CopyFrom(camera);
            
            _screenShotCamera.targetTexture = _buffer;
            _screenShotCamera.cullingMask = config.cameraMask.value;
            _screenShotCamera.clearFlags = CameraClearFlags.Skybox;

            //if (config.replacementShader != null)
            //{
            //    Debug.Log(string.Format("Replacement shader '{0}' found. Screen shot will be rendered with replacement shader for render type '{1}'", 
            //        config.replacementShader.name, 
            //        string.IsNullOrEmpty(config.replacementRenderType) ? "ALL" : config.replacementRenderType));

            //    _screenShotCamera.RenderWithShader(config.replacementShader, string.IsNullOrEmpty(config.replacementRenderType) ? "" : config.replacementRenderType);
            //        //SetReplacementShader(config.replacementShader, config.replacementRenderType);
            //}
            //else
            //{
                _screenShotCamera.Render();
            //}
            RenderTexture.active = _buffer;

            // We've rendered our buffer, call our decorators to get extended functionality
            //foreach (ScreenShotDecorator deco in decorators)
            //    deco.OnPreRender(this);
        }

        void OnPostRender()
        {
            // Extended functionality here
            foreach (ScreenShotDecorator deco in decorators)
                deco.OnPostRender(this);

            Texture2D temp = new Texture2D(_buffer.width, _buffer.height, TextureFormat.ARGB32, false);
            temp.ReadPixels(new Rect(0, 0, _buffer.width, _buffer.height), 0, 0, false);
            temp.Apply();

            ScreenShot.__Save(temp);

            _capture = false;
            enabled = false;

            if (_OnCaptureComplete != null)
                _OnCaptureComplete();
            else
                Debug.LogWarning("_OnCaptureComplete delegate is null");

            _OnCaptureComplete = null;
        }
    */
    }
}
