using System.Collections;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace BehaviourTreeLibrary
{
    public class RootNodeView : NodeView
    {
        public RootNodeView(BehaviourTreeView treeView, Node node) : base(treeView, node) { }

        protected override void CreateInputPorts()
        {
        }

        protected override void CreateOutputPorts()
        {
            output = InstantiatePort(Orientation.Vertical, Direction.Output, Port.Capacity.Single, typeof(bool));

            output.portName = "";
            output.style.flexDirection = FlexDirection.ColumnReverse;
            outputContainer.Add(output);
        }

        protected override void SetupClasses()
        {
            AddToClassList("root");
        }
    }
}