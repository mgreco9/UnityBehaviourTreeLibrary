using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviourTreeLibrary
{
#if UNITY_EDITOR
    public enum DecoratorType
    {
        START_CONDITION,
        UPDATE,
        STOP_CONDITION,
        OUTPUT
    }
#endif

    public abstract class DecoratorProperty : ScriptableObject
    {
        [HideInInspector]public Node node;
        [HideInInspector]public bool updateView;

        public virtual void Load() { }
        public virtual void BeforeStart() { }
        public virtual void AfterStart() { }
        public virtual void BeforeUpdate() { }
        public virtual void AfterUpdate() { }
        public virtual void BeforeStop() { }
        public virtual void AfterStop() { }

        public DecoratorProperty Clone()
        {
            return Instantiate(this);
        }

#if UNITY_EDITOR
        public abstract DecoratorType Type { get; }
        
        [HideInInspector]
        public virtual string Description
        {
            get
            {
                return "";
            }
        }
        
        private void OnValidate()
        {
            updateView = true;
        }
#endif
    }
}