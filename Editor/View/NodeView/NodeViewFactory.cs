using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviourTreeLibrary
{
    public class NodeViewFactory
    {
        public static NodeView CreateNodeView(BehaviourTreeView treeView, Node node)
        {
            return node switch
            {
                RootNode => new RootNodeView(treeView, node),
                CompositeNode => new CompositeNodeView(treeView, node),
                ActionNode => new ActionNodeView(treeView, node),
                _ => throw new Exception("Error : node type is not defined"),
            };
        }
    }
}
