using UnityEngine;
using UnityEditor;
using System.Collections;
using System.IO;
using System.Collections.Generic;
using FPSControl;
using FPSControlEditor.Utils;
using FPSControl.Controls;

//---
//
//---
namespace FPSControlEditor.Controls
{
    public class OuyaMapVis : EditorControlMapVis<OuyaControlMap>, IEditorControlMapVis
    {
        const string OUYA = "Ouya";
        Texture dpd, dpl, dpr, dpu, lb, lt, btn, rb, rt, stick, stickBtn;
        Texture outline;

        public ControlMap Map { get { return map; } }

        public OuyaMapVis(OuyaControlMap focusedMap, System.Action RepaintCallback) : base(focusedMap, RepaintCallback)
        {
            LoadAssets();
        }

        override protected void LoadAssets()
        {
            outline = AssetLoader.LoadPNG(GENERAL_DIR + OUYA, "ouya-outline");
            dpd = AssetLoader.LoadPNG(GENERAL_DIR + OUYA, "ouya-dpd");
            dpl = AssetLoader.LoadPNG(GENERAL_DIR + OUYA, "ouya-dpl");
            dpr = AssetLoader.LoadPNG(GENERAL_DIR + OUYA, "ouya-dpr");
            dpu = AssetLoader.LoadPNG(GENERAL_DIR + OUYA, "ouya-dpu");
            lb = AssetLoader.LoadPNG(GENERAL_DIR + OUYA, "ouya-lb");
            lt = AssetLoader.LoadPNG(GENERAL_DIR + OUYA, "ouya-lt");
            btn = AssetLoader.LoadPNG(GENERAL_DIR + OUYA, "ouya-ouyabutton");
            rb = AssetLoader.LoadPNG(GENERAL_DIR + OUYA, "ouya-rb");
            rt = AssetLoader.LoadPNG(GENERAL_DIR + OUYA, "ouya-rt");
            stick = AssetLoader.LoadPNG(GENERAL_DIR + OUYA, "ouya-stick");
            stickBtn = AssetLoader.LoadPNG(GENERAL_DIR + OUYA, "ouya-stickbutton");
        }

        public void Draw()
        {
            int width = outline.width;
            int height = outline.height;
            
            Rect drawRect = new Rect(20, 225, width, height);
            GUILayout.BeginArea(drawRect);

            DrawButtonFill(new Vector2(144, 170), dpd, 13);
            DrawButtonFill(new Vector2(122, 145), dpl, 14);
            DrawButtonFill(new Vector2(170, 145), dpr, 12);
            DrawButtonFill(new Vector2(144, 124), dpu, 11);

            DrawButtonFill(new Vector2(65, 10), lb, 4);//lb
            DrawButtonFill(new Vector2(73, 1), lt, 5);//lt
            DrawButtonFill(new Vector2(283, 10), rb, 6);//rb
            DrawButtonFill(new Vector2(285, 1), rt, 7);//rt

            DrawButtonFill(new Vector2(315, 111), btn, 0);//o
            DrawButtonFill(new Vector2(287, 79), btn, 1);//u
            DrawButtonFill(new Vector2(315, 51), btn, 2);//y
            DrawButtonFill(new Vector2(346, 79), btn, 3);//a

            DrawAxisFill(new Vector2(76, 75), stick, 0);//ls
            DrawButtonFill(new Vector2(70, 69), stickBtn, 8);//l3

            DrawAxisFill(new Vector2(253, 137), stick, 2);//rs
            DrawButtonFill(new Vector2(248, 131), stickBtn, 9);//r3

            GUILayout.EndArea();
            GUI.DrawTexture(new Rect(20, 225, width, height), outline);

            Repaint();
        }

        void DrawButtonFill(Vector2 position, Texture texture, int i)
        {
            ControlMap.ControlID id = map.GetIDBoundToButton(i);
            if (id == ControlMap.ControlID.NONE) return; //if we don't have a match just return
            Color color = GUI.color;
            GUI.color = ColorByControl(id) - new Color(0, 0, 0, .25F);
            Rect r = new Rect(position.x, position.y, texture.width, texture.height);
            GUI.DrawTexture(r, texture);
            GUI.color = color;
        }

        void DrawAxisFill(Vector2 position, Texture texture, int i)
        {
            ControlMap.ControlID id = map.GetIDBoundToAxis(i);
            if (id == ControlMap.ControlID.NONE) return; //if we don't have a match just return
            Color color = GUI.color;
            GUI.color = ColorByControl(id) - new Color(0, 0, 0, .25F);
            Rect r = new Rect(position.x, position.y, texture.width, texture.height);
            GUI.DrawTexture(r, texture);
            GUI.color = color;
        }

        new public static Color ColorByControl(ControlMap.ControlID control)
        {
            return EditorControlMapVis<ControlMap>.ColorByControl(control);
        }

    }
}
