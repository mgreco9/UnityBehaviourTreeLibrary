using BehaviourTreeLibrary;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;
using static UnityEditor.TypeCache;
using Node = BehaviourTreeLibrary.Node;

namespace BehaviourTreeLibrary
{
    public abstract class NodeView : UnityEditor.Experimental.GraphView.Node
    {
        public BehaviourTreeView treeView;
        public Node node;

        public Label nodeDescription;
        public Port input;
        public Port output;
        public VisualElement decoratorContainer;
        public UQueryState<DecoratorView> decorators { get; protected set; }
        public List<DecoratorView> decoratorViewsList = new List<DecoratorView>();

        public NodeView(BehaviourTreeView treeView, Node node) : base("Assets/BehaviourTreeLibrary/Editor/Uss/NodeView.uxml")
        {
            // 1 - Keep attributes in cache
            this.treeView = treeView;
            this.node = node;

            // 2 - Retrieve the visual elements
            nodeDescription = this.Q<Label>("description");
            decoratorContainer = this.Q<VisualElement>("decorator-container");

            // 3 - Build the decorator container
            decorators = decoratorContainer.Query<DecoratorView>().Build();

            // 4 - Retrieve the view attributes from the node
            viewDataKey = node.guid;
            style.position = Position.Absolute;
            style.left = node.gridPosition.x;
            style.top = node.gridPosition.y;
            title = node.name;
            nodeDescription.text = node.Description;

            // 5 - Instantiate the specific node views
            CreateInputPorts();
            CreateOutputPorts();
            SetupClasses();
        }

        /// <summary>
        /// Instantiate the input port
        /// </summary>
        protected abstract void CreateInputPorts();

        /// <summary>
        /// Instantiate the output port
        /// </summary>
        protected abstract void CreateOutputPorts();

        /// <summary>
        /// Set the class list
        /// </summary>
        protected abstract void SetupClasses();

        /// <summary>
        /// Set the position of the node view
        /// </summary>
        /// <param name="newPos">New position input</param>
        public override void SetPosition(Rect newPos)
        {
            base.SetPosition(newPos);
            Undo.RecordObject(node, "Behaviour Tree (Set Position)");

            node.gridPosition = newPos.position;
            node.gridSize = newPos.size;
            EditorUtility.SetDirty(node);
        }

        /// <summary>
        /// Update the node and decorator attribute if necessary
        /// </summary>
        public void LoadAttributes()
        {
            if (node.updateView)
            {
                nodeDescription.text = node.Description;
                node.updateView = false;
            }
            decorators.ToList().ForEach(d => d.LoadAttributes());
        }

        /// <summary>
        /// Use to build a contextual menu (right click)
        /// </summary>
        /// <param name="evt">The contextual menu event</param>
        public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
        {
            base.BuildContextualMenu(evt);

            // 1 - For each available decorator type, add one option to the contextual menu
            TypeCollection decoratorTypes = TypeCache.GetTypesDerivedFrom<DecoratorProperty>();
            foreach (Type decorator in decoratorTypes)
            {
                evt.menu.AppendAction($"New Decorator/[{decorator.BaseType.Name}] {decorator.Name}", (d) => CreateDecorator(decorator));
            }

            // 2 - Add a separator to differentiate from the other option sets
            evt.menu.AppendSeparator();
        }

        /// <summary>
        /// Called when a decorator is being created
        /// </summary>
        /// <param name="type">The decorator type to create</param>
        protected void CreateDecorator(Type type)
        {
            DecoratorProperty decorator = node.CreateDecorator(type);
            CreateDecoratorView(decorator);
        }

        /// <summary>
        /// Called to instantiate the view of a decorator
        /// </summary>
        /// <param name="decorator">The decorator to show</param>
        void CreateDecoratorView(DecoratorProperty decorator)
        {
            DecoratorView decoratorView = new DecoratorView(this, decorator);

            capabilities |= Capabilities.Selectable;

            decoratorContainer.Add(decoratorView);
        }

        /// <summary>
        /// Sort two nodes based on their position (left node must happen before right node)
        /// </summary>
        /// <param name="node1"></param>
        /// <param name="node2"></param>
        /// <returns>-1 if node1 is before node2, 1 otherwise</returns>
        protected int SortByHorizontalPosition(Node node1, Node node2)
        {
            return node1.gridPosition.x < node2.gridPosition.x ? -1 : 1;
        }

        /// <summary>
        /// Used to update the state view
        /// </summary>
        public void UpdateState()
        {
            if (!Application.isPlaying)
                return;

            RemoveFromClassList("running");
            RemoveFromClassList("success");
            RemoveFromClassList("failure");

            switch (node.state)
            {
                case NodeState.RUNNING:
                    AddToClassList("running");
                    break;
                case NodeState.SUCCESS:
                    AddToClassList("success");
                    break;
                case NodeState.FAILURE:
                    AddToClassList("failure");
                    break;
            }
        }

        /// <summary>
        /// Load the decorator views of the node
        /// </summary>
        public void LoadDecorator()
        {
            node.decorators.ForEach(d =>
            {
                CreateDecoratorView(d);
            });
        }

        /// <summary>
        /// Trigger when user select the node
        /// </summary>
        public override void OnSelected()
        {
            base.OnSelected();
            treeView.OnNodeSelected(this);
        }
        
        /// <summary>
        /// Trigger when a decorator is selected
        /// </summary>
        /// <param name="decoratorView">The decorator view selected</param>
        public void OnDecoratorSelected(DecoratorView decoratorView)
        {
            treeView.OnDecoratorSelected(decoratorView);
        }
    }
}