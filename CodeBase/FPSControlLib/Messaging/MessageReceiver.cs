using System.Collections;
using UnityEngine;
using FPSControl;
using System;

namespace FPSControl
{
    public enum MessageHandlerType
    {
        NONE,SpawnObject,Music,SoundEffect,PlayerHealth,PlayAnimation,DestroyThis
    }
    
    public class MessageReceiver : MonoBehaviour
    {
        public enum HealthModifier { Plus, Minus, SetTo }
        
        public string broadcastedMessage = "";
        public MessageHandlerType handler = MessageHandlerType.NONE;
        public GameObject objectToSpawn;
        public Transform spawnPosition;

        public string music = "";
        public AudioSource soundEffect;

        public HealthModifier healthMod = HealthModifier.Plus;
        public float health;

        public string animationName;

        void Awake()
        {
            MessageSender.OnMessageSend += OnMessageReceived;
        }

        void OnMessageReceived(string message, MessageSender sender)
        {
            if (message == broadcastedMessage)
            {
                switch (handler)
                {
                    case MessageHandlerType.DestroyThis: 
                        Destroy(gameObject); 
                    break;
                    
                    case MessageHandlerType.SpawnObject:
                        Instantiate(objectToSpawn, spawnPosition.position, spawnPosition.rotation); 
                    break;
                    
                    case MessageHandlerType.Music:
                        MessengerControl.Broadcast("FadeIn", music);
                    break;
                    
                    case MessageHandlerType.SoundEffect:
                        soundEffect.Play();
                    break;
                    
                    case MessageHandlerType.PlayerHealth:
                        DataController dc = null;

                        GameObject[] playerTaggedGOs = GameObject.FindGameObjectsWithTag("Player");
                        
                        foreach(GameObject go in playerTaggedGOs)
                        {
                            dc = go.GetComponent<DataController>();
                            if (dc) break;
                        }

                        if (!dc) Debug.LogError("Could not find data controller on Player");
                        
                    switch (healthMod)
                        {
                            case HealthModifier.Minus: dc.SendMessage("ApplyHealthAdditive", -Mathf.Abs(health)); break;
                            case HealthModifier.Plus: dc.SendMessage("ApplyHealthAdditive", Mathf.Abs(health)); break;
                            case HealthModifier.SetTo: dc.SendMessage("ApplyHealth",health); break;
                        }

                    break;
                    
                    case MessageHandlerType.PlayAnimation:
                        animation.Play(animationName); 
                    break;
                }
            }
        }

        void OnDestroy()
        {
            MessageSender.OnMessageSend -= OnMessageReceived;
        }
    }
}
