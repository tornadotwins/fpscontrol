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
    public interface IControlEditor
    {
        void Open();
        void Close();
        void Apply();
        void Draw();//Rect area);
    }
    
    public abstract class ControlEditor<T>
    {
        protected System.Action _OnClose;
        protected System.Action _OnApply;

        public ControlEditor(System.Action closeHandler, System.Action applyHandler)
        {
            _OnClose = closeHandler;
            _OnApply = applyHandler;
        }

        protected abstract void WaitForKeyInput(string key);
        protected abstract void MapKey(KeyCode key);

        protected bool DetectInput(out KeyCode keycode)
        {
            keycode = KeyCode.None;

            Event evt = Event.current;
            if (evt.isKey)
            {
                //Debug.Log(evt.keyCode + ", " + evt.modifiers);

                keycode = evt.keyCode;

                
                
                return true;
            }
            else if (evt.modifiers == EventModifiers.Shift) { keycode = KeyCode.LeftShift; return true; }
            else if (evt.modifiers == EventModifiers.Alt) { keycode = KeyCode.LeftAlt; return true; }
            else if (evt.modifiers == EventModifiers.Control) { keycode = KeyCode.LeftControl; return true; }

            return false;
        }

        protected Rect position = new Rect(300, 160, 324, 197);//new Rect(168, 140, 324, 197);
        protected T target;
        protected string title;
    }
}
