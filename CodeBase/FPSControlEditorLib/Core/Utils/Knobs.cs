//#define DEBUG

using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System;

namespace FPSControlEditor
{
    public class Knobs : object
    {
        #region Enums

        public enum Themes { WHITE, BLACK }
        public enum Increments { FIVE = 5, TEN = 10, TWENTY = 20, FIFTY = 50 }

        #endregion // Enums

        #region Events

       // public static Action<Vector2> OnInteractionBegan;
        //public static Action<Vector2> OnInteract;
        public static float interactValue;
        public static int interactID = -1;
        public static Vector2 interactPos;
       // public static Action<Vector2> OnInteractionEnd;

        #endregion // Events

        #region Textures

        private static Texture _disabledBG;

        private static Texture _incBG5;
        private static Texture _incBG10;
        private static Texture _incBG20;

        private static Texture _rotaryBG;

        private static Texture _minMaxBG;

        private static Texture _precisionBG;

        private static Texture _notch;

        const int WIDTH = 44;
        const int HEIGHT = 44;

        #endregion // Textures

        #region Class Properties

        const int MINMAX_ROTATION_ZONE = 260;
        const int MINMAX_ROTATION_OFFSET = 230;
        const int INCREMENTAL_ROTATION_ZONE = 300;
        const int INCREMENTAL_ROTATION_OFFSET = 210;
        const int ROTARY_ROTATION_ZONE = 360;

        const string ASSET_PATH = FPSControlMainEditor.ASSET_PATH + "Graphics/Knobs"; //Our Assets path

        private static Themes _theme = Themes.WHITE; //our loaded theme
        private static Vector2 _lastMouseClickPosition; //the last mouse position (EventType.Click)

        private static float _sensitivity = 1F; //current sensitivity

        public const int HIGH_SENSITIVITY = 2;
        public const int DEFAULT_SENSITIVITY = 1; //default sensitivity for easy access
        public const float PRECISE_SENSITIVITY = 0.5F;

        private static EditorWindow parentWindow;

        //private static bool _inspector;

        #endregion // Class Properties

        #region Properties

        public static void Sensitivity(float sensitivity)
        {
            _sensitivity = sensitivity;
        }

        public static void Theme(Themes theme, EditorWindow parent)
        {
            parentWindow = parent;

            _theme = theme;
            _disabledBG = GetTexture("inactive_bg");

            _incBG5 = GetTexture("incremental_5_bg");
            _incBG10 = GetTexture("incremental_10_bg");
            _incBG20 = GetTexture("incremental_20_bg");

            _rotaryBG = GetTexture("rotary_bg");

            _minMaxBG = GetTexture("minMax_bg");

            _precisionBG = GetTexture("precision_box_bg");

            _notch = GetTexture("rotating_mark");
        }

        #endregion // Properties

        #region Knobs

        /**
            An incremental knob is for integers, between a min and a max value.
        */

        public static int Incremental(Vector2 position, ref float knobPosition, int value, Increments increments, uint id)
        {
            int i = Incremental(position, ref knobPosition, value, 0, (int)increments, increments, id);

            if (id == interactID) interactValue = i;

            return i;
        }

        public static int Incremental(Vector2 position, ref float knobPosition, int value, int min, int max, uint id)
        {
            //safety.
            if (max != 5 && max != 10 && max != 20)
            {
                throw new System.Exception("Not a compatible increment! You might want to try the overload with min, max AND an Increments parameter!");
            }

            int i = Incremental(position, ref knobPosition, value, min, max, (Increments)max, id);

            if (id == interactID) interactValue = i;

            return i;
        }

