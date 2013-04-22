using UnityEngine;
using UnityEditor;
using System.Collections;
using System.IO;
using System.Collections.Generic;
using FPSControl;

namespace FPSControlEditor
{
    public enum DragResultState
    {
        None,
        Drag,
        Click
    }

    public class DragStyle
    {
        public Color backgroundColor;
        public Color textColor;
        public Texture bgTexture;
        public int textLimit = 7;
        public DragStyle(Color backgroundColor, Color textColor) {
            this.backgroundColor = backgroundColor;
            this.textColor = textColor;
            this.bgTexture = null;
        }
        public DragStyle(Color backgroundColor, Color textColor, int textLimit)
        {
            this.backgroundColor = backgroundColor;
            this.textColor = textColor;
            this.bgTexture = null;
            this.textLimit = textLimit;
        }
        public DragStyle(Texture bgTexture, Color textColor)
        {
            this.backgroundColor = Color.clear;
            this.textColor = textColor;
            this.bgTexture = bgTexture;
        }
        public DragStyle(Texture bgTexture, Color textColor, int textLimit)
        {
            this.backgroundColor = Color.clear;
            this.textColor = textColor;
            this.bgTexture = bgTexture;
            this.textLimit = textLimit;
        }
    }

    public class Drag
    {
        private static string imageFolder = FPSControlMainEditor.ASSET_PATH + FPSControlMainEditor.GRAPHICS + "Drag/";

        public class Styles
        {
            private static DragStyle _default;
            public static DragStyle Default {
                get {
                    if (_default != null) return _default;
                    Texture bg = (Texture)AssetDatabase.LoadAssetAtPath(imageFolder + "bg_default.png", typeof(Texture));
                    _default = new DragStyle(bg, Color.white);
                    return _default;
                }
            }
            public static DragStyle Hidden = new DragStyle(Color.clear, Color.clear);
        }

        public static bool debugMode = true;

        public static DragResultState DragArea<T>(Rect r) where T : UnityEngine.Object //Here for fast layout
        {
            T empty;
            return DragArea<T>(r, out empty, "", Styles.Default);
        }

        public static DragResultState DragArea<T>(Rect r, string text) where T : UnityEngine.Object //Here for fast layout
        {
            T empty;
            return DragArea<T>(r, out empty, text, Styles.Default);
        }

        public static DragResultState DragArea<T>(Rect r, out T obj) where T : UnityEngine.Object
        {
            return DragArea<T>(r, out obj, "", Styles.Default);
        }

        public static DragResultState DragArea<T>(Rect r, out T obj, DragStyle style) where T : UnityEngine.Object
        {
            return DragArea<T>(r, out obj, "", style);
        }

        public static DragResultState DragArea<T>(Rect r, out T obj, string text) where T : UnityEngine.Object
        {
            return DragArea<T>(r, out obj, text, Styles.Default);
        }

        public static DragResultState DragArea<T>(Rect r, out T obj, string text, DragStyle style) where T : UnityEngine.Object
        {
            T[] objs;
            DragResultState result = DragArea<T>(r, out objs, text, style);
            if (objs == null)
            {
                obj = null;
            }
            else
            {
                obj = objs[0];
            }
            return result;
        }

        public static DragResultState DragArea<T>(Rect r, out T[] objArray) where T : UnityEngine.Object
        {
            return DragArea<T>(r, out objArray, "", Styles.Default);
        }

        public static DragResultState DragArea<T>(Rect r, out T[] objArray, DragStyle style) where T : UnityEngine.Object
        {
            return DragArea<T>(r, out objArray, "", style);
        }

        public static DragResultState DragArea<T>(Rect r, out T[] objArray, string text) where T : UnityEngine.Object
        {
            return DragArea<T>(r, out objArray, text, Styles.Default);
        }

        public static DragResultState DragArea<T>(Rect r, out T[] objArray, string text, DragStyle style) where T : UnityEngine.Object
        {
            objArray = null;

            if (text.Length > style.textLimit) text = text.Substring(0, style.textLimit) + "..."; //Limit text

            Event evt = Event.current;

            GUIStyle gs = new GUIStyle();
            gs.normal.textColor = style.textColor;
            gs.alignment = TextAnchor.MiddleCenter;
            gs.border = new RectOffset(4, 4, 4, 4);
            if (style.bgTexture != null) gs.normal.background = (Texture2D)style.bgTexture;

            Color c = GUI.backgroundColor;
            if (style.bgTexture == null) GUI.backgroundColor = style.backgroundColor;
            if (GUI.Button(r, text, gs))
            {
                //Debug.Log("Clicked");
                return DragResultState.Click;
            }
            GUI.backgroundColor = c;

            List<T> returnList = new List<T>();

            switch (evt.type)
            {
                case EventType.DragUpdated:
                    //Keep this here
                case EventType.DragPerform:
                    if (!r.Contains(evt.mousePosition))
                    {
                        return DragResultState.None;
                    }

                    DragAndDrop.visualMode = DragAndDropVisualMode.Link;

                    if (evt.type == EventType.DragPerform)
                    {
                        DragAndDrop.AcceptDrag();

                        Object[] objects = DragAndDrop.objectReferences;

                        foreach (Object obj in objects)
                        {
                            if (obj.GetType() == typeof(T))
                            {
                                returnList.Add((T)obj);
                            }                            
                        }
                    }
                break;
            }
            if (returnList.Count == 0)
            {
                return DragResultState.None;
            }
            objArray = returnList.ToArray();
            return DragResultState.Drag;
        }

    }
}
