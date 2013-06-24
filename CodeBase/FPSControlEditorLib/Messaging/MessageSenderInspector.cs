using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using FPSControl;
using FPSControlEditor;

namespace FPSControlEditor
{

    [CustomEditor(typeof(MessageSender))]
    class MessageSenderInspector : FPSControlAbstractInspector
    {
        const string FOLDER = "Messaging/";
        const string TOP_BG = "blueBG";
        const string MID_BG = "greenBG";
        const string BOTTOM_BG = "darkGreenBG";
        const string HEADER = "message_text";
        const string TEXT = "sendMessageText";

        Texture2D topBG, midBG, bottomBG, header, text;
        GUIStyle bgStyle;
        
        MessageSender t { get { return (MessageSender)target; } }

        [MenuItem("FPS Control/Messaging/Add Sender To Selected GameObjects")]
        static void AddComponent()
        {
            foreach (GameObject go in Selection.gameObjects)
            {
                go.AddComponent<MessageSender>();
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
            
            Damageable attachedDamageable = t.GetComponent<Damageable>();
            Collider attachedCollider = t.GetComponent<Collider>();
            InteractLogic attachedInteractLogic = t.GetComponent<InteractLogic>();
            
            Color bgColor = GUI.backgroundColor;
            bgStyle = new GUIStyle();
            bgStyle.normal.background = topBG;
            //draw message box
            GUILayout.BeginVertical(bgStyle);

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label(header);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.Space(5);

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            t.broadcastedMessage = GUILayout.TextField(t.broadcastedMessage, GUILayout.MinWidth(200));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.EndVertical();

            GUI.backgroundColor = bgColor;

            GUILayout.Space(2);
            bgStyle.normal.background = midBG;
            GUILayout.BeginHorizontal(bgStyle);
            GUILayout.FlexibleSpace();
            GUILayout.Label(text);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.Space(2);

            bgStyle.normal.background = bottomBG;

            //Now we do our mutually exclusive handler setup

            GUILayout.BeginVertical();

            bool hb;

            //trigger
            GUILayout.BeginHorizontal(bgStyle);
            GUILayout.Space(5);

            hb = t.trigger == MessageTrigger.OnTriggerEnter;
            hb = GUILayout.Toggle(hb, "", EditorStyles.radioButton);
            t.trigger = hb ? MessageTrigger.OnTriggerEnter : t.trigger;
            GUI.enabled = hb;

            GUILayout.Space(10);
            GUILayout.Label("Another game object \"");
            GUILayout.Space(2);
            t.triggerObjectName = GUILayout.TextField(t.triggerObjectName, GUILayout.MinWidth(120));
            GUILayout.Space(2);
            GUILayout.Label("\" enters trigger ");

            if(hb && !attachedCollider)
                t.gameObject.AddComponent<BoxCollider>().isTrigger = true;
            else if(hb && !attachedCollider.isTrigger)
                attachedCollider.isTrigger = true;

            GUI.enabled = true;
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.Space(1);

            //health
            GUILayout.BeginHorizontal(bgStyle);
            GUILayout.Space(5);

            hb = t.trigger == MessageTrigger.OnDamage;
            hb = GUILayout.Toggle(hb, "", EditorStyles.radioButton);
            t.trigger = hb ? MessageTrigger.OnDamage : t.trigger;
            GUI.enabled = hb;

            GUILayout.Space(10);
            GUILayout.Label("Health of this object reaches:");
            GUILayout.Space(10);

            if (attachedDamageable && attachedCollider)
                t.objectHealth = EditorGUILayout.FloatField(t.objectHealth, GUILayout.Width(80));
            else
            {
                if (GUILayout.Button("Add Required Components"))
                {
                    if(!attachedDamageable) t.gameObject.AddComponent<Damageable>();
                    if (!attachedCollider) t.gameObject.AddComponent<BoxCollider>();
                }
            }

            GUI.enabled = true;
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.Space(1);

            //on destroy
            GUILayout.BeginHorizontal(bgStyle);
            GUILayout.Space(5);

            hb = t.trigger == MessageTrigger.OnDestroy;
            hb = GUILayout.Toggle(hb, "", EditorStyles.radioButton);
            t.trigger = hb ? MessageTrigger.OnDestroy : t.trigger;
            GUI.enabled = hb;

            GUILayout.Space(10);
            GUILayout.Label("This object is destroyed from the game");
            GUI.enabled = true;
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.Space(1);

            //on interact
            GUILayout.BeginHorizontal(bgStyle);
            GUILayout.Space(5);

            hb = t.trigger == MessageTrigger.OnInteraction;
            hb = GUILayout.Toggle(hb, "", EditorStyles.radioButton);
            t.trigger = hb ? MessageTrigger.OnInteraction : t.trigger;
            GUI.enabled = hb;

            GUILayout.Space(10);
            GUILayout.Label("Player interacts with this object");

            if (!attachedInteractLogic || !attachedCollider)
            {
                if (GUILayout.Button("Add Required Components"))
                {
                    if (!attachedInteractLogic) t.gameObject.AddComponent<InteractLogic>();
                    if (!attachedCollider) t.gameObject.AddComponent<BoxCollider>();
                }
            }

            GUI.enabled = true;
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.EndVertical();

            GUI.backgroundColor = Color.green;

            GUI.depth--;
            GUI.Box(GUILayoutUtility.GetLastRect(), "");
            GUI.depth++;

            GUI.backgroundColor = bgColor;
        }

    }
}
