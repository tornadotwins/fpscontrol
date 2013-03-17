﻿using System.Collections.Generic;
using System;
using System.Collections;
using FPSControl.Data;
using UnityEngine;
using FPSControl.Definitions;

namespace FPSControl
{
    [RequireComponent(typeof(Animation))]
    public class FPSControlWeaponParticles : FPSControlWeaponComponent
    {
        public static bool enableWarnings = true;

        public FPSControlWeaponParticlesDefinition definition = new FPSControlWeaponParticlesDefinition();
        
        [SerializeField] public FPSControlWeaponParticleData[] particles;

        [SerializeField]
        public bool lightIsEnabled;
        [SerializeField]
        public Light lightBurst;
        [SerializeField]
        public Transform lightPosition;
        [SerializeField]
        public float lightDuration = 1F;
        [SerializeField]
        float _startingIntensity;

        void Awake()
        {
            foreach (FPSControlWeaponParticleData data in particles)
            {
                data.Initialize();
            }

            if (lightBurst != null)
            {
                _startingIntensity = lightBurst.intensity;
                lightBurst.enabled = false;
                lightBurst.transform.parent = lightPosition;
                lightBurst.transform.localPosition = Vector3.zero;
                lightBurst.transform.localRotation = Quaternion.identity;
            }
        }

        public void SpawnFX()
        {
            foreach (FPSControlWeaponParticleData data in particles)
            {
                data.Spawn();
            }
        }

        public void KillFX_Clear()
        {
            KillFX(true);
        }

        public void KillFX()
        {
            KillFX(false);
        }

        private void KillFX(bool clear)
        {
            foreach (FPSControlWeaponParticleData data in particles)
            {
                data.Kill(clear);
            }

            if (lightBurst != null && clear)
            {
                lightBurst.enabled = false;
            }
        }

        IEnumerator ShowLight()
        {
            if (!lightIsEnabled && !lightBurst) yield break;
            lightBurst.enabled = true;
            yield return new WaitForSeconds(lightDuration);
            lightBurst.enabled = false;
        }
    }

    [System.Serializable]
    public class FPSControlWeaponParticleData : object
    {
        [SerializeField] 
        public GameObject particleSystem;
        public bool isLegacy{ get { return !_shuriken && (_legacyAnimator && _legacyEmitter && _legacyRenderer); } }
        
        ParticleSystem _shuriken;
        ParticleEmitter _legacyEmitter;
        ParticleRenderer _legacyRenderer;
        ParticleAnimator _legacyAnimator;

        [SerializeField] 
        public bool isEnabled;
        [SerializeField] 
        public Transform position;
        [SerializeField] 
        public bool global;
        
        private bool _enabled;

        private GameObject _particleSystem;

        public void Initialize()
        {
            _enabled = (isEnabled && position != null && particleSystem != null);

            if (!_enabled) return;
            _particleSystem = particleSystem;
            _particleSystem.transform.parent = position;
            _particleSystem.transform.localPosition = Vector3.zero;
            _particleSystem.transform.localRotation = Quaternion.identity;
            _shuriken = _particleSystem.GetComponent<ParticleSystem>();
            if(_shuriken)
            {
                if(FPSControlWeaponParticles.enableWarnings) Debug.LogWarning("Note: Shuriken particles must be set to use Local/Global Space in the asset.");
            }
            else
            {
                _legacyEmitter = _particleSystem.GetComponent<ParticleEmitter>();
                _legacyRenderer = _particleSystem.GetComponent<ParticleRenderer>();
                _legacyAnimator = _particleSystem.GetComponent<ParticleAnimator>();
                if (_legacyEmitter == null || _legacyRenderer == null || _legacyAnimator == null)
                {
                    _enabled = false;
                }
                else
                {
                    _legacyEmitter.useWorldSpace = global;
                }
            }
        }

        public void Spawn()
        {
            if (!_enabled) return;
            if (isLegacy)
            {
                _legacyEmitter.Emit();
            }
            else
            {
                _shuriken.Play();
            }
        }

        public void Kill(bool clear)
        {
            if (!_enabled) return;
            if (isLegacy)
            {
                _legacyEmitter.emit = false;
                if (clear) _legacyEmitter.ClearParticles();
            }
            else
            {
                _shuriken.Stop();
                if (clear) _shuriken.Clear();
            }
        }
    }
}
