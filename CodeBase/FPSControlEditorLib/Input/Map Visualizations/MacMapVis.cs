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
    public class MacMapVis : EditorControlMapVis<DesktopControlMap>, IEditorControlMapVis
    {
        const string MAC = "Mac";
        Texture outline, capsFill, cmdFill, escFill, funcFill, shiftFill, spaceFill, tabFill, keyFill;
        Texture mouseOutline, mouseLeft, mouseRight, mouseMotion;
        int row = 0, column = 0;

        public ControlMap Map { get { return map; } }

        public MacMapVis(DesktopControlMap focusedMap) : base(focusedMap)
        {
            LoadAssets();
        }

        override protected void LoadAssets()
        {
            outline = AssetLoader.LoadPNG(GENERAL_DIR + MAC, "outline");
            capsFill = AssetLoader.LoadPNG(GENERAL_DIR + MAC, "mac-caps");
            cmdFill = AssetLoader.LoadPNG(GENERAL_DIR + MAC, "mac-cmd");
            escFill = AssetLoader.LoadPNG(GENERAL_DIR + MAC, "mac-esc");
            funcFill = AssetLoader.LoadPNG(GENERAL_DIR + MAC, "mac-func");
            shiftFill = AssetLoader.LoadPNG(GENERAL_DIR + MAC, "mac-shift");
            spaceFill = AssetLoader.LoadPNG(GENERAL_DIR + MAC, "mac-space");
            tabFill = AssetLoader.LoadPNG(GENERAL_DIR + MAC, "mac-tab");
            keyFill = AssetLoader.LoadPNG(GENERAL_DIR + MAC, "mac-key");

            mouseOutline = AssetLoader.LoadPNG(GENERAL_DIR + MAC, "mouse-outline");
            mouseLeft = AssetLoader.LoadPNG(GENERAL_DIR + MAC, "mouse-left");
            mouseRight = AssetLoader.LoadPNG(GENERAL_DIR + MAC, "mouse-right");
            mouseMotion = AssetLoader.LoadPNG(GENERAL_DIR + MAC, "mouse-arrows");
        }

        public void Draw()
        {
            int width = outline.width;
            int height = outline.height;
            
            Rect drawRect = new Rect(20, 225, width, height);
            GUILayout.BeginArea(drawRect);

            //row 1
            DrawFill(new Vector2(8, 51), keyFill, KeyCode.BackQuote);
            DrawFill(new Vector2(38, 51), keyFill, KeyCode.Alpha1);
            DrawFill(new Vector2(67, 51), keyFill, KeyCode.Alpha2);
            DrawFill(new Vector2(97, 51), keyFill, KeyCode.Alpha3);
            DrawFill(new Vector2(127, 51), keyFill, KeyCode.Alpha4);
            DrawFill(new Vector2(157, 51), keyFill, KeyCode.Alpha5);
            DrawFill(new Vector2(187, 51), keyFill, KeyCode.Alpha6);
            DrawFill(new Vector2(217, 51), keyFill, KeyCode.Alpha7);
            DrawFill(new Vector2(247, 51), keyFill, KeyCode.Alpha8);
            DrawFill(new Vector2(277, 51), keyFill, KeyCode.Alpha9);
            DrawFill(new Vector2(207, 51), keyFill, KeyCode.Alpha0);
            DrawFill(new Vector2(338, 51), keyFill, KeyCode.Minus);
            DrawFill(new Vector2(368, 51), keyFill, KeyCode.Plus);
            DrawFill(new Vector2(397, 51), tabFill, KeyCode.Backspace);

            //row2
            DrawFill(new Vector2(8, 80), tabFill, KeyCode.Tab);
            DrawFill(new Vector2(53, 80), keyFill, KeyCode.Q);
            DrawFill(new Vector2(83, 80), keyFill, KeyCode.W);
            DrawFill(new Vector2(113, 80), keyFill, KeyCode.E);
            DrawFill(new Vector2(143, 80), keyFill, KeyCode.R);
            DrawFill(new Vector2(173, 80), keyFill, KeyCode.T);
            DrawFill(new Vector2(203, 80), keyFill, KeyCode.Y);
            DrawFill(new Vector2(233, 80), keyFill, KeyCode.U);
            DrawFill(new Vector2(263, 80), keyFill, KeyCode.I);
            DrawFill(new Vector2(293, 80), keyFill, KeyCode.O);
            DrawFill(new Vector2(323, 80), keyFill, KeyCode.P);
            DrawFill(new Vector2(353, 80), keyFill, KeyCode.LeftBracket);
            DrawFill(new Vector2(383, 80), keyFill, KeyCode.RightBracket);
            DrawFill(new Vector2(413, 80), keyFill, KeyCode.Backslash);

            //row3
            DrawFill(new Vector2(8, 109), capsFill, KeyCode.CapsLock);
            DrawFill(new Vector2(59, 109), keyFill, KeyCode.A);
            DrawFill(new Vector2(89, 109), keyFill, KeyCode.S);
            DrawFill(new Vector2(119, 109), keyFill, KeyCode.D);
            DrawFill(new Vector2(149, 109), keyFill, KeyCode.F);
            DrawFill(new Vector2(179, 109), keyFill, KeyCode.G);
            DrawFill(new Vector2(209, 109), keyFill, KeyCode.H);
            DrawFill(new Vector2(239, 109), keyFill, KeyCode.J);
            DrawFill(new Vector2(269, 109), keyFill, KeyCode.K);
            DrawFill(new Vector2(299, 109), keyFill, KeyCode.L);
            DrawFill(new Vector2(329, 109), keyFill, KeyCode.Semicolon);
            DrawFill(new Vector2(359, 109), keyFill, KeyCode.Quote);
            DrawFill(new Vector2(389, 109), capsFill, KeyCode.Return);
            
            //row4
            DrawFill(new Vector2(8, 138), shiftFill, KeyCode.LeftShift);
            DrawFill(new Vector2(75, 138), keyFill, KeyCode.Z);
            DrawFill(new Vector2(105, 138), keyFill, KeyCode.X);
            DrawFill(new Vector2(135, 138), keyFill, KeyCode.C);
            DrawFill(new Vector2(165, 138), keyFill, KeyCode.V);
            DrawFill(new Vector2(195, 138), keyFill, KeyCode.B);
            DrawFill(new Vector2(225, 138), keyFill, KeyCode.N);
            DrawFill(new Vector2(255, 138), keyFill, KeyCode.M);
            DrawFill(new Vector2(285, 138), keyFill, KeyCode.Comma);
            DrawFill(new Vector2(315, 138), keyFill, KeyCode.Period);
            DrawFill(new Vector2(345, 138), keyFill, KeyCode.Semicolon);
            DrawFill(new Vector2(375, 138), keyFill, KeyCode.Slash);
            DrawFill(new Vector2(405, 138), shiftFill, KeyCode.RightShift);

            //row5
            //DrawFill(new Vector2(8, 167), shiftFill, KeyCode.); //Fn?
            DrawFill(new Vector2(38, 167), funcFill, KeyCode.LeftControl);
            DrawFill(new Vector2(67, 167), funcFill, KeyCode.LeftAlt);
            DrawFill(new Vector2(97, 167), cmdFill, KeyCode.LeftApple);
            DrawFill(new Vector2(134, 167), spaceFill, KeyCode.Space);
            DrawFill(new Vector2(285, 167), cmdFill, KeyCode.RightApple);
            DrawFill(new Vector2(323, 167), funcFill, KeyCode.RightAlt);
            DrawFill(new Vector2(353, 184), escFill, KeyCode.LeftArrow);
            DrawFill(new Vector2(383, 167), escFill, KeyCode.UpArrow);
            DrawFill(new Vector2(383, 184), escFill, KeyCode.DownArrow);
            DrawFill(new Vector2(413, 184), escFill, KeyCode.RightArrow);

            GUILayout.EndArea();
            GUI.DrawTexture(new Rect(20, 225, 448, 206), outline);

            Rect mouseArea = new Rect(468, 186, 186, 247);
            GUILayout.BeginArea(mouseArea);
            DrawFill(new Vector2(5, 10), mouseMotion);
            DrawFill(new Vector2(46, 52), mouseLeft, 0);
            DrawFill(new Vector2(93, 52), mouseRight, 1);
            GUILayout.EndArea();
            GUI.DrawTexture(mouseArea, mouseOutline);

        }

        void DrawFill(Vector2 position, Texture texture, KeyCode keyCode)
        {
            ControlMap.ControlID id = map.GetIDBoundToControl(keyCode);
            if (id == ControlMap.ControlID.NONE) return; //if we don't have a match just return
            Color color = GUI.color;
            GUI.color = ColorByControl(id);
            Rect r = new Rect(position.x, position.y, texture.width, texture.height);
            GUI.DrawTexture(r, texture);
            GUI.color = color;
        }

        void DrawFill(Vector2 position, Texture texture, int mouseButton)
        {
            ControlMap.ControlID id = map.GetIDBoundToMouseButton(mouseButton);
            if (id == ControlMap.ControlID.NONE) return; //if we don't have a match just return
            Color color = GUI.color;
            GUI.color = ColorByControl(id);
            Rect r = new Rect(position.x, position.y, texture.width, texture.height);
            GUI.DrawTexture(r, texture);
            GUI.color = color;
        }

        void DrawFill(Vector2 position, Texture texture)
        {
            ControlMap.ControlID id = map.GetIDBoundToMouse();
            if (id == ControlMap.ControlID.NONE) return; //if we don't have a match just return
            Color color = GUI.color;
            GUI.color = ColorByControl(id);
            Rect r = new Rect(position.x, position.y, texture.width, texture.height);
            GUI.DrawTexture(r, texture);
            GUI.color = color;
        }

        public static Color ColorByControl(ControlMap.ControlID control)
        {
            switch (control)
            {
                case ControlMap.ControlID.Crouch: return new Color(89 / 255F, 86 / 255F, 224 / 255F, 1F);
                case ControlMap.ControlID.Jump: return new Color(42 / 255F, 51 / 255F, 237 / 255F, 1F);
                case ControlMap.ControlID.Defend: return Color.grey;
                case ControlMap.ControlID.Fire: return new Color(183 / 255F, 39 / 255F, 46 / 255F, 1F);
                case ControlMap.ControlID.Interact: return new Color(236 / 255F, 121 / 255F, 42 / 255F, 1F);
                case ControlMap.ControlID.Look: return Color.cyan;// new Color(89 / 255F, 86 / 255F, 224 / 255F, .8F);
                case ControlMap.ControlID.Move: return new Color(85 / 255F, 162 / 255F, 218 / 255F, 1F);
                case ControlMap.ControlID.Reload: return new Color(224 / 255F, 86 / 255F, 216 / 255F, 1F);
                case ControlMap.ControlID.Run: return new Color(247 / 255F, 236 / 255F, 23 / 255F, 1F);
                case ControlMap.ControlID.Scope: return new Color(132 / 255F, 71 / 255F, 183 / 255F, 1F);
                case ControlMap.ControlID.Weapon1:
                case ControlMap.ControlID.Weapon2:
                case ControlMap.ControlID.Weapon3:
                case ControlMap.ControlID.Weapon4: return new Color(62 / 255F, 178 / 255F, 72 / 255F, 1F);
                case ControlMap.ControlID.WeaponCycle: return new Color(46 / 255F, 108 / 255F, 43 / 255F, 1F);
            }

            return Color.clear;
        }

    }
}
