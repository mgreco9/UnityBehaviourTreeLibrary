namespace BehaviourTreeLibrary
{
    public class Negator : DecoratorProperty
    {
        public override DecoratorType Type
        {
            get
            {
                return DecoratorType.OUTPUT;
            }
        }

        public override void AfterStop()
        {
            // 1 - If node has succeeded, return failure
            if (node.state == NodeState.SUCCESS)
                node.state = NodeState.FAILURE;

            // 2 - If node has failed, return success
            else if (node.state == NodeState.FAILURE)
                node.state = NodeState.SUCCESS;
        }
    }
}