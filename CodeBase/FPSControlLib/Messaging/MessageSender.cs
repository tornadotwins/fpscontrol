using System.Collections;
using UnityEngine;
using FPSControl;
using System;

namespace FPSControl
{
    public enum MessageTrigger
    {
        NONE, OnTriggerEnter, OnDamage, OnDestroy, OnInteraction
    }

    public class MessageSender : InteractiveObject
    {
        public static event System.Action<string, MessageSender> OnMessageSend;

//      public bool oneShot = true;
        public string broadcastedMessage = "";
        public MessageTrigger trigger = MessageTrigger.NONE;

        public string triggerObjectName = "";
        public float objectHealth = 0F;

        Damageable _damageable;

        void Awake()
        {
            _damageable = GetComponent<Damageable>();
            if (_damageable) _damageable.OnDamageApplied += OnDamageRecieved;
        }

        void OnDamageRecieved(float currentHealth)
        {
            if (trigger == MessageTrigger.OnDamage && currentHealth <= objectHealth)
            {
                if (OnMessageSend != null) OnMessageSend(broadcastedMessage, this);
                //if (oneShot) Destroy(this);
            }
        }

        void OnTriggerEnter(Collider other)
        {
            if (trigger == MessageTrigger.OnTriggerEnter && other.gameObject.name == triggerObjectName)
            {
                if (OnMessageSend != null) OnMessageSend(broadcastedMessage, this);
                //if (oneShot) Destroy(this);
            }
        }

        override public void Interact()
        {
            if(trigger == MessageTrigger.OnInteraction)
            {
                if (OnMessageSend != null) OnMessageSend(broadcastedMessage, this);
                //if (oneShot) Destroy(this);
            }
        }

        void OnDestroy()
        {
            if (trigger == MessageTrigger.OnDestroy)
            {
                if (OnMessageSend != null) OnMessageSend(broadcastedMessage, null);
            }
        }
    }
}
