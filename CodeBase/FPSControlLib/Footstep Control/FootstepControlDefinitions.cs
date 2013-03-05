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
    public class FootstepControlDefinitions : ScriptableObject
    {        
        [SerializeField]
        public List<FootstepControlDefinition> footsteps = new List<FootstepControlDefinition> ();
    }
}
