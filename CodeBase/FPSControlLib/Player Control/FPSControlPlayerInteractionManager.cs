using UnityEngine;
using System.Collections;
using FPSControl;

namespace FPSControl
{
    /// <summary>
    /// this class will handle all of the interactions, like detecting if there is a door, or a weapon, etc.
    /// </summary>
    [RequireComponent(typeof(Camera))]
    public class FPSControlPlayerInteractionManager : FPSControlPlayerComponent
    {
        public KeyCode interactionKey = KeyCode.E;

        public bool allowInteracting = true;
        public float interactionMaxRange = 2.0F;
        bool _inInteractZone = false;
        InteractLogic _interactLogic;
        Transform _transform;
        Camera camera;

        void Awake()
        {
            camera = GetComponent<Camera>();
            _transform = transform;
        }

        public override void DoUpdate()
        {
            if (!FPSControlPlayerData.visible || FPSControlPlayerData.frozen) return;
            
            Ray ray = new Ray(_transform.position, _transform.forward);
            //Debug.DrawRay(ray.origin, ray.direction * interactionMaxRange, Color.cyan);
            //Debug.Log("raycasting");

            if (!allowInteracting)
            {
                if(_interactLogic)
                {
                    _interactLogic.NonHighlighted();
                    _interactLogic = null;
                }
                return;
            }

            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, interactionMaxRange, camera.cullingMask))
            {
                InteractLogic interactiveLogic = hit.collider.GetComponent<InteractLogic>();
                //Debug.Log(hit.collider.gameObject.name);
                if (interactiveLogic)
                {
                    //Debug.Log("interact logic");
                    
                    if (!_interactLogic || (_interactLogic == interactiveLogic))
                    {
                        _interactLogic = interactiveLogic;
                        _interactLogic.Highlighted();
                    }
                    else if (_interactLogic != interactiveLogic)
                    {
                        _interactLogic.NonHighlighted();
                        _interactLogic = interactiveLogic;
                        _interactLogic.Highlighted();
                    }

                }
                else
                {
                   // Debug.Log("No interact logic");
                    if (_interactLogic)
                    {
                        _interactLogic.NonHighlighted();
                        _interactLogic = null;
                    }
                }
            }
            else
            {
                if(_interactLogic)
                {
                    _interactLogic.NonHighlighted();
                    _interactLogic = null;
                }
            }

            if (FPSControlInput.IsInteracting() && _interactLogic)
            {
                //Debug.Log("interacted");
                _interactLogic.ReceivedInteraction(true);
            }
            //else if (Input.GetKeyDown(interactionKey)) Debug.Log("tried to interact, but nothing to interact with");


        }
    }
}
