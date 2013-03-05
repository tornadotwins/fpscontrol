/*
jmpp: Done!
*/
using UnityEngine;
using System.Collections;



namespace FPSControl
{
    //! "Theme" music playback management for the "Player" character.
    /*!
    The Ambience component handles "theme" music playback for when the "Player" character either enters or exits the <a href="http://docs.unity3d.com/Documentation/ScriptReference/Collider-isTrigger.html">trigger collider</a>
    of the gameObject to which this component is attached, leveraging the MessengerControl message broadcasting facility.
    */
    public class Ambience : MonoBehaviour
    {
        /*!
        Name of the music track to play when the "Player" gameObject enters the <a href="http://docs.unity3d.com/Documentation/ScriptReference/Collider-isTrigger.html">trigger collider</a>
        of this gameObject.
        */
        public string        _enterMusicName;
        /*!
        Name of the music track to play when the "Player" gameObject exits the <a href="http://docs.unity3d.com/Documentation/ScriptReference/Collider-isTrigger.html">trigger collider</a>
        of this gameObject.
        */
        public string        _exitMusicName;
     
        /*!
        Detects when a gameObject with tag "Player" enters the <a href="http://docs.unity3d.com/Documentation/ScriptReference/Collider-isTrigger.html">trigger collider</a>
        of whatever gameObject this component is attached to, and if the #_enterMusicName parameter is non-null it requests the MessengerControl to broadcast
        the "FadeIn" message, passing the music track name as argument to its listeners.
        
        It should be noted that this method is not to be executed manually; instead, it is automatically invoked by Unity through its private events & messaging
        facility.
        
        \param _other The "other" collider that just entered the <a href="http://docs.unity3d.com/Documentation/ScriptReference/Collider-isTrigger.html">trigger collider</a> of this gameObject.
        */
        void OnTriggerEnter (Collider _other)
        {
            if ((_enterMusicName != null) && (_other.tag.Equals ("Player"))) {
                MessengerControl.Broadcast ("FadeIn", _enterMusicName);
            }
        }
     
        /*!
        Detects when a gameObject with tag "Player" exits the <a href="http://docs.unity3d.com/Documentation/ScriptReference/Collider-isTrigger.html">trigger collider</a>
        of whatever gameObject this component is attached to, and if the #_enterMusicName parameter is non-null it requests the MessengerControl to broadcast
        the "FadeOut" message, passing a boolean value of true as argument to its listeners. Additionally, if the #_exitMusicName parameter is non-null, this
        method requests the MessengerControl to broadcast the "FadeIn" message, passing the music track name as argument to its listeners.
        
        It should be noted that this method is not to be executed manually; instead, it is automatically invoked by Unity through its private events & messaging
        facility.
        
        \param _other The "other" collider that just stopped touching the <a href="http://docs.unity3d.com/Documentation/ScriptReference/Collider-isTrigger.html">trigger collider</a> of this gameObject.
        */
        void OnTriggerExit (Collider _other)
        {
            if ((_enterMusicName != null) && (_other.tag.Equals ("Player"))) {
                MessengerControl.Broadcast ("FadeOut", true);
                if (_exitMusicName != null) {
                    MessengerControl.Broadcast ("FadeIn", _exitMusicName);
                }
            }
        }
    }
}