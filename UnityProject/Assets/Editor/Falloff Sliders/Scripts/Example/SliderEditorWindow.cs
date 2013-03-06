using UnityEngine;
using UnityEditor;
using System.Collections;

public class SliderEditorWindow : EditorWindow {

    FalloffSliderData data1;
    FalloffSliderData data2;
    AnimationClip clip;

    [MenuItem("Falloff Sliders/Test")]
    static void Init()
    {
        SliderEditorWindow window = EditorWindow.GetWindowWithRect<SliderEditorWindow>(new Rect(200, 200, 480, 320), true, "Falloff Sliders");
        window.Load();
    }

    void Load()
    {
       data1 = new FalloffSliderData();
       data2 = new FalloffSliderData();
    }

    void OnFocus()
    {
        //Load();
    }

    void OnGUI()
    {
        clip = (AnimationClip)EditorGUILayout.ObjectField("Animation:", clip, typeof(AnimationClip),false);
        if (!clip) GUILayout.Label("[!] Bottom Slider needs a clip here to work!");
        //FalloffSlider.color = EditorGUILayout.ColorField(FalloffSlider.color);
        FalloffSlider.color = Color.red;
        data1 = FalloffSlider.DamageSlider(data1, new Vector2(100, 120), Repaint);
        FalloffSlider.color = Color.cyan;
        data2 = FalloffSlider.FiringPatternSlider(data2, clip, new Vector2(100, 180), Repaint);
    }
}
