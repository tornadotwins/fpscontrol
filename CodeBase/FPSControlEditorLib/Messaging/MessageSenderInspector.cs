using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using FPSControl;
using FPSControlEditor;

namespace FPSControlEditor
{

    [CustomEditor(typeof(MessageSender))]
    class MessageSenderInspector : Editor
    {
        MessageSender t { get { return (MessageSender)target; } }

        public override void OnInspectorGUI()
        {
            Color bgColor = GUI.backgroundColor;

            //draw message box
            GUILayout.BeginVertical();

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label("Send Message:");
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.Space(5);

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            t.broadcastedMessage = GUILayout.TextField(t.broadcastedMessage, GUILayout.MinWidth(120));
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

            bool hb;

            //trigger
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            hb = t.trigger == MessageTrigger.OnTriggerEnter;
            hb = GUILayout.Toggle(hb, "", EditorStyles.radioButton);
            t.trigger = hb ? MessageTrigger.OnTriggerEnter : t.trigger;
            GUI.enabled = hb;

            GUILayout.Space(10);
            GUILayout.Label("Another game object \"");
            GUILayout.Space(10);
            t.triggerObjectName = GUILayout.TextField(t.triggerObjectName, GUILayout.MinWidth(120));
            GUILayout.Space(10);
            GUILayout.Label("\" enters trigger ");
            GUI.enabled = true;
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            //health
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            hb = t.trigger == MessageTrigger.OnDamage;
            hb = GUILayout.Toggle(hb, "", EditorStyles.radioButton);
            t.trigger = hb ? MessageTrigger.OnDamage : t.trigger;
            GUI.enabled = hb;

            GUILayout.Space(10);
            GUILayout.Label("Health of this object reaches:");
            GUILayout.Space(10);
            t.objectHealth = EditorGUILayout.FloatField(t.objectHealth, GUILayout.MinWidth(120));
            GUI.enabled = true;
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            //on destroy
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            hb = t.trigger == MessageTrigger.OnDestroy;
            hb = GUILayout.Toggle(hb, "", EditorStyles.radioButton);
            t.trigger = hb ? MessageTrigger.OnDestroy : t.trigger;
            GUI.enabled = hb;

            GUILayout.Space(10);
            GUILayout.Label("This object is destroyed from the game");
            GUI.enabled = true;
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            //on destroy
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            hb = t.trigger == MessageTrigger.OnInteraction;
            hb = GUILayout.Toggle(hb, "", EditorStyles.radioButton);
            t.trigger = hb ? MessageTrigger.OnInteraction : t.trigger;
            GUI.enabled = hb;

            GUILayout.Space(10);
            GUILayout.Label("Player interacts with this object");
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
