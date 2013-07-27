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

        public static Color ColorByControl(ControlMap.ControlID control)
        {
            switch (control)
            {
                case ControlMap.ControlID.Fire: return new Color(235 / 255F, 29 / 255F, 27 / 255F, 1F);
                case ControlMap.ControlID.Reload: return new Color(240 / 255F, 86 / 255F, 32 / 255F, 1F);
                case ControlMap.ControlID.Scope: return new Color(244 / 255F, 131 / 255F, 35 / 255F, 1F);
                case ControlMap.ControlID.Interact: return new Color(252 / 255F, 183 / 255F, 30 / 255F, 1F);
                case ControlMap.ControlID.Defend: return new Color(254 / 255F, 245 / 255F, 0, 1F);

                case ControlMap.ControlID.Weapon1:
                case ControlMap.ControlID.Weapon2:
                case ControlMap.ControlID.Weapon3:
                case ControlMap.ControlID.Weapon4: return new Color(115 / 255F, 192 / 255F, 60 / 255F, 1F);
                case ControlMap.ControlID.WeaponCycle: return new Color(7 / 255F, 168 / 255F, 65 / 255F, 1F);

                case ControlMap.ControlID.Look: return new Color(30 / 255F, 183 / 255F, 178 / 255F, 1F);
                case ControlMap.ControlID.Move: return new Color(0, 99 / 255F, 177 / 255F, 1F);

                case ControlMap.ControlID.Jump: return new Color(60 / 255F, 54 / 255F, 142 / 255F, 1F);

                case ControlMap.ControlID.Run: return new Color(140 / 255F, 44 / 255F, 134 / 255F, 1F);

                case ControlMap.ControlID.Crouch: return new Color(178 / 255F, 45 / 255F, 110 / 255F, 1F);
            }

            return Color.clear;
        }
    }
}
