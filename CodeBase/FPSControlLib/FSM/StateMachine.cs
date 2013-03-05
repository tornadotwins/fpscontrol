using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using FPSControl;

namespace FPSControl.States
{
    public abstract class StateMachine : MonoBehaviour, IEnumerable
    {
        List<State> _states;
        public State defaultState { get; private set; }
        public virtual State currentState { get; protected set; }
        bool _initialized = false;

        protected void Initialize(State defaultState)
        {
            if(_initialized)
            {
                Debug.LogWarning("StateMachine already initialized!");
            }
            _initialized = true;
            this.defaultState = defaultState;
            this.currentState = defaultState;
            OnInitialize();
        }

        protected abstract void OnInitialize();

        public void ChangeState(string stateName)
        {
            ChangeState(this[stateName]);
        }

        public void ChangeState(State state)
        {
            if (!_initialized) throw new System.Exception("StateMachine was never initialized!");
            currentState = state;
            OnStateChange();
        }

        protected abstract void OnStateChange();

        public IEnumerator GetEnumerator()
        {
            foreach (State s in _states)
            {
                yield return s;
            }
        }

        public State this[string stateName]
        {
            get
            {
                foreach (State s in _states)
                {
                    if (s.name == stateName) return s;
                }

                return null;
            }
        }

        public void Add(State state)
        {
            if (_states == null) _states = new List<State>();
            _states.Add(state);
        }

        public void Remove(State state)
        {
            _states.Remove(state);
        }
    }
}