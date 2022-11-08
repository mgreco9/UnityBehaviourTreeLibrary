using System.Collections.Generic;

namespace BehaviourTreeLibrary
{
    public class ParallelSelector : CompositeNode
    {
        public override void OnStart()
        {
        }

        public override NodeState OnUpdate()
        {
            bool stillRunning = false;

            // 1 - Iterate over each child node (one node must succeed)
            foreach (Node child in children)
            {
                state = child.Evaluate();

                // 2 - If child node has succeeded, return success
                if (state == NodeState.SUCCESS)
                    return NodeState.SUCCESS;

                // 3 - If child node is still running, mark selector as running
                if (state == NodeState.RUNNING)
                    stillRunning = true;
            }

            // 4 - If still iterating over nodes, return running
            if (stillRunning)
                return NodeState.RUNNING;

            // 5 - If every node failed, return failure
            return NodeState.FAILURE;
        }

        public override void OnStop()
        {
        }
    }
}
