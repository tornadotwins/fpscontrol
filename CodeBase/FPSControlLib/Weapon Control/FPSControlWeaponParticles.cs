using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FPSControl
{
    [RequireComponent(typeof(Animation))]
    public class FPSControlWeaponParticles : FPSControlWeaponComponent
    {
        public static bool enableWarnings = true;
        
        [SerializeField] public FPSControlWeaponParticleData[] particles;
        public bool lightIsEnabled;
        public Light lightBurst;
        public Transform lightPosition;
        public float lightDuration = 1F;
        float _startingIntensity;

        void Awake()
        {
            foreach (FPSControlWeaponParticleData data in particles)
            {
                data.Initialize();
            }

            if (lightBurst)
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

            StopCoroutine("ShowLight");
            StartCoroutine("ShowLight");
        }

        public void KillFX(bool clear)
        {
            foreach (FPSControlWeaponParticleData data in particles)
            {
                data.Kill(clear);
            }

            if (clear)
            {
                StopCoroutine("ShowLight");
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

        public void Initialize()
        {
            _enabled = (isEnabled && position != null && particleSystem != null);

            if (!_enabled) return;
            particleSystem.transform.parent = position;
            particleSystem.transform.localPosition = Vector3.zero;
            particleSystem.transform.localRotation = Quaternion.identity;
            
            _shuriken = particleSystem.GetComponent<ParticleSystem>();
            if(_shuriken)
            {
                if(FPSControlWeaponParticles.enableWarnings) Debug.LogWarning("Note: Shuriken particles must be set to use Local/Global Space in the asset.");
            }
            else
            {
                _legacyEmitter = particleSystem.GetComponent<ParticleEmitter>();
                _legacyRenderer = particleSystem.GetComponent<ParticleRenderer>();
                _legacyAnimator = particleSystem.GetComponent<ParticleAnimator>();
                _legacyEmitter.useWorldSpace = global;
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
