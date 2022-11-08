using System.Collections.Generic;

namespace BehaviourTreeLibrary
{
    public class Selector : CompositeNode
    {
        protected int currentChildNodeIdx;

        public override void OnStart()
        {
            currentChildNodeIdx = 0;
        }

        public override NodeState OnUpdate()
        {
            // 1 - Iterate over each child node (one node must succeed)
            state = children[currentChildNodeIdx].Evaluate();

            // 2 - If child node has succeeded, return success
            if (state == NodeState.SUCCESS)
                return NodeState.SUCCESS;

            // 3 - If child node has failed, iterate over next node
            if (state == NodeState.FAILURE)
                currentChildNodeIdx++;

            // 4 - If every node have been realized, return failure
            if (currentChildNodeIdx >= children.Count)
                return NodeState.FAILURE;

            // 5 - If still iterating over nodes, return running
            return NodeState.RUNNING;
        }

        public override void OnStop()
        {
        }
    }
}
