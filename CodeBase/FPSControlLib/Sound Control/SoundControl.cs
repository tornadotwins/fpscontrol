/*
jmpp: Done!
*/
/* This is an exclusive plugin for Gameprefabs store. */
using UnityEngine;
using System.Collections;


namespace FPSControl
{

    //! Assortment of static methods that handle AudioClip playback.
    /*!
    The SoundControl static class provides an assortment of static methods that play in different ways <a href="http://docs.unity3d.com/Documentation/ScriptReference/AudioClip.html">AudioClips</a>
    stored in the <a href="http://docs.unity3d.com/Documentation/Manual/LoadingResourcesatRuntime.html">Resources folder</a>,
    wherefrom Unity loads them by full path (e.g. "charactersounds/hit.mp3") through the <a href="http://docs.unity3d.com/Documentation/ScriptReference/Resources.Load.html">Resources.Load()</a> method.
    
    There are two basic grops of methods, PlayOneShot() and PlayLoop(), each with several overloads that provide for different calling needs.
    The PlayOneShot group of overloads provides for a one-off playing of an AudioClip, while the PlayLoop group allows the callers to control the number of
    times the AudioClip is looped.
    */
    public static class SoundControl
    {
     
        /*!
        Upon loading the "audioDIR" <a href="http://docs.unity3d.com/Documentation/ScriptReference/AudioClip.html">AudioClip</a> by full pathname (e.g. "charactersounds/hit.mp3")
        from the <a href="http://docs.unity3d.com/Documentation/Manual/LoadingResourcesatRuntime.html">Resources folder</a>, this overload of
        the PlayOneShot method creates a new gameObject and attaches a SoundControlAudio component to it, sending it the "StartAudio" message
        and the AudioClip to play.
        
        The new gameObject is returned to the caller.
        
        \param audioDIR The name of the <a href="http://docs.unity3d.com/Documentation/ScriptReference/AudioClip.html">AudioClip</a> to load from the <a href="http://docs.unity3d.com/Documentation/Manual/LoadingResourcesatRuntime.html">Resources folder</a>.
        
        \retval GameObject The new gameObject with the SoundControlAudio component attached to it.
        */
        public static GameObject PlayOneShot (string audioDIR)
        {
            AudioClip clip = (AudioClip)Resources.Load (audioDIR);
            GameObject audioGO = new GameObject ();
            audioGO.AddComponent<SoundControlAudio> ();
            audioGO.SendMessage ("StartAudio", clip);
            return audioGO;
        }

          
        /*!
        Upon loading the "audioDIR" <a href="http://docs.unity3d.com/Documentation/ScriptReference/AudioClip.html">AudioClip</a> by full pathname (e.g. "charactersounds/hit.mp3")
        from the <a href="http://docs.unity3d.com/Documentation/Manual/LoadingResourcesatRuntime.html">Resources folder</a>, this overload of
        the PlayOneShot method creates a new gameObject and attaches a SoundControlAudio component to it, sending it the "StartAudio" message
        and the AudioClip to play, along with the "SetVolume" message and the target volume.
        
        The new gameObject is returned to the caller.
        
        \param audioDIR The name of the <a href="http://docs.unity3d.com/Documentation/ScriptReference/AudioClip.html">AudioClip</a> to load from the <a href="http://docs.unity3d.com/Documentation/Manual/LoadingResourcesatRuntime.html">Resources folder</a>.
        \param Volume The volume at which the "audioDIR" <a href="http://docs.unity3d.com/Documentation/ScriptReference/AudioClip.html">AudioClip</a> should be played.
        
        \retval GameObject The new gameObject with the SoundControlAudio component attached to it.
        */
        public static GameObject PlayOneShot (string audioDIR, float Volume)
        {
            AudioClip clip = (AudioClip)Resources.Load (audioDIR);
            GameObject audioGO = new GameObject ();
            audioGO.AddComponent<SoundControlAudio> ();
            audioGO.SendMessage ("StartAudio", clip);
            audioGO.SendMessage ("SetVolume", Volume);
            return audioGO;
        }
     
          
        /*!
        Upon loading the "audioDIR" <a href="http://docs.unity3d.com/Documentation/ScriptReference/AudioClip.html">AudioClip</a> by full pathname (e.g. "charactersounds/hit.mp3")
        from the <a href="http://docs.unity3d.com/Documentation/Manual/LoadingResourcesatRuntime.html">Resources folder</a>, this overload of
        the PlayOneShot method creates a new gameObject and attaches a SoundControlAudio component to it, sending it the "StartAudio" message
        and the AudioClip to play, along with the "SetVolume" message and the target volume.
        
        The new gameObject is made a child of the passed-in "parent" transform, relocated to the same position as the parent, and then returned
        to the caller. If the requested clip is not configured as a 3D AudioClip in its <a href="http://docs.unity3d.com/Documentation/Components/class-AudioClip.html">import settings</a>,
        this relocation of the created gameObject will have no effect on the playback, since the clip will be effectively acting as a 2D sound.
        
        \param audioDIR The name of the <a href="http://docs.unity3d.com/Documentation/ScriptReference/AudioClip.html">AudioClip</a> to load from the <a href="http://docs.unity3d.com/Documentation/Manual/LoadingResourcesatRuntime.html">Resources folder</a>.
        \param Volume The volume at which the "audioDIR" <a href="http://docs.unity3d.com/Documentation/ScriptReference/AudioClip.html">AudioClip</a> should be played.
        \param parent The transform that will act as parent to the gameObject created to play the requested <a href="http://docs.unity3d.com/Documentation/ScriptReference/AudioClip.html">clip</a>.
        
        \retval GameObject The new gameObject with the SoundControlAudio component attached to it.
        */
        public static GameObject PlayOneShot (string audioDIR, float Volume, Transform parent)
        {
            AudioClip clip = (AudioClip)Resources.Load (audioDIR);
            GameObject audioGO = new GameObject ();
            audioGO.AddComponent<SoundControlAudio> ();
            audioGO.SendMessage ("StartAudio", clip);
            audioGO.SendMessage ("SetVolume", Volume);
            audioGO.transform.parent = parent;
            audioGO.transform.position = parent.position;
            return audioGO;
        }
     
          
        /*!
        Upon loading the "audioDIR" <a href="http://docs.unity3d.com/Documentation/ScriptReference/AudioClip.html">AudioClip</a> by full pathname (e.g. "charactersounds/hit.mp3")
        from the <a href="http://docs.unity3d.com/Documentation/Manual/LoadingResourcesatRuntime.html">Resources folder</a>, this overload of
        the PlayOneShot method creates a new gameObject, sets the position of its transform to the requested location, and attaches a SoundControlAudio
        component to it, sending it the "StartAudio" message and the AudioClip to play.
        
        If the requested clip is not configured as a 3D AudioClip in its <a href="http://docs.unity3d.com/Documentation/Components/class-AudioClip.html">import settings</a>,
        changing the position at which it is played will have no effect on the playbak, since the clip will be effectively acting as a 2D sound.
        
        The new gameObject is returned to the caller.

        \param audioDIR The name of the <a href="http://docs.unity3d.com/Documentation/ScriptReference/AudioClip.html">AudioClip</a> to load from the <a href="http://docs.unity3d.com/Documentation/Manual/LoadingResourcesatRuntime.html">Resources folder</a>.
        \param position The global position at which to play the "audioDIR" <a href="http://docs.unity3d.com/Documentation/ScriptReference/AudioClip.html">AudioClip</a>.
        
        \retval GameObject The new gameObject with the SoundControlAudio component attached to it.
        */
        public static GameObject PlayOneShot (string audioDIR, Vector3 position)
        {
            AudioClip clip = (AudioClip)Resources.Load (audioDIR);
            GameObject audioGO = new GameObject ();
            audioGO.AddComponent<SoundControlAudio> ();
            audioGO.SendMessage ("StartAudio", clip);
            audioGO.transform.position = position;
            return audioGO;
        }
     
          
        /*!
        Upon loading the "audioDIR" <a href="http://docs.unity3d.com/Documentation/ScriptReference/AudioClip.html">AudioClip</a> by full pathname (e.g. "charactersounds/hit.mp3")
        from the <a href="http://docs.unity3d.com/Documentation/Manual/LoadingResourcesatRuntime.html">Resources folder</a>, this overload of
        the PlayOneShot method creates a new gameObject, sets the position of its transform to the requested location, and attaches a SoundControlAudio
        component to it, sending it the "StartAudio" message and the AudioClip to play, along with the "SetVolume" message and the target volume.
        
        If the requested clip is not configured as a 3D AudioClip in its <a href="http://docs.unity3d.com/Documentation/Components/class-AudioClip.html">import settings</a>,
        changing the position at which it is played will have no effect on the playbak, since the clip will be effectively acting as a 2D sound.
                
        The new gameObject is returned to the caller.
        
        \param audioDIR The name of the <a href="http://docs.unity3d.com/Documentation/ScriptReference/AudioClip.html">AudioClip</a> to load from the <a href="http://docs.unity3d.com/Documentation/Manual/LoadingResourcesatRuntime.html">Resources folder</a>.
        \param Volume The volume at which the "audioDIR" <a href="http://docs.unity3d.com/Documentation/ScriptReference/AudioClip.html">AudioClip</a> should be played.
        \param position The global position at which to play the "audioDIR" <a href="http://docs.unity3d.com/Documentation/ScriptReference/AudioClip.html">AudioClip</a>.
        
        \retval GameObject The new gameObject with the SoundControlAudio component attached to it.
        */
        public static GameObject PlayOneShot (string audioDIR, float Volume, Vector3 position)
        {
            AudioClip clip = (AudioClip)Resources.Load (audioDIR);
            GameObject audioGO = new GameObject ();
            audioGO.AddComponent<SoundControlAudio> ();
            audioGO.SendMessage ("StartAudio", clip);
            audioGO.SendMessage ("SetVolume", Volume);
            audioGO.transform.position = position;
            return audioGO;
        }
     
          
        /*!
        Upon loading the "audioDIR" <a href="http://docs.unity3d.com/Documentation/ScriptReference/AudioClip.html">AudioClip</a> by full pathname (e.g. "charactersounds/hit.mp3")
        from the <a href="http://docs.unity3d.com/Documentation/Manual/LoadingResourcesatRuntime.html">Resources folder</a>, this overload of
        the PlayOneShot method creates a new gameObject, sets the position of its transform to the requested location, and attaches a SoundControlAudio
        component to it, sending it the "StartAudio" message and the AudioClip to play, the "SetVolume" message and the target volume, and
        finally the "SetMode" message and the requested <a href="http://docs.unity3d.com/Documentation/ScriptReference/AudioRolloffMode.html">AudioRolloffMode</a>
        to use during playback.
        
        If the requested clip is not configured as a 3D AudioClip in its <a href="http://docs.unity3d.com/Documentation/Components/class-AudioClip.html">import settings</a>,
        changing the position at which it is played will have no effect on the playbak, since the clip will be effectively acting as a 2D sound.
        
        The new gameObject is returned to the caller.
        
        \param audioDIR The name of the <a href="http://docs.unity3d.com/Documentation/ScriptReference/AudioClip.html">AudioClip</a> to load from the <a href="http://docs.unity3d.com/Documentation/Manual/LoadingResourcesatRuntime.html">Resources folder</a>.
        \param Volume The volume at which the "audioDIR" <a href="http://docs.unity3d.com/Documentation/ScriptReference/AudioClip.html">AudioClip</a> should be played.
        \param position The global position at which to play the "audioDIR" <a href="http://docs.unity3d.com/Documentation/ScriptReference/AudioClip.html">AudioClip</a>.
        \param mode The <a href="http://docs.unity3d.com/Documentation/ScriptReference/AudioRolloffMode.html">AudioRolloffMode</a> to use during playback of the "audioDIR" <a href="http://docs.unity3d.com/Documentation/ScriptReference/AudioClip.html">AudioClip</a>.
        
        \retval GameObject The new gameObject with the SoundControlAudio component attached to it.
        */
        public static GameObject PlayOneShot (string audioDIR, float Volume, Vector3 position, AudioRolloffMode mode)
        {
            AudioClip clip = (AudioClip)Resources.Load (audioDIR);
            GameObject audioGO = new GameObject ();
            audioGO.AddComponent<SoundControlAudio> ();
            audioGO.SendMessage ("StartAudio", clip);
            audioGO.SendMessage ("SetVolume", Volume);
            audioGO.transform.position = position;
            audioGO.SendMessage ("SetMode", mode);
            return audioGO;
        }
     
          
        /*!
        Upon loading the "audioDIR" <a href="http://docs.unity3d.com/Documentation/ScriptReference/AudioClip.html">AudioClip</a> by full pathname (e.g. "charactersounds/hit.mp3")
        from the <a href="http://docs.unity3d.com/Documentation/Manual/LoadingResourcesatRuntime.html">Resources folder</a>, this overload of
        the PlayOneShot method creates a new gameObject and attaches a SoundControlAudio component to it, sending it the "StartAudio" message
        and the AudioClip to play, the "SetVolume" message and the target volume, and finally the "SetMode" message and the requested <a href="http://docs.unity3d.com/Documentation/ScriptReference/AudioRolloffMode.html">AudioRolloffMode</a>
        to use during playback.
        
        The new gameObject is made a child of the passed-in "parent" transform, relocated to the same position as the parent, and then returned
        to the caller. If the requested clip is not configured as a 3D AudioClip in its <a href="http://docs.unity3d.com/Documentation/Components/class-AudioClip.html">import settings</a>,
        this relocation of the created gameObject will have no effect on the playback, since the clip will be effectively acting as a 2D sound.
        
        \param audioDIR The name of the <a href="http://docs.unity3d.com/Documentation/ScriptReference/AudioClip.html">AudioClip</a> to load from the <a href="http://docs.unity3d.com/Documentation/Manual/LoadingResourcesatRuntime.html">Resources folder</a>.
        \param Volume The volume at which the "audioDIR" <a href="http://docs.unity3d.com/Documentation/ScriptReference/AudioClip.html">AudioClip</a> should be played.
        \param parent The transform that will act as parent to the gameObject created to play the requested <a href="http://docs.unity3d.com/Documentation/ScriptReference/AudioClip.html">clip</a>.
        \param mode The <a href="http://docs.unity3d.com/Documentation/ScriptReference/AudioRolloffMode.html">AudioRolloffMode</a> to use during playback of the "audioDIR" <a href="http://docs.unity3d.com/Documentation/ScriptReference/AudioClip.html">AudioClip</a>.
        
        \retval GameObject The new gameObject with the SoundControlAudio component attached to it.
        */
        public static GameObject PlayOneShot (string audioDIR, float Volume, Transform parent, AudioRolloffMode mode)
        {
            AudioClip clip = (AudioClip)Resources.Load (audioDIR);
            GameObject audioGO = new GameObject ();
            audioGO.AddComponent<SoundControlAudio> ();
            audioGO.SendMessage ("StartAudio", clip);
            audioGO.SendMessage ("SetVolume", Volume);
            audioGO.transform.parent = parent;
            audioGO.transform.position = parent.position;
            audioGO.SendMessage ("SetMode", mode);
            return audioGO;
        }
     
          
        /*!
        Upon loading the "audioDIR" <a href="http://docs.unity3d.com/Documentation/ScriptReference/AudioClip.html">AudioClip</a> by full pathname (e.g. "charactersounds/hit.mp3")
        from the <a href="http://docs.unity3d.com/Documentation/Manual/LoadingResourcesatRuntime.html">Resources folder</a>, this overload of
        the PlayOneShot method creates a new gameObject and attaches a SoundControlAudio component to it, sending it the "StartAudio" message
        and the AudioClip to play, the "SetVolume" message and the target volume, the "SetMode" message and the requested <a href="http://docs.unity3d.com/Documentation/ScriptReference/AudioRolloffMode.html">AudioRolloffMode</a>
        to use during playback, the "SetMinDistance" message and the MinDistance parameter, and finally the "SetMaxDistance" message and the
        MaxDitance parameter.
        
        The new gameObject is made a child of the passed-in "parent" transform, relocated to the same position as the parent, and then returned
        to the caller. If the requested clip is not configured as a 3D AudioClip in its <a href="http://docs.unity3d.com/Documentation/Components/class-AudioClip.html">import settings</a>,
        this relocation of the created gameObject will have no effect on the playback, since the clip will be effectively acting as a 2D sound.
        
        \param audioDIR The name of the <a href="http://docs.unity3d.com/Documentation/ScriptReference/AudioClip.html">AudioClip</a> to load from the <a href="http://docs.unity3d.com/Documentation/Manual/LoadingResourcesatRuntime.html">Resources folder</a>.
        \param Volume The volume at which the "audioDIR" <a href="http://docs.unity3d.com/Documentation/ScriptReference/AudioClip.html">AudioClip</a> should be played.
        \param parent The transform that will act as parent to the gameObject created to play the requested <a href="http://docs.unity3d.com/Documentation/ScriptReference/AudioClip.html">clip</a>.
        \param mode The <a href="http://docs.unity3d.com/Documentation/ScriptReference/AudioRolloffMode.html">AudioRolloffMode</a> to use during playback of the "audioDIR" <a href="http://docs.unity3d.com/Documentation/ScriptReference/AudioClip.html">AudioClip</a>.
        \param MinDistance The distance at which the created <a href="http://docs.unity3d.com/Documentation/ScriptReference/AudioSource.html">AudioSource</a> in the returned gameObject will cease to grow louder when approaching it.
        \param MaxDistance The distance from the created <a href="http://docs.unity3d.com/Documentation/ScriptReference/AudioSource.html">AudioSource</a> in the returned gameObject at which the requested <a href="http://docs.unity3d.com/Documentation/ScriptReference/AudioRolloffMode.html">AudioRolloffMode</a> will stop attenuating the sound.
        
        \retval GameObject The new gameObject with the SoundControlAudio component attached to it.
        */
        public static GameObject PlayOneShot (string audioDIR, float Volume, Transform parent, AudioRolloffMode mode, float MinDistance, float MaxDistance)
        {
            AudioClip clip = (AudioClip)Resources.Load (audioDIR);
            GameObject audioGO = new GameObject ();
            audioGO.AddComponent<SoundControlAudio> ();
            audioGO.SendMessage ("StartAudio", clip);
            audioGO.SendMessage ("SetVolume", Volume);
            audioGO.transform.parent = parent;
            audioGO.transform.position = parent.position;
            audioGO.SendMessage ("SetMode", mode);
            audioGO.SendMessage ("SetMinDistance", MinDistance);
            audioGO.SendMessage ("SetMaxDistance", MaxDistance);
            return audioGO;
        }
     
          
        /*!
        Upon loading the "audioDIR" <a href="http://docs.unity3d.com/Documentation/ScriptReference/AudioClip.html">AudioClip</a> by full pathname (e.g. "charactersounds/hit.mp3")
        from the <a href="http://docs.unity3d.com/Documentation/Manual/LoadingResourcesatRuntime.html">Resources folder</a>, this overload of
        the PlayOneShot method creates a new gameObject, sets the position of its transform to the requested location, and attaches a SoundControlAudio
        component to it, sending it the "StartAudio" message and the AudioClip to play, the "SetVolume" message and the target volume, the "SetMode"
        message and the requested <a href="http://docs.unity3d.com/Documentation/ScriptReference/AudioRolloffMode.html">AudioRolloffMode</a>
        to use during playback, the "SetMinDistance" message and the MinDistance parameter, and finally the "SetMaxDistance" message and the
        MaxDitance parameter.

        If the requested clip is not configured as a 3D AudioClip in its <a href="http://docs.unity3d.com/Documentation/Components/class-AudioClip.html">import settings</a>,
        changing the position at which it is played will have no effect on the playbak, since the clip will be effectively acting as a 2D sound.
        
        The new gameObject is returned to the caller.        
        
        \param audioDIR The name of the <a href="http://docs.unity3d.com/Documentation/ScriptReference/AudioClip.html">AudioClip</a> to load from the <a href="http://docs.unity3d.com/Documentation/Manual/LoadingResourcesatRuntime.html">Resources folder</a>.
        \param Volume The volume at which the "audioDIR" <a href="http://docs.unity3d.com/Documentation/ScriptReference/AudioClip.html">AudioClip</a> should be played.
        \param position The global position at which to play the "audioDIR" <a href="http://docs.unity3d.com/Documentation/ScriptReference/AudioClip.html">AudioClip</a>.
        \param mode The <a href="http://docs.unity3d.com/Documentation/ScriptReference/AudioRolloffMode.html">AudioRolloffMode</a> to use during playback of the "audioDIR" <a href="http://docs.unity3d.com/Documentation/ScriptReference/AudioClip.html">AudioClip</a>.
        \param MinDistance The distance at which the created <a href="http://docs.unity3d.com/Documentation/ScriptReference/AudioSource.html">AudioSource</a> in the returned gameObject will cease to grow louder when approaching it.
        \param MaxDistance The distance from the created <a href="http://docs.unity3d.com/Documentation/ScriptReference/AudioSource.html">AudioSource</a> in the returned gameObject at which the requested <a href="http://docs.unity3d.com/Documentation/ScriptReference/AudioRolloffMode.html">AudioRolloffMode</a> will stop attenuating the sound.
        
        \retval GameObject The new gameObject with the SoundControlAudio component attached to it.
        */
        public static GameObject PlayOneShot (string audioDIR, float Volume, Vector3 position, AudioRolloffMode mode, float MinDistance, float MaxDistance)
        {
            AudioClip clip = (AudioClip)Resources.Load (audioDIR);
            GameObject audioGO = new GameObject ();
            audioGO.AddComponent<SoundControlAudio> ();
            audioGO.SendMessage ("StartAudio", clip);
            audioGO.SendMessage ("SetVolume", Volume);
            audioGO.transform.position = position;
            audioGO.SendMessage ("SetMode", mode);
            audioGO.SendMessage ("SetMinDistance", MinDistance);
            audioGO.SendMessage ("SetMaxDistance", MaxDistance);
            return audioGO;
        }
     
          
        /*!
        Upon loading the "audioDIR" <a href="http://docs.unity3d.com/Documentation/ScriptReference/AudioClip.html">AudioClip</a> by full pathname (e.g. "charactersounds/hit.mp3")
        from the <a href="http://docs.unity3d.com/Documentation/Manual/LoadingResourcesatRuntime.html">Resources folder</a>, this overload of
        the PlayLoop method creates a new gameObject and attaches a SoundControlAudio component to it, sending it the "StartAudio" message
        and the AudioClip to play, along with the "SetLoops" message and the Loops parameter.
        
        The new gameObject is returned to the caller.
        
        \param audioDIR The name of the <a href="http://docs.unity3d.com/Documentation/ScriptReference/AudioClip.html">AudioClip</a> to load from the <a href="http://docs.unity3d.com/Documentation/Manual/LoadingResourcesatRuntime.html">Resources folder</a>.
        \param Loops The number of times to loop the "audioDIR" <a href="http://docs.unity3d.com/Documentation/ScriptReference/AudioClip.html">AudioClip</a>.
                
        \retval GameObject The new gameObject with the SoundControlAudio component attached to it.
        */
        public static GameObject PlayLoop (string audioDIR, int Loops)
        {
            AudioClip clip = (AudioClip)Resources.Load (audioDIR);
            GameObject audioGO = new GameObject ();
            audioGO.AddComponent<SoundControlAudio> ();
            audioGO.SendMessage ("StartAudio", clip);
            audioGO.SendMessage ("SetLoops", Loops);
            return audioGO;
        }
     
          
        /*!
        Upon loading the "audioDIR" <a href="http://docs.unity3d.com/Documentation/ScriptReference/AudioClip.html">AudioClip</a> by full pathname (e.g. "charactersounds/hit.mp3")
        from the <a href="http://docs.unity3d.com/Documentation/Manual/LoadingResourcesatRuntime.html">Resources folder</a>, this overload of
        the PlayLoop method creates a new gameObject and attaches a SoundControlAudio component to it, sending it the "StartAudio" message
        and the AudioClip to play, the "SetVolume" message and the target volume, and finally the "SetLoops" message and the Loops parameter.
        
        The new gameObject is returned to the caller.
        
        \param audioDIR The name of the <a href="http://docs.unity3d.com/Documentation/ScriptReference/AudioClip.html">AudioClip</a> to load from the <a href="http://docs.unity3d.com/Documentation/Manual/LoadingResourcesatRuntime.html">Resources folder</a>.
        \param Volume The volume at which the "audioDIR" <a href="http://docs.unity3d.com/Documentation/ScriptReference/AudioClip.html">AudioClip</a> should be played.
        \param Loops The number of times to loop the "audioDIR" <a href="http://docs.unity3d.com/Documentation/ScriptReference/AudioClip.html">AudioClip</a>.
                
        \retval GameObject The new gameObject with the SoundControlAudio component attached to it.
        */        
        public static GameObject PlayLoop (string audioDIR, float Volume, int Loops)
        {
            AudioClip clip = (AudioClip)Resources.Load (audioDIR);
            GameObject audioGO = new GameObject ();
            audioGO.AddComponent<SoundControlAudio> ();
            audioGO.SendMessage ("StartAudio", clip);
            audioGO.SendMessage ("SetVolume", Volume);
            audioGO.SendMessage ("SetLoops", Loops);
            return audioGO;
        }
     
          
        /*!
        Upon loading the "audioDIR" <a href="http://docs.unity3d.com/Documentation/ScriptReference/AudioClip.html">AudioClip</a> by full pathname (e.g. "charactersounds/hit.mp3")
        from the <a href="http://docs.unity3d.com/Documentation/Manual/LoadingResourcesatRuntime.html">Resources folder</a>, this overload of
        the PlayLoop method creates a new gameObject, sets the position of its transform to the requested location, and attaches a SoundControlAudio
        component to it, sending it the "StartAudio" message and the AudioClip to play, along with the "SetLoops" message and the Loops parameter.
        
        If the requested clip is not configured as a 3D AudioClip in its <a href="http://docs.unity3d.com/Documentation/Components/class-AudioClip.html">import settings</a>,
        changing the position at which it is played will have no effect on the playbak, since the clip will be effectively acting as a 2D sound.
        
        The new gameObject is returned to the caller.
        
        \param audioDIR The name of the <a href="http://docs.unity3d.com/Documentation/ScriptReference/AudioClip.html">AudioClip</a> to load from the <a href="http://docs.unity3d.com/Documentation/Manual/LoadingResourcesatRuntime.html">Resources folder</a>.
        \param position The global position at which to play the "audioDIR" <a href="http://docs.unity3d.com/Documentation/ScriptReference/AudioClip.html">AudioClip</a>.        
        \param Loops The number of times to loop the "audioDIR" <a href="http://docs.unity3d.com/Documentation/ScriptReference/AudioClip.html">AudioClip</a>.
                
        \retval GameObject The new gameObject with the SoundControlAudio component attached to it.
        */        
        public static GameObject PlayLoop (string audioDIR, Vector3 position, int Loops)
        {
            AudioClip clip = (AudioClip)Resources.Load (audioDIR);
            GameObject audioGO = new GameObject ();
            audioGO.AddComponent<SoundControlAudio> ();
            audioGO.SendMessage ("StartAudio", clip);
            audioGO.SendMessage ("SetLoops", Loops);
            audioGO.transform.position = position;
            return audioGO;
        }
     
          
        /*!
        Upon loading the "audioDIR" <a href="http://docs.unity3d.com/Documentation/ScriptReference/AudioClip.html">AudioClip</a> by full pathname (e.g. "charactersounds/hit.mp3")
        from the <a href="http://docs.unity3d.com/Documentation/Manual/LoadingResourcesatRuntime.html">Resources folder</a>, this overload of
        the PlayLoop method creates a new gameObject, sets the position of its transform to the requested location, and attaches a SoundControlAudio
        component to it, sending it the "StartAudio" message and the AudioClip to play, the "SetVolume" message and the target volume, and finally the
        "SetLoops" message and the Loops parameter.
        
        If the requested clip is not configured as a 3D AudioClip in its <a href="http://docs.unity3d.com/Documentation/Components/class-AudioClip.html">import settings</a>,
        changing the position at which it is played will have no effect on the playbak, since the clip will be effectively acting as a 2D sound.
        
        The new gameObject is returned to the caller.
        
        \param audioDIR The name of the <a href="http://docs.unity3d.com/Documentation/ScriptReference/AudioClip.html">AudioClip</a> to load from the <a href="http://docs.unity3d.com/Documentation/Manual/LoadingResourcesatRuntime.html">Resources folder</a>.
        \param Volume The volume at which the "audioDIR" <a href="http://docs.unity3d.com/Documentation/ScriptReference/AudioClip.html">AudioClip</a> should be played.
        \param position The global position at which to play the "audioDIR" <a href="http://docs.unity3d.com/Documentation/ScriptReference/AudioClip.html">AudioClip</a>.        
        \param Loops The number of times to loop the "audioDIR" <a href="http://docs.unity3d.com/Documentation/ScriptReference/AudioClip.html">AudioClip</a>.
                
        \retval GameObject The new gameObject with the SoundControlAudio component attached to it.        
        */        
        public static GameObject PlayLoop (string audioDIR, float Volume, Vector3 position, int Loops)
        {
            AudioClip clip = (AudioClip)Resources.Load (audioDIR);
            GameObject audioGO = new GameObject ();
            audioGO.AddComponent<SoundControlAudio> ();
            audioGO.SendMessage ("StartAudio", clip);
            audioGO.SendMessage ("SetVolume", Volume);
            audioGO.SendMessage ("SetLoops", Loops);
            audioGO.transform.position = position;
            return audioGO;
        }

          
        //   public static GameObject PlayOneShot(this AudioClip clip)          
        /*!
        Referencing the <a href="http://docs.unity3d.com/Documentation/ScriptReference/AudioClip.html">AudioClip</a> asset directly, this overload of the
        PlayOneShot method creates a new gameObject and attaches a SoundControlAudio component to it, sending it the "StartAudio" message and the AudioClip
        to play.
        
        The new gameObject is returned to the caller.
        
        \param clip The <a href="http://docs.unity3d.com/Documentation/ScriptReference/AudioClip.html">AudioClip</a> asset to play.
        
        \retval GameObject The new gameObject with the SoundControlAudio component attached to it.        
        */
        public static GameObject PlayOneShot (AudioClip clip)
        {
            GameObject audioGO = new GameObject ();
            audioGO.AddComponent<SoundControlAudio> ();
            audioGO.SendMessage ("StartAudio", clip);
            return audioGO;
        }
     
     
        //   public static GameObject PlayOneShot(this AudioClip clip,float Volume)          
        /*!
        Referencing the <a href="http://docs.unity3d.com/Documentation/ScriptReference/AudioClip.html">AudioClip</a> asset directly, this overload of the
        PlayOneShot method creates a new gameObject and attaches a SoundControlAudio component to it, sending it the "StartAudio" message and the AudioClip
        to play, along with the "SetVolume" message and the target volume.
        
        The new gameObject is returned to the caller.
        
        \param clip The <a href="http://docs.unity3d.com/Documentation/ScriptReference/AudioClip.html">AudioClip</a> asset to play.
        \param Volume The volume at which the <a href="http://docs.unity3d.com/Documentation/ScriptReference/AudioClip.html">clip</a> should be played.
        
        \retval GameObject The new gameObject with the SoundControlAudio component attached to it.        
        */        
        public static GameObject PlayOneShot (AudioClip clip, float Volume)
        {
            GameObject audioGO = new GameObject ();
            audioGO.AddComponent<SoundControlAudio> ();
            audioGO.SendMessage ("StartAudio", clip);
            audioGO.SendMessage ("SetVolume", Volume);
            return audioGO;
        }
         
         
        //   public static GameObject PlayOneShot(this AudioClip clip,Vector3 position)          
        /*!
        Referencing the <a href="http://docs.unity3d.com/Documentation/ScriptReference/AudioClip.html">AudioClip</a> asset directly, this overload of the
        PlayOneShot method creates a new gameObject, sets the position of its transform to the requested location, and attaches a SoundControlAudio component
        to it, sending it the "StartAudio" message and the AudioClip to play.
        
        If the requested clip is not configured as a 3D AudioClip in its <a href="http://docs.unity3d.com/Documentation/Components/class-AudioClip.html">import settings</a>,
        changing the position at which it is played will have no effect on the playbak, since the clip will be effectively acting as a 2D sound.
        
        The new gameObject is returned to the caller.
        
        \param clip The <a href="http://docs.unity3d.com/Documentation/ScriptReference/AudioClip.html">AudioClip</a> asset referenced from the Unity inspector.
        \param position The global position at which to play the <a href="http://docs.unity3d.com/Documentation/ScriptReference/AudioClip.html">clip</a>.
        
        \retval GameObject The new gameObject with the SoundControlAudio component attached to it.        
        */        
        public static GameObject PlayOneShot (AudioClip clip, Vector3 position)
        {
            GameObject audioGO = new GameObject ();
            audioGO.AddComponent<SoundControlAudio> ();
            audioGO.SendMessage ("StartAudio", clip);
            audioGO.transform.position = position;
            return audioGO;
        }
             
             
        //   public static GameObject PlayOneShot(this AudioClip clip,float Volume,Vector3 position)     
        /*!
        Referencing the <a href="http://docs.unity3d.com/Documentation/ScriptReference/AudioClip.html">AudioClip</a> asset directly, this overload of the
        PlayOneShot method creates a new gameObject, sets the position of its transform to the requested location, and attaches a SoundControlAudio component
        to it, sending it the "StartAudio" message and the AudioClip to play, along with the "SetVolume" message and the target volume.
        
        If the requested clip is not configured as a 3D AudioClip in its <a href="http://docs.unity3d.com/Documentation/Components/class-AudioClip.html">import settings</a>,
        changing the position at which it is played will have no effect on the playbak, since the clip will be effectively acting as a 2D sound.
                
        The new gameObject is returned to the caller.
        
        \param clip The <a href="http://docs.unity3d.com/Documentation/ScriptReference/AudioClip.html">AudioClip</a> asset to play.
        \param Volume The volume at which the <a href="http://docs.unity3d.com/Documentation/ScriptReference/AudioClip.html">clip</a> should be played.
        \param position The global position at which to play the <a href="http://docs.unity3d.com/Documentation/ScriptReference/AudioClip.html">clip</a>.
        
        \retval GameObject The new gameObject with the SoundControlAudio component attached to it.        
        */        
        public static GameObject PlayOneShot (AudioClip clip, float Volume, Vector3 position)
        {
            GameObject audioGO = new GameObject ();
            audioGO.AddComponent<SoundControlAudio> ();
            audioGO.SendMessage ("StartAudio", clip);
            audioGO.SendMessage ("SetVolume", Volume);
            audioGO.transform.position = position;
            return audioGO;
        }
     
     
        //   public static GameObject PlayOneShot(this AudioClip clip,float Volume,Transform parent,AudioRolloffMode mode)          
        /*!
        Referencing the <a href="http://docs.unity3d.com/Documentation/ScriptReference/AudioClip.html">AudioClip</a> asset directly, this overload of the
        PlayOneShot method creates a new gameObject and attaches a SoundControlAudio component to it, sending it the "StartAudio" message and the AudioClip
        to play, the "SetVolume" message and the target volume, and finally the "SetMode" message and the requested <a href="http://docs.unity3d.com/Documentation/ScriptReference/AudioRolloffMode.html">AudioRolloffMode</a>
        to use during playback.
        
        The new gameObject is made a child of the passed-in "parent" transform, relocated to the same position as the parent, and then returned
        to the caller. If the requested clip is not configured as a 3D AudioClip in its <a href="http://docs.unity3d.com/Documentation/Components/class-AudioClip.html">import settings</a>,
        this relocation of the created gameObject will have no effect on the playback, since the clip will be effectively acting as a 2D sound.
        
        \param clip The <a href="http://docs.unity3d.com/Documentation/ScriptReference/AudioClip.html">AudioClip</a> asset to play.
        \param Volume The volume at which the <a href="http://docs.unity3d.com/Documentation/ScriptReference/AudioClip.html">clip</a> should be played.
        \param parent The transform that will act as parent to the gameObject created to play the requested <a href="http://docs.unity3d.com/Documentation/ScriptReference/AudioClip.html">clip</a>.
        \param mode The <a href="http://docs.unity3d.com/Documentation/ScriptReference/AudioRolloffMode.html">AudioRolloffMode</a> to use during playback of the <a href="http://docs.unity3d.com/Documentation/ScriptReference/AudioClip.html">clip</a>.
        
        \retval GameObject The new gameObject with the SoundControlAudio component attached to it.        
        */        
        public static GameObject PlayOneShot (AudioClip clip, float Volume, Transform parent, AudioRolloffMode mode)
        {
            GameObject audioGO = new GameObject ();
            audioGO.AddComponent<SoundControlAudio> ();
            audioGO.SendMessage ("StartAudio", clip);
            audioGO.SendMessage ("SetVolume", Volume);
            audioGO.transform.parent = parent;
            audioGO.transform.position = parent.position;
            audioGO.SendMessage ("SetMode", mode);
            return audioGO;
        }
    
      
        //   public static GameObject PlayOneShot(this AudioClip clip,float Volume,Vector3 position,AudioRolloffMode mode)          
        /*!
        Referencing the <a href="http://docs.unity3d.com/Documentation/ScriptReference/AudioClip.html">AudioClip</a> asset directly, this overload of the
        PlayOneShot method creates a new gameObject, sets the position of its transform to the requested location, and attaches a SoundControlAudio component
        to it, sending it the "StartAudio" message and the AudioClip to play, the "SetVolume" message and the target volume, and finally the "SetMode" message
        and the requested <a href="http://docs.unity3d.com/Documentation/ScriptReference/AudioRolloffMode.html">AudioRolloffMode</a> to use during playback.
        
        If the requested clip is not configured as a 3D AudioClip in its <a href="http://docs.unity3d.com/Documentation/Components/class-AudioClip.html">import settings</a>,
        changing the position at which it is played will have no effect on the playbak, since the clip will be effectively acting as a 2D sound.
        
        The new gameObject is returned to the caller.
        
        \param clip The <a href="http://docs.unity3d.com/Documentation/ScriptReference/AudioClip.html">AudioClip</a> asset to play.
        \param Volume The volume at which the <a href="http://docs.unity3d.com/Documentation/ScriptReference/AudioClip.html">clip</a> should be played.
        \param position The global position at which to play the <a href="http://docs.unity3d.com/Documentation/ScriptReference/AudioClip.html">clip</a>.
        \param mode The <a href="http://docs.unity3d.com/Documentation/ScriptReference/AudioRolloffMode.html">AudioRolloffMode</a> to use during playback of the <a href="http://docs.unity3d.com/Documentation/ScriptReference/AudioClip.html">clip</a>.
        
        \retval GameObject The new gameObject with the SoundControlAudio component attached to it.        
        */        
        public static GameObject PlayOneShot (AudioClip clip, float Volume, Vector3 position, AudioRolloffMode mode)
        {
            GameObject audioGO = new GameObject ();
            audioGO.AddComponent<SoundControlAudio> ();
            audioGO.SendMessage ("StartAudio", clip);
            audioGO.SendMessage ("SetVolume", Volume);
            audioGO.transform.position = position;
            audioGO.SendMessage ("SetMode", mode);
            return audioGO;
        }
    
    
        //   public static GameObject PlayOneShot(this AudioClip clip,float Volume,Transform parent,AudioRolloffMode mode,float MinDistance,float MaxDistance)          
        /*!
        Referencing the <a href="http://docs.unity3d.com/Documentation/ScriptReference/AudioClip.html">AudioClip</a> asset directly, this overload of the
        PlayOneShot method creates a new gameObject and attaches a SoundControlAudio component to it, sending it the "StartAudio" message and the AudioClip
        to play, the "SetVolume" message and the target volume, the "SetMode" message and the requested <a href="http://docs.unity3d.com/Documentation/ScriptReference/AudioRolloffMode.html">AudioRolloffMode</a>
        to use during playback, the "SetMinDistance" message and the MinDistance parameter, and finally the "SetMaxDistance" message and the MaxDitance parameter.
        
        The new gameObject is made a child of the passed-in "parent" transform, relocated to the same position as the parent, and then returned
        to the caller. If the requested clip is not configured as a 3D AudioClip in its <a href="http://docs.unity3d.com/Documentation/Components/class-AudioClip.html">import settings</a>,
        this relocation of the created gameObject will have no effect on the playback, since the clip will be effectively acting as a 2D sound.
                
        \param clip The <a href="http://docs.unity3d.com/Documentation/ScriptReference/AudioClip.html">AudioClip</a> asset to play.
        \param Volume The volume at which the <a href="http://docs.unity3d.com/Documentation/ScriptReference/AudioClip.html">clip</a> should be played.
        \param parent The transform that will act as parent to the gameObject created to play the requested <a href="http://docs.unity3d.com/Documentation/ScriptReference/AudioClip.html">clip</a>.
        \param mode The <a href="http://docs.unity3d.com/Documentation/ScriptReference/AudioRolloffMode.html">AudioRolloffMode</a> to use during playback of the <a href="http://docs.unity3d.com/Documentation/ScriptReference/AudioClip.html">clip</a>.
        \param MinDistance The distance at which the created <a href="http://docs.unity3d.com/Documentation/ScriptReference/AudioSource.html">AudioSource</a> in the returned gameObject will cease to grow louder when approaching it.
        \param MaxDistance The distance from the created <a href="http://docs.unity3d.com/Documentation/ScriptReference/AudioSource.html">AudioSource</a> in the returned gameObject at which the requested <a href="http://docs.unity3d.com/Documentation/ScriptReference/AudioRolloffMode.html">AudioRolloffMode</a> will stop attenuating the sound.
        
        \retval GameObject The new gameObject with the SoundControlAudio component attached to it.        
        */        
        public static GameObject PlayOneShot (AudioClip clip, float Volume, Transform parent, AudioRolloffMode mode, float MinDistance, float MaxDistance)
        {
            GameObject audioGO = new GameObject ();
            audioGO.AddComponent<SoundControlAudio> ();
            audioGO.SendMessage ("StartAudio", clip);
            audioGO.SendMessage ("SetVolume", Volume);
            audioGO.transform.parent = parent;
            audioGO.transform.position = parent.position;
            audioGO.SendMessage ("SetMode", mode);
            audioGO.SendMessage ("SetMinDistance", MinDistance);
            audioGO.SendMessage ("SetMaxDistance", MaxDistance);
            return audioGO;
        }
     
     
        //   public static GameObject PlayOneShot(this AudioClip clip,float Volume,Vector3 position,AudioRolloffMode mode,float MinDistance,float MaxDistance)          
        /*!
        Referencing the <a href="http://docs.unity3d.com/Documentation/ScriptReference/AudioClip.html">AudioClip</a> asset directly, this overload of the
        PlayOneShot method creates a new gameObject, sets the position of its transform to the requested location, and attaches a SoundControlAudio component
        to it, sending it the "StartAudio" message and the AudioClip to play, the "SetVolume" message and the target volume, the "SetMode" message and the
        requested <a href="http://docs.unity3d.com/Documentation/ScriptReference/AudioRolloffMode.html">AudioRolloffMode</a> to use during playback, the
        "SetMinDistance" message and the MinDistance parameter, and finally the "SetMaxDistance" message and the MaxDitance parameter.
        
        If the requested clip is not configured as a 3D AudioClip in its <a href="http://docs.unity3d.com/Documentation/Components/class-AudioClip.html">import settings</a>,
        changing the position at which it is played will have no effect on the playbak, since the clip will be effectively acting as a 2D sound.
                
        The new gameObject is returned to the caller.
        
        \param clip The <a href="http://docs.unity3d.com/Documentation/ScriptReference/AudioClip.html">AudioClip</a> asset to play.
        \param Volume The volume at which the <a href="http://docs.unity3d.com/Documentation/ScriptReference/AudioClip.html">clip</a> should be played.
        \param position The global position at which to play the <a href="http://docs.unity3d.com/Documentation/ScriptReference/AudioClip.html">clip</a>.
        \param mode The <a href="http://docs.unity3d.com/Documentation/ScriptReference/AudioRolloffMode.html">AudioRolloffMode</a> to use during playback of the <a href="http://docs.unity3d.com/Documentation/ScriptReference/AudioClip.html">clip</a>.
        \param MinDistance The distance at which the created <a href="http://docs.unity3d.com/Documentation/ScriptReference/AudioSource.html">AudioSource</a> in the returned gameObject will cease to grow louder when approaching it.
        \param MaxDistance The distance from the created <a href="http://docs.unity3d.com/Documentation/ScriptReference/AudioSource.html">AudioSource</a> in the returned gameObject at which the requested <a href="http://docs.unity3d.com/Documentation/ScriptReference/AudioRolloffMode.html">AudioRolloffMode</a> will stop attenuating the sound.
        
        \retval GameObject The new gameObject with the SoundControlAudio component attached to it.        
        */        
        public static GameObject PlayOneShot (AudioClip clip, float Volume, Vector3 position, AudioRolloffMode mode, float MinDistance, float MaxDistance)
        {
            GameObject audioGO = new GameObject ();
            audioGO.AddComponent<SoundControlAudio> ();
            audioGO.SendMessage ("StartAudio", clip);
            audioGO.SendMessage ("SetVolume", Volume);
            audioGO.transform.position = position;
            audioGO.SendMessage ("SetMode", mode);
            audioGO.SendMessage ("SetMinDistance", MinDistance);
            audioGO.SendMessage ("SetMaxDistance", MaxDistance);
            return audioGO;
        }
     
                     
        //   public static void PlayLoop(this AudioClip clip,int Loops)          
        /*!
        Referencing the <a href="http://docs.unity3d.com/Documentation/ScriptReference/AudioClip.html">AudioClip</a> asset directly, this overload of the
        PlayLoop method creates a new gameObject and attaches a SoundControlAudio component to it, sending it the "StartAudio" message and the AudioClip
        to play, along with the "SetLoops" message and the Loops parameter.
        
        The new gameObject is returned to the caller.
        
        \param clip The <a href="http://docs.unity3d.com/Documentation/ScriptReference/AudioClip.html">AudioClip</a> asset to play.        
        \param Loops The number of times to loop the <a href="http://docs.unity3d.com/Documentation/ScriptReference/AudioClip.html">clip</a>.
        */        
        public static void PlayLoop (AudioClip clip, int Loops)
        {
            GameObject audioGO = new GameObject ();
            audioGO.AddComponent<SoundControlAudio> ();
            audioGO.SendMessage ("StartAudio", clip);
            audioGO.SendMessage ("SetLoops", Loops);
        }
             
             
        //   public static GameObject PlayLoop(this AudioClip clip,float Volume,int Loops)          
        /*!
        Referencing the <a href="http://docs.unity3d.com/Documentation/ScriptReference/AudioClip.html">AudioClip</a> asset directly, this overload of the
        PlayLoop method creates a new gameObject and attaches a SoundControlAudio component to it, sending it the "StartAudio" message and the AudioClip
        to play, the "SetVolume" message and the target volume, and finally the "SetLoops" message and the Loops parameter.
        
        The new gameObject is returned to the caller.
        
        \param clip The <a href="http://docs.unity3d.com/Documentation/ScriptReference/AudioClip.html">AudioClip</a> asset to play.
        \param Volume The volume at which the <a href="http://docs.unity3d.com/Documentation/ScriptReference/AudioClip.html">clip</a> should be played.
        \param Loops The number of times to loop the <a href="http://docs.unity3d.com/Documentation/ScriptReference/AudioClip.html">clip</a>.
        
        \retval GameObject The new gameObject with the SoundControlAudio component attached to it.        
        */        
        public static GameObject PlayLoop (AudioClip clip, float Volume, int Loops)
        {
            GameObject audioGO = new GameObject ();
            audioGO.AddComponent<SoundControlAudio> ();
            audioGO.SendMessage ("StartAudio", clip);
            audioGO.SendMessage ("SetVolume", Volume);
            audioGO.SendMessage ("SetLoops", Loops);
            return audioGO;
        }
             
             
        //   public static GameObject PlayLoop(this AudioClip clip,Vector3 position,int Loops)          
        /*!
        Referencing the <a href="http://docs.unity3d.com/Documentation/ScriptReference/AudioClip.html">AudioClip</a> asset directly, this overload of the
        PlayLoop method creates a new gameObject, sets the position of its transform to the requested location, and attaches a SoundControlAudio
        component to it, sending it the "StartAudio" message and the AudioClip to play, along with the "SetLoops" message and the Loops parameter.
        
        If the requested clip is not configured as a 3D AudioClip in its <a href="http://docs.unity3d.com/Documentation/Components/class-AudioClip.html">import settings</a>,
        changing the position at which it is played will have no effect on the playbak, since the clip will be effectively acting as a 2D sound.
        
        The new gameObject is returned to the caller.
        
        \param clip The <a href="http://docs.unity3d.com/Documentation/ScriptReference/AudioClip.html">AudioClip</a> asset to play.
        \param position The global position at which to play the <a href="http://docs.unity3d.com/Documentation/ScriptReference/AudioClip.html">clip</a>.
        \param Loops The number of times to loop the <a href="http://docs.unity3d.com/Documentation/ScriptReference/AudioClip.html">clip</a>.
        
        \retval GameObject The new gameObject with the SoundControlAudio component attached to it.        
        */
        public static GameObject PlayLoop (AudioClip clip, Vector3 position, int Loops)
        {
            GameObject audioGO = new GameObject ();
            audioGO.AddComponent<SoundControlAudio> ();
            audioGO.SendMessage ("StartAudio", clip);
            audioGO.SendMessage ("SetLoops", Loops);
            audioGO.transform.position = position;
            return audioGO;
        }
     
     
        //   public static GameObject PlayLoop(this AudioClip clip,float Volume,Vector3 position,int Loops)          
        /*!
        Referencing the <a href="http://docs.unity3d.com/Documentation/ScriptReference/AudioClip.html">AudioClip</a> asset directly, this overload of the
        PlayLoop method creates a new gameObject, sets the position of its transform to the requested location, and attaches a SoundControlAudio
        component to it, sending it the "StartAudio" message and the AudioClip to play, the "SetVolume" message and the target volume, and finally the
        "SetLoops" message and the Loops parameter.
        
        If the requested clip is not configured as a 3D AudioClip in its <a href="http://docs.unity3d.com/Documentation/Components/class-AudioClip.html">import settings</a>,
        changing the position at which it is played will have no effect on the playbak, since the clip will be effectively acting as a 2D sound.
        
        The new gameObject is returned to the caller.
        
        \param clip The <a href="http://docs.unity3d.com/Documentation/ScriptReference/AudioClip.html">AudioClip</a> asset to play.
        \param Volume The volume at which the <a href="http://docs.unity3d.com/Documentation/ScriptReference/AudioClip.html">clip</a> should be played.
        \param position The global position at which to play the <a href="http://docs.unity3d.com/Documentation/ScriptReference/AudioClip.html">clip</a>.
        \param Loops The number of times to loop the <a href="http://docs.unity3d.com/Documentation/ScriptReference/AudioClip.html">clip</a>.
        
        \retval GameObject The new gameObject with the SoundControlAudio component attached to it.        
        */
        public static GameObject PlayLoop (AudioClip clip, float Volume, Vector3 position, int Loops)
        {
            GameObject audioGO = new GameObject ();
            audioGO.AddComponent<SoundControlAudio> ();
            audioGO.SendMessage ("StartAudio", clip);
            audioGO.SendMessage ("SetVolume", Volume);
            audioGO.SendMessage ("SetLoops", Loops);
            audioGO.transform.position = position;
            return audioGO;
        }
    }
}