        public static int Incremental(Vector2 position, ref float knobPosition, int value, int min, int max, Increments increments, uint id)
        {
            //formulate a rect based on the desired position and the bg's dimensions
            Rect rect = new Rect(position.x, position.y, WIDTH, HEIGHT);

            //This gives us our true zero!
            Rect glRect = GetLocalSpaceOrigin(rect);
            //adjust our rect relative to the position of this component's inspector.
            rect = new Rect(glRect.x + rect.x, glRect.y + rect.y + glRect.height, WIDTH, HEIGHT);


            float degreesPerInt = (float)INCREMENTAL_ROTATION_ZONE / (float)(max - min); //how many degrees to the nearest snap point.	

            CheckClick(rect, id);

            float _degrees = GetRotationDegrees(rect,id);

            /** I'll be completely honest, I probably convoluted the hell out of this math, but it's working and I'd rather not push my luck today. */

            //add the accumulated degrees to our knob's current position
            knobPosition = Mathf.Clamp(knobPosition + _degrees, 0, (float)INCREMENTAL_ROTATION_ZONE);

            //calculate snapping for our visual representation and our output values.
            float _snap = knobPosition / (float)degreesPerInt;
            int newValue = Mathf.RoundToInt(((_snap * degreesPerInt) / (float)INCREMENTAL_ROTATION_ZONE) * max);
            float rotation = (newValue / (float)max) * (float)INCREMENTAL_ROTATION_ZONE;

            Texture _incBG = _incBG5;

            switch (increments)
            {
                case Increments.FIVE: _incBG = _incBG5; break;
                case Increments.TEN: _incBG = _incBG10; break;
                case Increments.TWENTY: _incBG = _incBG20; break;
            }

            //draw our knob.
            DrawKnob(rect, rotation + INCREMENTAL_ROTATION_OFFSET, _incBG, id);

            if (id == interactID) interactValue = newValue;

            return newValue;
        }

        /** 
            A rotary turns forever in both directions.
                Vector2 position : the position in your editor
                int value : the value this control modifies
        */

        public static int Rotary(Vector2 position, int value, uint id)
        {
            float i = value;

            //formulate a rect based on the desired position and the bg's dimensions
            Rect rect = new Rect(position.x, position.y, WIDTH, HEIGHT);

            //if we are clicking on something, cache it's position to see what knob we're inteacting with
            CheckClick(rect, id);

            Rect glRect = GetLocalSpaceOrigin(rect);

            //adjust our rect relative to the position of this component's inspector.
            rect = new Rect(glRect.x + rect.x, glRect.y + rect.y + glRect.height, WIDTH, HEIGHT);

            //get our rotation this frame
            float degrees = GetRotationDegrees(rect,id);

            //add our accumulated degrees to our current value. this means every rotation is worth 360.
            i = Mathf.Round(degrees) + value;

            int rotation = (int)Mathf.Repeat(i, ROTARY_ROTATION_ZONE);

            DrawKnob(rect, rotation, _rotaryBG, id);

            if (id == interactID) interactValue = i;

            return (int)i;
        }

        /**
            Much like incremental, but for floats. Has a min and max.
        */

        public static float MinMax(Vector2 position, float value, float min, float max, uint id, bool hideDecimal = false)
        {
            float i = value;

            value = Mathf.Clamp(value, min, max); //just for safety!		

            //formulate a rect based on the desired position and the bg's dimensions
            Rect rect = new Rect(position.x, position.y, WIDTH, HEIGHT);

            //if we are clicking on something, cache it's position to see what knob we're inteacting with
            CheckClick(rect, id);
            
            //This gives us our true zero!
            Rect glRect = GetLocalSpaceOrigin(rect);

            //adjust our rect relative to the position of this component's inspector.
            rect = new Rect(glRect.x + rect.x, glRect.y + rect.y + glRect.height, WIDTH, HEIGHT);

            float degrees = GetRotationDegrees(rect, id);

            //we take the accumulated degrees this loop and divide that by our maximum (300). 
            //This will give us a float value between our provided min and max which we can add to our value from last frame.
            i = ((degrees / (float)MINMAX_ROTATION_ZONE) * max);
            //if (i != 0) print(i);
            i += value;
            //We will now clamp i.
            i = Mathf.Clamp(i, min, max);
            //to do this we need to figure out what the normalized value
            float percent = (float)(i - min) / (float)(max - min);
            //the visible rotation has a dead zone of 60 degrees, which means the rotational representation of our max value is 300 degrees.
            float rotation = percent * MINMAX_ROTATION_ZONE;
            rotation = Mathf.Clamp(rotation, 0, MINMAX_ROTATION_ZONE);

            //the visible rotation also needs to be offset by 210 degrees so that our min (0) appears in the right place, 210 degrees clockwise.
            rotation += MINMAX_ROTATION_OFFSET;

            //draw our knob.
            DrawKnob(rect, rotation, _minMaxBG, id, hideDecimal);

            if (id == interactID) interactValue = i;

            return i;
        }

