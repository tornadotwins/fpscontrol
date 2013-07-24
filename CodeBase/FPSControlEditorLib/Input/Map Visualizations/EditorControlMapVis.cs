using UnityEngine;
using UnityEditor;
using System.Collections;
using System.IO;
using System.Collections.Generic;
using FPSControl;
using FPSControl.Controls;

//---
//
//---
namespace FPSControlEditor.Controls
{
    public interface IEditorControlMapVis
    {
        ControlMap Map { get; }
        void Draw();
    }
    
    public abstract class EditorControlMapVis<T> where T : ControlMap
    {
        protected const string GENERAL_DIR = "Player Control/Platform Maps/";
        
        protected T map;

        public EditorControlMapVis(T focusedMap)
        {
            map = focusedMap;
        }

        protected abstract void LoadAssets();
    }
}
