using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using FPSControl.Definitions;

namespace FPSControl
{
    [RequireComponent(typeof(AudioSource))]
    public class FPSControlWeaponSound : FPSControlWeaponComponent
    {
        [HideInInspector]
        public FPSControlWeaponSoundDefinition definition = new FPSControlWeaponSoundDefinition();
        [HideInInspector]
        public AudioClip equipSFX;
        [HideInInspector]
        public AudioClip fire1SFX;
        [HideInInspector]
        public AudioClip fire2SFX;
        [HideInInspector]
        public AudioClip fire3SFX;
        [HideInInspector]
        public AudioClip reloadSFX;
        [HideInInspector]
        public AudioClip emptySFX;

        public void SFX_Equip()
        {
            Debug.Log("HOW2");
            audio.clip = equipSFX;
            audio.Play();
        }

        public void SFX_Fire1()
        {
            audio.clip = fire1SFX;
            audio.Play();
        }

        public void SFX_Fire2()
        {
            audio.clip = fire2SFX;
            audio.Play();
        }

        public void SFX_Fire3()
        {
            audio.clip = fire3SFX;
            audio.Play();
        }

        public void SFX_Reload()
        {
            audio.clip = reloadSFX;
            audio.Play();
        }

        public void SFX_Empty()
        {
            audio.clip = emptySFX;
            audio.Play();
        }
    }
}
