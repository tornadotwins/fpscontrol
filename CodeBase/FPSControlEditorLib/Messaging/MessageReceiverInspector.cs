using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using FPSControl;
using FPSControlEditor;

namespace FPSControlEditor
{
    
    [CustomEditor(typeof(MessageReceiver))]
    class MessageReceiverInspector : FPSControlAbstractInspector
    {
        const string FOLDER = "Messaging/";
        const string TOP_BG = "blueBG";
        const string MID_BG = "redBG";
        const string BOTTOM_BG = "darkRedBG";
        const string HEADER = "receive_text";
        const string TEXT = "takeActionText";

        Texture2D topBG, midBG, bottomBG, header, text;
        GUIStyle bgStyle;

        MessageReceiver t { get { return (MessageReceiver)target; } }

        [MenuItem("FPS Control/Messaging/Add Receiver To Selected GameObjects")]
        static void AddComponent()
        {
            foreach (GameObject go in Selection.gameObjects)
            {
                go.AddComponent<MessageReceiver>();
            }
        }

        void GetAssets()
        {
            if (!topBG) topBG = (Texture2D)LoadPNG(FOLDER, TOP_BG);
            if (!midBG) midBG = (Texture2D)LoadPNG(FOLDER, MID_BG);
            if (!bottomBG) bottomBG = (Texture2D)LoadPNG(FOLDER, BOTTOM_BG);
            if (!header) header = (Texture2D)LoadPNG(FOLDER, HEADER);
            if (!text) text = (Texture2D)LoadPNG(FOLDER, TEXT);
        }

        public override void OnInspectorGUI()
        {
            GetAssets();
            
            Color bgColor = GUI.backgroundColor; 
            
            //draw message box
            bgStyle = new GUIStyle();
            bgStyle.normal.background = topBG;

            GUILayout.BeginVertical(bgStyle);

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label(header);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.Space(5);

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            t.broadcastedMessage = GUILayout.TextField(t.broadcastedMessage,GUILayout.MinWidth(200));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
           
            GUILayout.EndVertical();
            GUILayout.Space(2);
            GUI.backgroundColor = bgColor;

            bgStyle.normal.background = midBG;
            GUILayout.BeginHorizontal(bgStyle);
            GUILayout.FlexibleSpace();
            GUILayout.Label(text);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.Space(2);

            //Now we do our mutually exclusive handler setup
            bgStyle.normal.background = bottomBG;
            GUILayout.BeginVertical();

            MessageHandlerType handler = t.handler;
            bool hb;

            //spawn stuff
            GUILayout.BeginHorizontal(bgStyle);
            GUILayout.Space(5);

            hb = t.handler == MessageHandlerType.SpawnObject;
            hb = GUILayout.Toggle(hb, "", EditorStyles.radioButton);
            t.handler = hb ? MessageHandlerType.SpawnObject : t.handler;
            GUI.enabled = hb;

            GUILayout.Space(10);
            GUILayout.Label("Spawn object");
            GUILayout.Space(10);
            t.objectToSpawn = (GameObject)EditorGUILayout.ObjectField(t.objectToSpawn, typeof(GameObject), false);
            GUILayout.Space(10);
            GUILayout.Label("at location of: ");
            GUILayout.Space(10);
            t.spawnPosition = (Transform)EditorGUILayout.ObjectField(t.spawnPosition, typeof(Transform), true);
            GUI.enabled = true;
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.Space(1);

            //music stuff
            GUILayout.BeginHorizontal(bgStyle);
            GUILayout.Space(5);

            hb = t.handler == MessageHandlerType.Music;
            hb = GUILayout.Toggle(hb, "", EditorStyles.radioButton);
            t.handler = hb ? MessageHandlerType.Music : t.handler;
            GUI.enabled = hb;
            GUILayout.Space(10);
            GUILayout.Label("Switch music to: ");
            GUILayout.Space(10);
            t.music = GUILayout.TextField(t.music, GUILayout.MinWidth(120));
            GUI.enabled = true;
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.Space(1);

            //sfx stuff
            GUILayout.BeginHorizontal(bgStyle);
            GUILayout.Space(5);

            hb = t.handler == MessageHandlerType.SoundEffect;
            hb = GUILayout.Toggle(hb, "", EditorStyles.radioButton);
            t.handler = hb ? MessageHandlerType.SoundEffect : t.handler;
            GUI.enabled = hb;
            GUILayout.Space(10);
            GUILayout.Label("Play sound effect: ");
            GUILayout.Space(10);
            t.soundEffect = (AudioSource) EditorGUILayout.ObjectField(t.soundEffect, typeof(AudioSource), true);
            GUI.enabled = true;
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.Space(1);

            //player health stuff
            GUILayout.BeginHorizontal(bgStyle);
            GUILayout.Space(5);

            hb = t.handler == MessageHandlerType.PlayerHealth;
            hb = GUILayout.Toggle(hb, "", EditorStyles.radioButton);
            t.handler = hb ? MessageHandlerType.PlayerHealth : t.handler;
            GUI.enabled = hb;
            GUILayout.Space(10);
            GUILayout.Label("Modify player health: ");
            GUILayout.Space(10);
            t.healthMod = (MessageReceiver.HealthModifier)EditorGUILayout.EnumPopup(t.healthMod);
            GUILayout.Space(10);
            t.health = EditorGUILayout.FloatField(t.health, GUILayout.MinWidth(60));
            GUI.enabled = true;
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.Space(1);

            //animation stuff
            Animation siblingAnimation = t.GetComponent<Animation>();

            GUILayout.BeginHorizontal(bgStyle);
            GUILayout.Space(5);

            hb = t.handler == MessageHandlerType.PlayAnimation;
            hb = GUILayout.Toggle(hb, "", EditorStyles.radioButton);
            t.handler = hb ? MessageHandlerType.PlayAnimation : t.handler;
            GUI.enabled = hb;
            GUILayout.Space(10);
            GUILayout.Label("Play animation: ");
            GUILayout.Space(10);


            if (siblingAnimation)
            {
                t.animationName = GUILayout.TextField(t.animationName, GUILayout.MinWidth(120));
            }
            else
            {
                //GUILayout.Box("[No Animation Component]", EditorStyles.textField);
                if (GUILayout.Button("Add Animation Component"))
                {
                    t.gameObject.AddComponent<Animation>();
                    Repaint();
                }
            }
            
            GUI.enabled = true;
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.Space(1);

            //destroy stuff
            GUILayout.BeginHorizontal(bgStyle);
            GUILayout.Space(5);

            hb = t.handler == MessageHandlerType.DestroyThis;
            hb = GUILayout.Toggle(hb, "", EditorStyles.radioButton);
            t.handler = hb ? MessageHandlerType.DestroyThis : t.handler;
            GUI.enabled = hb;
            GUILayout.Space(10);
            GUILayout.Label("Destroy this object ");
            GUI.enabled = true;
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.EndVertical();

            GUI.backgroundColor = bgColor;
        }

    }
}
