﻿using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace FPSControl
{
    [RequireComponent(typeof(AudioSource))]
    public class FPSControlWeaponSound : FPSControlWeaponComponent
    {
        public AudioClip equipSFX;
        public AudioClip fire1SFX;
        public AudioClip fire2SFX;
        public AudioClip fire3SFX;
        public AudioClip reloadSFX;
        public AudioClip emptySFX;

        public void SFX_Equip()
        {
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
