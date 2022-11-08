using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace BehaviourTreeLibrary
{
    public class RootNode : Node
    {
        public Node child;

        public override int ChildrenCount
        {
            get
            {
                return 1;
            }
        }

        public override void OnStart()
        {
        }

        public override void OnStop()
        {
        }

        public override NodeState OnUpdate()
        {
            return child.Evaluate();
        }

#if UNITY_EDITOR
        public override void AddChild(Node child)
        {
            Undo.RecordObject(this, "Behaviour Tree (AddChild)");
            this.child = child;
            EditorUtility.SetDirty(this);
        }

        public override void RemoveChild(Node child)
        {
            Undo.RecordObject(this, "Behaviour Tree (RemoveChild)");
            if (this.child == child)
                this.child = null;
            EditorUtility.SetDirty(this);
        }

        public override List<Node> GetChildren()
        {
            List<Node> children = new List<Node>();

            if (child != null)
                children.Add(child);

            return children;
        }
#endif

        public override Node Clone()
        {
            RootNode node = Instantiate(this);
            node.child = child.Clone();
            node.decorators = decorators.ConvertAll(d => d.Clone());
            return node;
        }
    }
}
