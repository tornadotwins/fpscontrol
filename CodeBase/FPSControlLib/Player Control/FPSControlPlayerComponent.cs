using UnityEngine;
using System.Collections;
using FPSControl;
using FPSControl.States;
using FPSControl.States.Player;

namespace FPSControl
{
    /// <summary>
    /// base for all the various components that work together under the FPSControlPlayer
    /// </summary>
    public abstract class FPSControlPlayerComponent : MonoBehaviour
    {
        [HideInInspector]
        public FPSControlPlayer Player { get; private set; }
        [HideInInspector]
        public bool Initialized { get; private set; }
        [HideInInspector]
        protected PlayerState state;

        public virtual void Initialize(FPSControlPlayer player)
        {
            if (Initialized)
            {
                Debug.LogWarning("Initialization of FPSControlPlayerComponent " + GetType() + " has already taken place.");
                return;
            }

            Player = player;
            Initialized = true;
            SetState(player.currentState);
            OnInitialize();
        }

        protected virtual void OnInitialize() { }

        public virtual void SetState(State state)
        {
            this.state = (PlayerState) state;
        }

        #region Loops

        /*
         * We break these out so that they can never independently execute, but rather, are driven by the FPSControlPlayer's loops. 
         */
        public virtual void DoUpdate() { }
        public virtual void DoFixedUpdate() { }
        public virtual void DoLateUpdate() { }

        #endregion // Loops
    }
}
