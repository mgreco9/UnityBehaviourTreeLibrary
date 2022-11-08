using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviourTreeLibrary
{
    public class BehaviourTreeRunner : MonoBehaviour
    {
        public BehaviourTree behaviourTree;

        protected virtual void Awake()
        {
            behaviourTree = behaviourTree.Clone();
        }
        protected virtual void Start()
        {
        }

        protected virtual void Update()
        {
            ResetInputs();
            behaviourTree.Update();
        }

        public virtual void ResetInputs()
        {

        }
    }
}
