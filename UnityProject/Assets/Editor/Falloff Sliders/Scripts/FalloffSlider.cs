using UnityEngine;
using UnityEditor;
using System.Collections;

public class FalloffSlider : object
{
    #region CONSTS

    const string ASSET_PATH = "Assets/Editor/Falloff Sliders/Editor Assets/Images/";
    const int WIDTH = 290;
    const int HEIGHT = 38;

    #endregion // CONSTS

    #region Assets

    internal static Material mat
    {
        get
        {
            Material m = new Material(Shader.Find("Hidden/Editor/GL"));
            m.color = color;
            return m;
        }
    }

    static Texture2D slider;
    static Texture2D grid;
    static Texture2D addButton;
    static Texture2D fieldBG;

    #endregion // Assets

    #region Properties

    public static Color color = Color.black;
    static FalloffSliderData _focusedData;
    static FalloffSliderPoint _focusedSlider;

    static FalloffSliderPoint focused
    { 
        get 
        {
            if (_focusedData == null || _focusedSlider == null)
            {
                //Debug.LogWarning("no slider!");
                return null;
            }
            return _focusedSlider; 
        }
    }

    #endregion // Properties

    #region Main

    public static FalloffSliderData DamageSlider(FalloffSliderData src, Vector2 position,System.Action Repaint)
    {
        bool enabled = GUI.enabled;
        
        //if(focused) GUILayout.Label("" + _focusedData + " [" + _focusedData.IndexOf(_focusedSlider) + "]");
        slider = GetTexture("slider");
        grid = GetTexture("grid");
        addButton = GetTexture("addButton");
        fieldBG = GetTexture("fieldBG");

        GUIStyle fieldStyle = new GUIStyle();
        fieldStyle.normal.background = fieldBG;
        fieldStyle.alignment = TextAnchor.MiddleCenter;
        fieldStyle.normal.textColor = Color.white;

        Event evt = Event.current;
        GUIStyle style;

        //Rect rect = new Rect(position.x, position.y, WIDTH, HEIGHT);

        //DRAW OUR ADD SLIDER BUTTON
        style = new GUIStyle();
        style.normal.background = addButton;
        
        GUI.enabled = src.distance > 0;
        GUI.SetNextControlName("ADD_BTN"); //we'll need this later to stop a bug from happening, other than that it has no affect.
        if (GUI.Button(new Rect(position.x + 258, position.y + 23, addButton.width, addButton.height), "", style))
        {
            src.Add(new FalloffSliderPoint(.5F, 1F));
        }
        GUI.enabled = enabled;
        //DRAW OUR DISTANCE FIELD HERE
        src.distance = EditorGUI.FloatField(new Rect(position.x + 258, position.y, 31, 16), src.distance, fieldStyle);
        if(src.distance < 0) src.distance = 0;
        GUI.enabled = src.distance > 0;
        FalloffSliderPoint[] points = src.ToArray();

        //DRAW THE GRAPH SLOPES
        for (int i = 0; i < points.Length; i++)
        {
            FalloffSliderPoint p = points[i];
            int x = (int) Mathf.Lerp(position.x, position.x + 252, p.location);
            int y = (int)Mathf.Lerp(position.y+19, position.y+2, p.value);
            if (i == 0)
            {
                DrawSlope(mat, new Vector2(position.x + 1, position.y + 2), new Vector2(x, y), new Vector2(x, position.y + 19), new Vector2(position.x, position.y + 19));
            }
            else
            {
                FalloffSliderPoint _p = points[i-1];
                int _x = (int)Mathf.Lerp(position.x, position.x + 252, _p.location);
                int _y = (int)Mathf.Lerp(position.y + 19,position.y + 2, _p.value);
                DrawSlope(mat, new Vector2(_x, _y), new Vector2(x, y), new Vector2(x, position.y + 19), new Vector2(_x, position.y + 19));
            }
            
            if (i == points.Length - 1) DrawSlope(mat, new Vector2(x, y), new Vector2(position.x + 252, position.y + 19), new Vector2(position.x + 252, position.y + 19), new Vector2(x, position.y + 19));
        }

        //DRAW THE GRID
        GUI.DrawTexture(new Rect(position.x, position.y, grid.width, grid.height), grid);

        //int currDepth = GUI.depth;
        Rect sliderRect = new Rect(0, 0, 0, 0);
        Rect focusedRect = new Rect(0, 0, 0, 0);

        //ITERATE THROUGH OUR SLIDER THUMBS
        foreach (FalloffSliderPoint p in points)
        {
            //Determine our rect for the current slider, based on the data of it's position (0-1) on the ruler and our control's global position as an offset
            sliderRect = new Rect
            (
                Mathf.Lerp(position.x - 7, position.x + 252 - 7, p.location),
                position.y + 10,
                13,
                12
            );

            //handle interaction here
            if (evt.type == EventType.MouseDown && evt.button == 0 && GUI.enabled)
            {
                if (sliderRect.Contains(evt.mousePosition))
                {
                    GUI.FocusControl("ADD_BTN"); //this really only serves a purpose to stopping a bug where text fields get confused.
                    _focusedData = src;
                    _focusedSlider = p;
                    evt.Use();
                }
            }
            else if (evt.type == EventType.MouseDown && evt.button == 1 && GUI.enabled) //Context clicking to remove a slider
            {
                if (sliderRect.Contains(evt.mousePosition))
                {
                    evt.Use();
                    src.Remove(p);
                    continue;
                }
            }
            else if (evt.type == EventType.MouseDrag && focused == p && GUI.enabled) //handling dragging
            {
                int i = src.IndexOf(p);
                float loc = p.location;

                int min = (int)position.x;
                //int max = 252 + min;

                float clampedMouse = Mathf.Clamp(evt.mousePosition.x - min, 0, 252);

                loc = Mathf.Clamp(clampedMouse / 252F, 0F, 1F);

                FalloffSliderPoint pt = new FalloffSliderPoint(loc, p.value);
                src[i] = pt;
                _focusedSlider = pt;
                evt.Use();
            }
            else if (evt.type == EventType.MouseUp || !GUI.enabled)
            {
                _focusedData = null;
                _focusedSlider = null;
            }

            //We'll only draw stuff in the loop if we aren't interacting with it currently, otherwise store it for after the loop executes so it draws on top of everything.
            if (focused == p)
            {
                focusedRect = sliderRect;
            }
            else
            {
                GUI.DrawTexture(sliderRect, slider);
                p.value = (float)EditorGUI.IntField(new Rect(sliderRect.x - 9, sliderRect.y + sliderRect.height, 31, 16), (int)Mathf.Clamp(p.value * 100, 0, 100),fieldStyle) / 100F;
            }
        }

        //If we have a focused slider that we're interacting with - draw it now.
        if(focused && _focusedData == src)
        {
            float meters = Mathf.RoundToInt((focused.location * src.distance) * 10) / 10F;
            GUI.DrawTexture(focusedRect, slider);
            GUI.Label(new Rect(focusedRect.x - 9, focusedRect.y + focusedRect.height, 31, 16), meters + "m", fieldStyle);
            //manually repaint so it doesn't look choppy
            Repaint();
        }

        GUI.enabled = enabled;
        //and finally we can return the value.
        return src;
    }

