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
        MessageReceiver t { get { return (MessageReceiver)target; } }

        [MenuItem("FPS Control/Messaging/Add Receiver To Selected GameObjects")]
        static void AddComponent()
        {
            foreach (GameObject go in Selection.gameObjects)
            {
                go.AddComponent<MessageReceiver>();
            }
        }

        public override void OnInspectorGUI()
        {
            Color bgColor = GUI.backgroundColor;
            
            //draw message box
            GUILayout.BeginVertical();

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label("Receive Message:");
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.Space(5);
            
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            t.broadcastedMessage = GUILayout.TextField(t.broadcastedMessage,GUILayout.MinWidth(120));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
           
            GUILayout.EndVertical();
            
            GUI.backgroundColor = Color.blue;

            GUI.depth--;
            GUI.Box(GUILayoutUtility.GetLastRect(), "");
            GUI.depth++;

            GUI.backgroundColor = bgColor;

            GUILayout.Space(10);

            //Now we do our mutually exclusive handler setup

            GUILayout.BeginVertical();

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label("Take Action:");
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            MessageHandlerType handler = t.handler;
            bool hb;

            //spawn stuff
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

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

            //music stuff
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

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

            //sfx stuff
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

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

            //player health stuff
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

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

            //animation stuff
            Animation siblingAnimation = t.GetComponent<Animation>();
            /*
             * List<string> clips = new List<string>();
            if (siblingAnimation )
            {
                foreach (AnimationState state in siblingAnimation)
                {
                    GUILayout.Label(state.clip.name, EditorStyles.miniBoldLabel);
                    clips.Add(state.clip.name);
                }
            }*/
            
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            hb = t.handler == MessageHandlerType.PlayAnimation;
            hb = GUILayout.Toggle(hb, "", EditorStyles.radioButton);
            t.handler = hb ? MessageHandlerType.PlayAnimation : t.handler;
            GUI.enabled = hb;
            GUILayout.Space(10);
            GUILayout.Label("Play animation: ");
            GUILayout.Space(10);
            /*if (GUI.enabled) Repaint();
            if (siblingAnimation && clips.Count > 0)
            {
                int indexOfClip = clips.Contains(t.animationName) ? clips.IndexOf(t.animationName) : 0;
                indexOfClip = EditorGUILayout.Popup(indexOfClip,clips.ToArray());
                t.animationName = clips[indexOfClip];
            }
            else if (siblingAnimation && clips.Count < 1)
            {
                GUILayout.Box("[Empty Animation Component]", EditorStyles.textField);
            }
            else
            {
                GUILayout.Box("[No Animation Component]", EditorStyles.textField);
            }
            clips = new List<string>();
             * */
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

            //destroy stuff
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

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

            GUI.backgroundColor = Color.red;

            GUI.depth--;
            GUI.Box(GUILayoutUtility.GetLastRect(), "");
            GUI.depth++;

            GUI.backgroundColor = bgColor;
        }

    }
}
