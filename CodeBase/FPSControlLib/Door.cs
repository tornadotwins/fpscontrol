/*
jmpp: Done!
*/
using UnityEngine;
using System.Collections;


namespace FPSControl
{
    //! %Door activities event handling facility.
    /*!
    The Door component provides event handlers for common door activities (actions), like \link OnOpen() door opening\endlink.
    */
    public class Door : MonoBehaviour
    {
        /*!
        GameObject with an <a href="http://docs.unity3d.com/Documentation/ScriptReference/Animation.html">animation</a> component attached to it, to be used
        for playback of the various door animations that correspond to each door action.
        */
        public GameObject        _animator;
        /*!
        The name of the door opening animation.
        */
        public string            _openAnimation;
        /*!
        <a href="http://docs.unity3d.com/Documentation/ScriptReference/AudioSource.html">AudioSource</a> through which to play the <a href="http://docs.unity3d.com/Documentation/ScriptReference/AudioClip.html">AudioClips</a>
        that correspond to each door action.
        */
        public AudioSource       _audioSource;
        /*!
        The <a href="http://docs.unity3d.com/Documentation/ScriptReference/AudioClip.html">AudioClip</a> for the door opening action.
        */
        public AudioClip     _openSound;
 
 
        /*!
        Handles the "open" action for the door, playing the #_openSound <a href="http://docs.unity3d.com/Documentation/ScriptReference/AudioClip.html">AudioClip</a>
        if it and #_audioSource are non-null, and playing the #_openAnimation if the #_animator gameObject is also non-null and its <a href="http://docs.unity3d.com/Documentation/ScriptReference/Animation.html">animation</a>
        component provides such an animation clip.
        
        Adittionally, If any of the children of the gameObject this Door component is attached to hold an InteractLogic component, said components' \link InteractLogic#StopInteraction() StopInteraction()\endlink
        methods are invoked if their \link InteractLogic#_oneTime _oneTime\endlink properties are true.
        */
        void OnOpen ()
        {
            if ((_audioSource != null) && (_openSound != null)) {
                _audioSource.PlayOneShot (_openSound);
            }
         
            if (_animator != null) {
                if (_animator.GetComponent<Animation>()[_openAnimation] != null) {
                    _animator.GetComponent<Animation>().Play(_openAnimation);
                }
            }
         
            InteractLogic[] logics = GetComponentsInChildren<InteractLogic> ();
            foreach (InteractLogic logic in logics) {
                if (logic._oneTime) {
                    logic.StopInteraction ();
                }
            }
        }
    }
}
