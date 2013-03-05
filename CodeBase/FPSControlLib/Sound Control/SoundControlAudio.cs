/*
jmpp: Done!
*/
using UnityEngine;
using System.Collections;


namespace FPSControl
{
    [RequireComponent (typeof(AudioSource))]
 

    //! Programatic interface to the most common properties of an <a href="http://docs.unity3d.com/Documentation/ScriptReference/AudioSource.html">AudioSource</a>.
    /*!
    The SoundControlAudio component provides a set of methods that act as a programatic interface to the most common properties of an <a href="http://docs.unity3d.com/Documentation/ScriptReference/AudioSource.html">AudioSource</a>.
    The main purpose of this class is to be used by the various audio-management methods of the SoundControl class, which call into this one by first
    attaching it as a component to a newly created gameObject and then sending a number of appropriate messages to it.
    */
    public class SoundControlAudio : MonoBehaviour
    {
        /* This is the second part of UPSound system.
      * When a new sound is played, OneShot or Loop this script is added to a new GameObject and
      * it takes care of that audio life. It automatically stops or loops the audio as you set
      * using the UpSound base class.
      * */
     
        float setTime;
        bool Started = false;
        int TotalLoops = 1;
        int CurrentLoops = 1;
        float FinishCorrection = 0.1f;
        bool Infinite = false;
 
        /*!
        Plays the <a href="http://docs.unity3d.com/Documentation/ScriptReference/AudioClip.html">AudioClip</a> parameter using the <a href="http://docs.unity3d.com/Documentation/ScriptReference/AudioSource.html">AudioSource</a>
        component attached to the gameObject.
        
        \param clip The <a href="http://docs.unity3d.com/Documentation/ScriptReference/AudioClip.html">AudioClip</a> asset to play.
        */
        public void StartAudio (AudioClip clip)
        {
            setTime = Time.time;
            audio.clip = clip;   
            audio.Play ();
            Started = true;
        }
     
        /*!
        Sets the volume of the <a href="http://docs.unity3d.com/Documentation/ScriptReference/AudioSource.html">AudioSource</a> component attached to the gameObject.
        
        \param Volume The volume at which the <a href="http://docs.unity3d.com/Documentation/ScriptReference/AudioSource.html">AudioSource</a> should play its current <a href="http://docs.unity3d.com/Documentation/ScriptReference/AudioClip.html">AudioClip</a>.
        */
        public void SetVolume (float Volume)
        {
            audio.volume = Volume;
        }
     
        /*!
        Sets the <a href="http://docs.unity3d.com/Documentation/ScriptReference/AudioRolloffMode.html">AudioRolloffMode</a> mode of the <a href="http://docs.unity3d.com/Documentation/ScriptReference/AudioSource.html">AudioSource</a>
        component attached to the gameObject to the passed-in parameter.
        
        It also hardcodes the <a href="http://docs.unity3d.com/Documentation/ScriptReference/AudioSource-maxDistance.html">maxDistance</a> property of the AudioSource to 40.
        
        \param mode The <a href="http://docs.unity3d.com/Documentation/ScriptReference/AudioRolloffMode.html">AudioRolloffMode</a> to use during playback.
        */
        public void SetMode (AudioRolloffMode mode)
        {
            audio.rolloffMode = mode;
            audio.maxDistance = 40f;
        }
     
        /*!
        Sets the <a href="http://docs.unity3d.com/Documentation/ScriptReference/AudioSource-minDistance.html">minDistance</a> property of the <a href="http://docs.unity3d.com/Documentation/ScriptReference/AudioSource.html">AudioSource</a>
        component attached to the gameObject to the passed-in float parameter.
        
        \param min The distance at which the <a href="http://docs.unity3d.com/Documentation/ScriptReference/AudioSource.html">AudioSource</a> will cease to grow louder when approaching it.
        */
        public void SetMinDistance (float min)
        {
            audio.minDistance = min;
        }
     
        /*!
        Sets the <a href="http://docs.unity3d.com/Documentation/ScriptReference/AudioSource-maxDistance.html">maxDistance</a> property of the <a href="http://docs.unity3d.com/Documentation/ScriptReference/AudioSource.html">AudioSource</a>
        component attached to the gameObject to the passed-in float parameter.
        
        \param max The distance from the <a href="http://docs.unity3d.com/Documentation/ScriptReference/AudioSource.html">AudioSource</a> at which the current <a href="http://docs.unity3d.com/Documentation/ScriptReference/AudioRolloffMode.html">AudioRolloffMode</a> will stop attenuating the sound.
        */
        public void SetMaxDistance (float max)
        {
            audio.maxDistance = max;
        }
     
        /*!
        Sets how many times the <a href="http://docs.unity3d.com/Documentation/ScriptReference/AudioSource.html">AudioSource</a> component attached to the
        gameObject should loop the playback of its current <a href="http://docs.unity3d.com/Documentation/ScriptReference/AudioClip.html">AudioClip</a>.
        If the passed in Loops parameter is -1, the AudioSource will loop the clip indefinitely.
        
        \param Loops The number of times to loop the <a href="http://docs.unity3d.com/Documentation/ScriptReference/AudioClip.html">AudioClip</a>.
        */
        public void SetLoops (int Loops)
        {
            if (Loops == -1) {
                Infinite = true;
                audio.loop = true;
            } else {
                audio.loop = true;
                TotalLoops = Loops;
                CurrentLoops = 1;
            }
        }
     
     
        /*!
        Performs a frame-by-frame monitoring of the looped playback of the <a href="http://docs.unity3d.com/Documentation/ScriptReference/AudioClip.html">AudioClip</a>
        assigned to the <a href="http://docs.unity3d.com/Documentation/ScriptReference/AudioSource.html">AudioSource</a> component in the gameObject, according
        to the loop count set through the SetLoops() method. Once the number of loops has been reached, and a "finish correction" time threshold of 0.1 seconds
        has elapsed, the gameObject holding this SoundControlAudio component is destroyed and all playback stops.
        
        The only exception to this rule is if infinite playback is requested through a loops parameter of -1 in SetLoops(), in which case the AudioClip will
        loop indefinitely.
        */
        void Update ()
        {
            if (Started && Time.time > (setTime + audio.clip.length)) {
                if (CurrentLoops >= TotalLoops && !Infinite) {
                    audio.Stop ();
                    audio.loop = false;
                    if (Time.time > (setTime + audio.clip.length + FinishCorrection)) {
                        //Once audio playing is finished object is destroyed
                        GameObject.Destroy (gameObject);
                    }
                 
                } else {
                    setTime = Time.time;
                    CurrentLoops++;
                }
            }
     
        }
    }
}