using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BehaviourTreeLibrary
{
    public class Repeater : DecoratorProperty
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
            node.state = NodeState.RUNNING;
        }
    }
}