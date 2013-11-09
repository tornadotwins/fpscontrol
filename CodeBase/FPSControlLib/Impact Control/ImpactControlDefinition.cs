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
    [Serializable]
    public class ImpactControlEffect
    {
        public bool normalRotate = true;
        public GameObject effectObj = null;
    }

    [Serializable]
    public class ImpactControlDefinition
    {
        [SerializeField]
        /*!
        */
        public string name;
        [SerializeField]
        /*!
        */
        public string group;
        [SerializeField]
        /*!
        */
        public string tag;
        [SerializeField]
        /*!
        */
        public bool alignNormal;
        [SerializeField]
        /*!
        */
        public List<AudioClip> sounds = new List<AudioClip> ();
        [SerializeField]
        /*!
        */
        public List<ImpactControlEffect> particles = new List<ImpactControlEffect>();
        [SerializeField]
        /*!
        */
        public List<GameObject> decals = new List<GameObject>();
        [SerializeField]
        /*!
        */
        public List<Texture2D> textures = new List<Texture2D> ();
     
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
        public Vector2 particleScroll;
        /*!
        */
        public Vector2 decalScroll;
        /*!
        */
        public Vector2 textureScroll;
    }
}