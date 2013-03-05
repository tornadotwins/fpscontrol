/*
jmpp: Done!
*/
using UnityEngine;
using System.Collections;


namespace FPSControl
{
    //! Abstract interface for the application of damage to a gameObject.
    /*!
    The Damageable component provides an abstract and simple interface that allows programatic application of damage to the containing gameObject, based on the
    information provided by a DamageSource object.
    */
    public class Damageable : MonoBehaviour
    {
        /*!
        Initial health of the containing gameObject.
        */
        public float         _health = 5f;
        /*!
        Wether the containing gameObject should be hidden when its health value drops to or below zero (0).
        */
        public GameObject        _hideThisOnDeath;
        /*!
        GameObject holding an animation component from which a suitable death animation clip should be read and played.
        */
        public GameObject        _animator;
        /*!
        The name of the death animation.
        */
        public string            _deathAnimation;
        /*!
        The AudioSource that will play the death sound of the gameObject.
        */
        public AudioSource   _audioSource;
        /*!
        The death sound clip for the gameObject.
        */
        public AudioClip     _deathSound;
        /*!
        The name of a custom action that should be sent to the parent of the containing gameObject (itself if has no parent) upon
        its health value droping to or below zero (0).
        */
        public string            _deathAction;
 
 
        /*!
        Applies the damage determined by the \link DamageSource#damageAmount damageAmount\endlink parameter of the passed in DamageSource object, substracting it from
        the current #_health value whenever it is above zero (0).
        
        Once #_health drops below this threshold, damage is no longer applyed and the OnDeath() method is invoked.
        */
        void ApplyDamage (DamageSource damageSource)
        {
            if (_health > 0f) {
                _health -= damageSource.damageAmount;
             
                if (_health <= 0f)
                    OnDeath ();
            }
        }
 
     

        /*!
        Hides the containing gameObject and all of its children by deactivating them if the #_hideThisOnDeath parameter is true. Plays the
        #_deathSound audio clip "one shot" if it and #_audioSource are non-null, and activates the death animation clip if the #_animator gameObject
        has an animation component listing #_deathAnimation as one of its available clips.
        
        If #_deathAction is non-null, it is sent as a message to the parent gameObject of the one containing this Damageable component; if no such parent
        exists, the message is sent to the containing gameObject itself, i.e. this.gameObject.
        */
        void OnDeath ()
        {
            if (_hideThisOnDeath != null) {
                _hideThisOnDeath.SetActiveRecursively (false);
            }
 
            if ((_audioSource != null) && (_deathSound != null)) {
                _audioSource.PlayOneShot (_deathSound);
            }
         
            if (_animator.animation [_deathAnimation] != null) {
                _animator.animation.Play (_deathAnimation);
            }
         
            if (_deathAction != null) {
                GameObject parent = gameObject;
             
                if (transform.parent != null)
                    parent = transform.parent.gameObject;
             
                parent.SendMessage (_deathAction);
            }
        }
    }
}