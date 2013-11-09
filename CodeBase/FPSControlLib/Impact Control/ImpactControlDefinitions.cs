/*
jmpp: In progress.
*/
using UnityEngine;
using System;
using System.Collections.Generic;

//---
//
//---
namespace FPSControl
{
    [Serializable]
    public class ImpactControlDefinitions : ScriptableObject
    {
        [SerializeField]
        public bool terrainCheck;
        /*!
        */
        [SerializeField]
        public List<ImpactControlDefinition> impacts = new List<ImpactControlDefinition>();
    }
}