        #endregion // Knobs

        #region // Interaction

        static void CheckClick(Rect rect, uint id)
        {
            if (!GUI.enabled) return;

            //if (interactValue != id && interactValue != -1) return;

            Event evt = Event.current;

            //if we are clicking on something, cache it's position to see what knob we're inteacting with
            if (evt.type == EventType.MouseDown)
            {
                _lastMouseClickPosition = evt.mousePosition;

                if (rect.Contains(_lastMouseClickPosition))
                {
                    interactID = (int)id;
                    evt.Use();
                }
            }
            if (evt.type == EventType.MouseUp)
            {
                interactID = -1;
                _lastMouseClickPosition = Vector3.zero;
            }
        }

        static float GetRotationDegrees(Rect rect, uint id)
        {
            if (!GUI.enabled) return 0;

            Event evt = Event.current;

            //bool _dragging = (rect.Contains(_lastMouseClickPosition));
            bool _dragging = (interactID == id);            
            
            float _degrees = 0;
            if (evt.type == EventType.MouseDrag && _dragging)
            {
                float _delta = -evt.delta.y;
                _degrees = _delta * _sensitivity; //1px = 1 degree * sensitivity
                //interactID = (int)id;
                evt.Use();
            }
            else if (evt.type == EventType.ScrollWheel && rect.Contains(evt.mousePosition))
            {
                _degrees = evt.delta.y * _sensitivity;
                interactID = (int)id;
                evt.Use();
            }

           // if (_dragging && OnInteract != null) OnInteract(new Vector2(rect.x,rect.y));
            if (interactID == id) interactPos = new Vector2(rect.x, rect.y);

            return _degrees;
        }

        #endregion // Interaction

        #region Draw

        static void DrawKnob(Rect rect, float rotation, Texture tex, uint id, bool hideDecimal = false)
        {
            if (GUI.enabled)
            {
                GUI.DrawTexture(rect, tex);
                Matrix4x4 matrix = GUI.matrix;
                GUIUtility.RotateAroundPivot(rotation, new Vector2(rect.x + (WIDTH / 2F), rect.y + (HEIGHT / 2F)));
                GUI.DrawTexture(rect, _notch);
                GUI.matrix = matrix;
                if (interactID != -1 && id == interactID)
                {
                    GUIStyle gs = new GUIStyle();
                    gs.normal.textColor = Color.white;
                    gs.alignment = TextAnchor.MiddleCenter;
                    gs.normal.background = (Texture2D)_precisionBG;
                    Rect precisionBox = new Rect(interactPos.x - 18, interactPos.y + 45, 80, 25);
                    if (hideDecimal)
                    {
                        GUI.Box(precisionBox, "" + Mathf.Floor(interactValue), gs);
                    }
                    else
                    {
                        GUI.Box(precisionBox, "" + interactValue, gs);
                    }                    
                    parentWindow.Repaint();
                }
            }
            else
            {
                GUI.DrawTexture(rect, _disabledBG);
            }
        }

        #endregion // Draw

        #region Helpers

        static Rect GetLocalSpaceOrigin(Rect rect)
        {
            Rect glRect = new Rect(0, 0, 0, 0);
            return glRect;
        }

        static Texture GetTexture(string textureName)
        {
            Texture _tex;
            string path = ASSET_PATH + GetAssetPathByTheme() + "/" + textureName + ".png";
            _tex = (Texture)AssetDatabase.LoadAssetAtPath(path, typeof(Texture));
            return _tex;
        }

        static string GetAssetPathByTheme()
        {
            return "/" + ((_theme == Knobs.Themes.BLACK) ? "Black" : "White");
        }

        static void print(object msg)
        {
            #if DEBUG
		    Debug.Log("" + msg);					
            #endif
        }


        #endregion
    }
}