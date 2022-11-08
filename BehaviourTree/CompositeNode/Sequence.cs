using System.Collections.Generic;
using UnityEngine;

namespace BehaviourTreeLibrary
{
    public class Sequence : CompositeNode
    {
        protected int currentChildNodeIdx;

        public override void OnStart()
        {
            children.ForEach(c => c.state = NodeState.NONE);
            currentChildNodeIdx = 0;
        }

        public override NodeState OnUpdate()
        {
            // 1 - Iterate over each child node (one node must succeed)
            state = children[currentChildNodeIdx].Evaluate();

            // 2 - If child node has failed, return failure
            if (state == NodeState.FAILURE)
                return NodeState.FAILURE;

            // 3 - If child node has succeeded, iterate over next node
            if (state == NodeState.SUCCESS)
                currentChildNodeIdx++;

            // 4 - If every node have been realized, return success
            if (currentChildNodeIdx >= children.Count)
                return NodeState.SUCCESS;

            // 5 - If still iterating over nodes, return running
            return NodeState.RUNNING;
        }

        public override void OnStop()
        {
        }
    }
}