    public static FalloffSliderData FiringPatternSlider(FalloffSliderData src, AnimationClip clip, Vector2 position, System.Action Repaint)
    {
        bool enabled = GUI.enabled;

        //if(focused) GUILayout.Label("" + _focusedData + " [" + _focusedData.IndexOf(_focusedSlider) + "]");
        slider = GetTexture("slider");
        grid = GetTexture("grid");
        addButton = GetTexture("addButton");
        fieldBG = GetTexture("fieldBG");

        GUIStyle fieldStyle = new GUIStyle();
        fieldStyle.normal.background = fieldBG;
        fieldStyle.alignment = TextAnchor.MiddleCenter;
        fieldStyle.normal.textColor = Color.white;

        Event evt = Event.current;
        GUIStyle style;

        //Rect rect = new Rect(position.x, position.y, WIDTH, HEIGHT);

        //DRAW OUR ADD SLIDER BUTTON
        style = new GUIStyle();
        style.normal.background = addButton;

        GUI.enabled = (src.distance > 0 && clip);
        GUI.SetNextControlName("ADD_BTN"); //we'll need this later to stop a bug from happening, other than that it has no affect.
        if (GUI.Button(new Rect(position.x + 258, position.y + 23, addButton.width, addButton.height), "", style))
        {
            src.Add(new FalloffSliderPoint(.5F, 1F));
        }
        GUI.enabled = enabled;
        //DRAW OUR DISTANCE FIELD HERE
        src.distance = EditorGUI.FloatField(new Rect(position.x + 258, position.y, 31, 16), src.distance, fieldStyle);
        if (src.distance < 0) src.distance = 0;
        GUI.enabled = (src.distance > 0 && clip);
        FalloffSliderPoint[] points = src.ToArray();
        
        float clipLength = (clip != null) ? clip.length : 0F;
        float rise = 17;
        float run = (clipLength / src.distance) * 252F;
        float slope = rise / run;
        float origin = 0;
        float remainingX = run - 252;
        if (remainingX < 0) remainingX = 0;
        float remainingY = Mathf.Clamp(remainingX*slope,0,17);
      

        //DRAW THE GRAPH SLOPES
        if (src.distance > 0)
        {
            DrawSlope(mat, new Vector2(position.x, position.y + 2), new Vector2(Mathf.Clamp(position.x + run, position.x, position.x + 252), position.y + 19 - remainingY), new Vector2(Mathf.Clamp(position.x + run, position.x, position.x + 252), position.y + 19), new Vector2(position.x, position.y + 19));

            for (int i = 0; i < points.Length; i++)
            {
                FalloffSliderPoint p = points[i];
                origin = Mathf.Lerp(0, 252, p.location);

                int x = (int)(position.x + origin);
                //int y = (int)Mathf.Lerp(position.y + 19, position.y + 2, p.value);

                remainingX = (origin + run) - 252;
                if (remainingX < 0) remainingX = 0;
                remainingY = Mathf.Clamp(remainingX * slope, 0, 17);
                //GUILayout.Label("X: " + remainingX + " Y: " + remainingY); //debugging

                DrawSlope(mat, 
                    new Vector2(x, position.y + 2),
                    new Vector2(Mathf.Clamp(x + run, position.x, position.x + 252), position.y + 19 - remainingY), 
                    new Vector2(Mathf.Clamp(x + run, position.x, position.x + 252), position.y + 19), 
                    new Vector2(x, position.y + 19));
            }
        }
        //DRAW THE GRID
        GUI.DrawTexture(new Rect(position.x, position.y, grid.width, grid.height), grid);

        //int currDepth = GUI.depth;
        Rect sliderRect = new Rect(0, 0, 0, 0);
        Rect focusedRect = new Rect(0, 0, 0, 0);

        //ITERATE THROUGH OUR SLIDER THUMBS
        foreach (FalloffSliderPoint p in points)
        {
            //Determine our rect for the current slider, based on the data of it's position (0-1) on the ruler and our control's global position as an offset
            sliderRect = new Rect
            (
                Mathf.Lerp(position.x - 7, position.x + 252 - 7, p.location),
                position.y + 10,
                13,
                12
            );

            //handle interaction here
            if (evt.type == EventType.MouseDown && evt.button == 0 && GUI.enabled)
            {
                if (sliderRect.Contains(evt.mousePosition))
                {
                    GUI.FocusControl("ADD_BTN"); //this really only serves a purpose to stopping a bug where text fields get confused.
                    _focusedData = src;
                    _focusedSlider = p;
                    evt.Use();
                }
            }
            else if (evt.type == EventType.MouseDown && evt.button == 1 && GUI.enabled) //Context clicking to remove a slider
            {
                if (sliderRect.Contains(evt.mousePosition))
                {
                    evt.Use();
                    src.Remove(p);
                    continue;
                }
            }
            else if (evt.type == EventType.MouseDrag && focused == p && GUI.enabled) //handling dragging
            {
                int i = src.IndexOf(p);
                float loc = p.location;

                int min = (int)position.x;
                //int max = 252 + min;

                float clampedMouse = Mathf.Clamp(evt.mousePosition.x - min, 0, 252);

                loc = Mathf.Clamp(clampedMouse / 252F, 0F, 1F);

                FalloffSliderPoint pt = new FalloffSliderPoint(loc, p.value);
                src[i] = pt;
                _focusedSlider = pt;
                evt.Use();
            }
            else if (evt.type == EventType.MouseUp || !GUI.enabled)
            {
                _focusedData = null;
                _focusedSlider = null;
            }

            //We'll only draw stuff in the loop if we aren't interacting with it currently, otherwise store it for after the loop executes so it draws on top of everything.
            if (focused == p)
            {
                focusedRect = sliderRect;
            }
            else
            {
                GUI.DrawTexture(sliderRect, slider);
            }
        }

        //If we have a focused slider that we're interacting with - draw it now.
        if (focused && _focusedData == src)
        {
            float seconds = Mathf.RoundToInt((focused.location * src.distance) * 100) / 100F;
            GUI.DrawTexture(focusedRect, slider);
            GUI.Label(new Rect(focusedRect.x - 9, focusedRect.y + focusedRect.height, 31, 16), seconds + "s", fieldStyle);
            //manually repaint so it doesn't look choppy
            Repaint();
        }

        GUI.enabled = enabled;
        //and finally we can return the value.
        return src;
    }

    #endregion // Main

    #region Helpers

    static void DrawSlope(Material m, Vector2 v1, Vector2 v2, Vector2 v3, Vector2 v4)
    {
        GL.PushMatrix();
        m.SetPass(0);
        GL.LoadPixelMatrix();
        GL.Begin(GL.QUADS);
        GL.Color(color);
        GL.Vertex3(v1.x, v1.y, 0);
        GL.Vertex3(v2.x, v2.y, 0);
        GL.Vertex3(v3.x, v3.y, 0);
        GL.Vertex3(v4.x, v4.y, 0);
        GL.End();
        GL.PopMatrix();
    }

    static Texture2D GetTexture(string textureName)
    {
        Texture2D _tex;
        string path = ASSET_PATH + textureName + ".png";
        _tex = (Texture2D)AssetDatabase.LoadAssetAtPath(path, typeof(Texture2D));
        return _tex;
    }

    #endregion // Helpers

}
