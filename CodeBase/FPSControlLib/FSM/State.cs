using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using FPSControl;


namespace FPSControl.States
{
    public abstract class State : object
    {
        public StateMachine Parent { get; protected set; }
        public string name;
        //public string Name { get { return _name; } }
        public bool isDefault { get { return Parent.defaultState == this; } }

        public State(string name, StateMachine parent)
        {
            this.name = name;
            Parent = parent;
        }

        public void Rename(string newName)
        {
            name = newName;
        }

        protected virtual void Initialize() { }
        protected virtual void Deinitialize() { }

        public static implicit operator bool(State s)
        {
            return s != null;
        }
    }
}