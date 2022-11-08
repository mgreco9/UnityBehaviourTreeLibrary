using System.Collections;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace BehaviourTreeLibrary
{
    public class ActionNodeView : NodeView
    {
        public ActionNodeView(BehaviourTreeView treeView, Node node) : base(treeView, node) { }

        protected override void CreateInputPorts()
        {
            input = InstantiatePort(Orientation.Vertical, Direction.Input, Port.Capacity.Single, typeof(bool));

            input.portName = "";
            input.style.flexDirection = FlexDirection.Column;
            inputContainer.Add(input);
        }

        protected override void CreateOutputPorts()
        {
        }

        protected override void SetupClasses()
        {
            AddToClassList("action");
        }
    }
}