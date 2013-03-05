//=====
//=====
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//---
//
//---
namespace FPSControl
{
    public class MusicControl : MonoBehaviour
    {    
        public float fadeSpeed = 1.0f;
        public List<MusicControlTrack> audioTracks;
        public AudioSource currentSource;
        public MusicControlTrack selectedTrack;
        public MusicControlTrack previousTrack;
        public float currentVolume = 0.0f;
        private bool stopped = true;
 
     
        //---
        //
        //---
        void OnEnable ()
        {
            MessengerControl.AddListener ("FadeIn", this, "PlayEffect");
            MessengerControl.AddListener ("FadeOut", this, "FadeEffectOut");
            MessengerControl.AddListener ("StopTrack", this, "StopAll");
            MessengerControl.AddListener ("ResumeTrack", this, "Resume");
        }
     
 
        //---
        //
        //---
        void OnDisable ()
        {
            MessengerControl.RemoveListener ("FadeIn", this, "PlayEffect");
            MessengerControl.RemoveListener ("FadeOut", this, "FadeEffectOut");
            MessengerControl.RemoveListener ("StopTrack", this, "StopAll");
            MessengerControl.RemoveListener ("ResumeTrack", this, "Resume");
        }
 
 
        //---
        //
        //---
        void Start ()
        {
            if (currentSource == null) {
                currentSource = gameObject.AddComponent<AudioSource> ();
                selectedTrack = null;
            }
        }
 
 
        //---
        //
        //---
        void PlayEffect (string name)
        {
            if (currentSource == null)
                Start ();
         
            stopped = false; 
         
            if (selectedTrack != null && selectedTrack.backgroundMusic) {
                previousTrack = selectedTrack;
                previousTrack.time = currentSource.time;
            }
         
            AudioSource newSource = gameObject.AddComponent<AudioSource> ();
         
            foreach (MusicControlTrack audioTrack in audioTracks) {
                if (audioTrack.name == name) {
                    selectedTrack = audioTrack;
                }
            }
         
            newSource.clip = selectedTrack.track;
            newSource.loop = selectedTrack.loop;
            newSource.Play ();
 
            StartCoroutine (FadeEffectIn (newSource));
         
            if (!selectedTrack.loop) {
                StartCoroutine (FadeOnEnd (newSource));
            }
     
        }
 
 
        //---
        //
        //---
        IEnumerator FadeEffectIn (AudioSource newSource)
        {
            currentVolume = currentSource.volume;
            AudioSource oldSource = currentSource;
            currentSource = newSource;
            float t = 0.0f;
         
            while (t <= 1) {
                t += Time.deltaTime / fadeSpeed;
                if (oldSource) {
                    oldSource.volume = Mathf.Lerp (currentVolume, 0, Mathf.SmoothStep (0.0f, 1.0f, t));
                }
                currentSource.volume = Mathf.Lerp (0, selectedTrack.volume, Mathf.SmoothStep (0.0f, 1.0f, t));
 
                yield return null;
            }
 
            Destroy (oldSource);
        }
     
     
        //---
        //
        //---
        void FadeEffectOut ()
        {
            if (previousTrack.track) {
                AudioSource previousSource = gameObject.AddComponent<AudioSource> ();
                selectedTrack = previousTrack;
                previousSource.clip = selectedTrack.track;
                previousSource.loop = selectedTrack.loop;
                previousSource.time = selectedTrack.time;
                previousSource.Play ();
                StartCoroutine (FadeEffectIn (previousSource));
            }
        }
 
 
        //---
        //
        //---
        public IEnumerator StopAll (bool pause)
        {
            currentVolume = currentSource.volume;
            float t = 0.0f;
            while (t <= 1) {
                t += Time.deltaTime / fadeSpeed;
                currentSource.volume = Mathf.Lerp (currentVolume, 0, Mathf.SmoothStep (0.0f, 1.0f, t));
                yield return 0;
            }
         
            if (pause) {
                currentSource.Pause ();
            } else {
                currentSource.Stop ();
            }
         
            stopped = true;
        }
 
 
        //---
        //
        //---
        public IEnumerator Resume ()
        {
            float t = 0.0f;
 
            currentSource.Play ();
         
            while (t <= 1) {
                t += Time.deltaTime / fadeSpeed;
                currentSource.volume = Mathf.Lerp (0, selectedTrack.volume, Mathf.SmoothStep (0.0f, 1.0f, t));
                yield return 0;
            }
        }
     
     
        //---
        //
        //---
        public IEnumerator FadeOnEnd (AudioSource newSource)
        {
            yield return new WaitForSeconds(newSource.clip.length - fadeSpeed);
            if (!stopped) {
                FadeEffectOut ();
            }
        }
 
    }
}