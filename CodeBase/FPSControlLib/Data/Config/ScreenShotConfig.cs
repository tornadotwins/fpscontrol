using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LitJson;
using UnityEngine;
using FPSControl;
using System.IO;

namespace FPSControl.Data.Config
{
    public class ScreenShotConfig : ConfigBase
    {
        public LayerMask cameraMask;
        
        public float bufferScale;
        public int bufferDepthBits;
        //public RenderTextureFormat bufferFormat = RenderTextureFormat.Default;
        public RenderTextureReadWrite bufferRWMode = RenderTextureReadWrite.Default;

        public Shader replacementShader;
        public string replacementRenderType;

        public GameObject decoratorPrefab;
    }
}
