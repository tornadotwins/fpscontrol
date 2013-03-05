using UnityEngine;
using System.Collections;
using FPSControl;
namespace FPSControl.States.Player
{
    [System.Serializable]
    public class PlayerState : State
    {
        public bool fallThroughState = false; //should this state take on the properties of the last?
        
        public float forwardSpeed;
        public float reverseSpeed;
        public float strafeSpeed;
        
        public bool canJump;
        public bool canCrouch;
        public bool canMove;
        public bool canInteract;
        public bool canUseWeapon;
        
        public PlayerState(string name, FPSControlPlayer player) : base(name, player){}

        protected override void Initialize()
        {
            base.Initialize();
        }

        protected override void Deinitialize()
        {
            base.Deinitialize();
        }

        public void SetFallthroughData(PlayerState lastState)
        {
            
            forwardSpeed = lastState.forwardSpeed;
            reverseSpeed = lastState.reverseSpeed;
            strafeSpeed = lastState.strafeSpeed;
        }
    }
}
