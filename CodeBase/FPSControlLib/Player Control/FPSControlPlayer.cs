using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using FPSControl.States;
using FPSControl.States.Player;

namespace FPSControl
{
    public class FPSControlPlayer : StateMachine
    {
        //Components
        public FPSControlPlayerCamera playerCamera;
        public FPSControlPlayerInteractionManager interactionManager;
        public FPSControlPlayerMovement movementController;
        public FPSControlPlayerWeaponManager weaponManager; 

        //Player States
        public PlayerState idleState;
        public PlayerState walkState;
        public PlayerState runState;
        public PlayerState jumpState;

        //player info
        public bool moving { get { return currentState != idleState; } } 
        public bool jumping { get { return movementController.isJumping; } }
        public bool crouching { get { return movementController.isCrouching; } }
        public bool strafing { get { return movementController.isStrafing; } }
        public bool reversing { get { return movementController.isMovingBackwards; } }
        public Vector3 velocity { get { return movementController.Controller.velocity; } }
        public Vector3 forward { get { return transform.forward; } }
        public Vector3 aim { get { return playerCamera.transform.forward; } } 

        //Weapon States - for a later date

        new public PlayerState currentState
        { 
            get 
            { 
                return (PlayerState)base.currentState; 
            } 
            protected set
            {
                PlayerState newState = value;
                if (newState.fallThroughState) newState.SetFallthroughData(this.currentState);
                base.currentState = (PlayerState)value;
 
            } 
        } //i need to rethink the polymorphism here i believe, but this works for now.

        void Awake()
        {
            //short cutting for demo purposes.
            Add(idleState);
            Add(walkState);
            Add(runState);
            Add(jumpState);
            
            //initialize our states here, and default our first
            Initialize(idleState);
        }

        void Start()
        {
            //Initialize our sub-components, we do this on Start instead of Awake to make sure that they have ample time to self-initialize if need be.
            playerCamera.Initialize(movementController,this);
            interactionManager.Initialize(this);
            movementController.Initialize(playerCamera, this);
            weaponManager.Initialize(playerCamera, movementController, this);
        }

        protected override void OnInitialize()
        {
            
        }

        protected override void OnStateChange()
        {
            playerCamera.SetState(currentState);
            interactionManager.SetState(currentState);
            movementController.SetState(currentState);
            weaponManager.SetState(currentState);
        }

        #region Loops

        void FixedUpdate()
        {
            
        }

        void Update()
        {
            playerCamera.DoUpdate();
            movementController.DoUpdate();
            interactionManager.allowInteracting = currentState.canInteract;
            interactionManager.DoUpdate();
            weaponManager.DoUpdate();
        }

        void LateUpdate()
        {
            playerCamera.DoLateUpdate();
            weaponManager.DoLateUpdate();
        }

        #endregion // Loops
    }
}
