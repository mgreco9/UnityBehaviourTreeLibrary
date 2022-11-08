using System.Collections;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace BehaviourTreeLibrary
{
    public class CompositeNodeView : NodeView
    {
        public CompositeNodeView(BehaviourTreeView treeView, Node node) : base(treeView, node) { }

        protected override void CreateInputPorts()
        {
            input = InstantiatePort(Orientation.Vertical, Direction.Input, Port.Capacity.Single, typeof(bool));
            
            input.portName = "";
            input.style.flexDirection = FlexDirection.Column;
            inputContainer.Add(input);
        }

        protected override void CreateOutputPorts()
        {
            output = InstantiatePort(Orientation.Vertical, Direction.Output, Port.Capacity.Multi, typeof(bool));
           
            output.portName = "";
            output.style.flexDirection = FlexDirection.ColumnReverse;
            outputContainer.Add(output);
        }

        protected override void SetupClasses()
        {
            AddToClassList("composite");
        }

        public void SortChildren()
        {
            CompositeNode composite = node as CompositeNode;
            if (composite)
            {
                composite.children.Sort(SortByHorizontalPosition);
            }
        }
    }
}