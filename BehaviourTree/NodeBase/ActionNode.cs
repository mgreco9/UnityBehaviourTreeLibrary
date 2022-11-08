using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BehaviourTreeLibrary
{
    public abstract class ActionNode : Node
    {
        public override int ChildrenCount
        {
            get
            {
                return 0;
            }
        }
#if UNITY_EDITOR
        public override void AddChild(Node child)
        {

        }

        public override void RemoveChild(Node child)
        {

        }
#endif
        public override List<Node> GetChildren()
        {
            return new List<Node>();
        }
    }
}