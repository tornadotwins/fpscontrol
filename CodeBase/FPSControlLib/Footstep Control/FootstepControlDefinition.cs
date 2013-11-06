/*
jmpp: In progress.
*/
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

//---
//
//---
namespace FPSControl
{
    /*!
    */
    public enum FOOTSTEPTYPES
    {
        /*!
        */
        Player,
        /*!
        */
        Npc
    }
 
    [Serializable]
    public class FootstepControlDefinition
    {
        [SerializeField]
        /*!
        */
        public string name;
        [SerializeField]
        /*!
        */
        public string tag;
        [SerializeField]
        /*!
        */
        public bool terrainCheck;
        [SerializeField]
        /*!
        */
        public List<AudioClip> sounds = new List<AudioClip> ();
        [SerializeField]
        /*!
        */
        public List<Texture2D> textures = new List<Texture2D> ();
        [SerializeField]
        /*!
        */
        public FOOTSTEPTYPES type;
     
        //This isnt very neat but ref/out wont work with get/set;
        /*!
        */
        public float minValPitch;
        /*!
        */
        public float minLimitPitch;
        /*!
        */
        public float maxValPitch;
        /*!
        */
        public float maxLimitPitch;
        /*!
        */
        public float minValVolume;
        /*!
        */
        public float minLimitVolume;
        /*!
        */
        public float maxValVolume;
        /*!
        */
        public float maxLimitVolume;
        /*!
        */
        public Vector2 audioScroll;
        /*!
        */
        public Vector2 textureScroll;
    }
}