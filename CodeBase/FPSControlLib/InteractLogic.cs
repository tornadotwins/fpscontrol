/*
jmpp: In progress.
*/
using UnityEngine;
using System.Collections;

//---
//
//---
namespace FPSControl
{
    //! In progress...
    /*!
    */
    public class InteractLogic : MonoBehaviour
    {
        /*!
        Determines whether interaction with this componet can happen only once or multiple times, defautling to only once.
        */
        public bool          _oneTime = true;
        /*!
        Determines whether interaction with this component is possible or not, defaulting to yes.
        */
        public bool          _canInteractWith = true;
        public string        _normalInteraction;
        public GUIText       _interactGUIText;
        public string        _interactText;
        public float     _interactWidth;
        public Color     _interactTextColor;
        public Color     _highlightColor;
        public Color     _nonhighlightColor;
        public float     _fadeTime = 1F;
        public Renderer[]    _highlightParts;
        private bool     _doneInteraction = false;
        private float        _fadeTimer = 0F;
     
     
        //---
        //
        //---
        public void Update ()
        {
            if ((_fadeTimer > 0F) && (_fadeTimer < Time.time)) {
                NonHighlighted ();
            }
        }
     
     
        //---
        //
        //---
        public void ReceivedInteraction (bool zone)
        {
            //this should be the new paradigm
            InteractiveObject iObj = GetComponent<InteractiveObject>();
            iObj.Interact();
 
            SendMessage (_normalInteraction); //support legacy
            _doneInteraction = true;
         
            if (_oneTime) {
                _canInteractWith = false;
            }
        }
     
     
        //---
        //
        //---
        public void FinishedInteraction ()
        {
            //Debug.Log ("finished interact");
            if (_doneInteraction) {
                _doneInteraction = false;
            }
        }
     
     
  
        /*!
        Sets the #_canInteractWith variable to false, so that interactions with this component are no longer possible.
        */      
        public void StopInteraction ()
        {
            _canInteractWith = false;
        }
        
     
        public void Highlighted ()
        {
            if (! _canInteractWith)
                return;
         
            _fadeTimer = Time.time + _fadeTime;
         
            foreach (Renderer rend in _highlightParts) {
                rend.material.SetFloat ("_Outline", _interactWidth);
                rend.material.SetColor ("_OutlineColor", _highlightColor);
            }
 
            if (_interactGUIText != null) {
                _interactGUIText.text = _interactText;
                _interactGUIText.font.material.color = _interactTextColor;
            }
         
        }
     
        public void NonHighlighted ()
        {
            _fadeTimer = 0F;
         
            if (_interactGUIText != null) {
                _interactGUIText.text = "";
                _interactGUIText.font.material.color = _interactTextColor;
            }
         
            foreach (Renderer rend in _highlightParts) {
                rend.material.SetFloat ("_Outline", _interactWidth);
                rend.material.SetColor ("_OutlineColor", _nonhighlightColor);
            }
        }
    }
}