using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace BehaviourTreeLibrary
{
    public abstract class CompositeNode : Node
    {
        [HideInInspector]  public List<Node> children = new List<Node>();

        public override int ChildrenCount
        {
            get
            {
                return children.Count;
            }
        }

#if UNITY_EDITOR
        public override void AddChild(Node child)
        {
            Undo.RecordObject(this, "Behaviour Tree (AddChild)");
            children.Add(child);
            EditorUtility.SetDirty(this);
        }

        public override void RemoveChild(Node child)
        {
            Undo.RecordObject(this, "Behaviour Tree (RemoveChild)");
            children.Remove(child);
            EditorUtility.SetDirty(this);
        }

        public override List<Node> GetChildren()
        {
            return children;
        }
#endif

        public override Node Clone()
        {
            CompositeNode node = Instantiate(this);
            node.children = children.ConvertAll(c => c.Clone());
            node.decorators = decorators.ConvertAll(d => d.Clone());
            return node;
        }
    }
}
