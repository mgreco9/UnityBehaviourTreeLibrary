using BehaviourTreeLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;
using static UnityEditor.TypeCache;

namespace BehaviourTreeLibrary
{
    public class BehaviourTreeView : GraphView
    {
        public BehaviourTreeWindow window;

        public BlackboardView blackboardView;

        public BehaviourTree tree;

        public new class UxmlFactory : UxmlFactory<BehaviourTreeView, GraphView.UxmlTraits> { }

        public BehaviourTreeView()
        {
            // 1 - Set grid background
            Insert(0, new GridBackground());

            // 2 - Add manipulators
            this.AddManipulator(new ContentZoomer());
            this.AddManipulator(new ContentDragger());
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new FocusRectangleSelector());

            // 3 - Add blackboard
            blackboardView = new BlackboardView(this);
            this.Add(blackboardView);

            // 4 - Set style sheet
            StyleSheet styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/BehaviourTreeLibrary/Editor/Uss/BehaviourTreeWindow.uss");
            styleSheets.Add(styleSheet);

            // 5 - Set the Undo Redo event
            Undo.undoRedoPerformed = OnUndoRedo;
        }

        /// <summary>
        /// On Undo/Redo, reload the view
        /// </summary>
        private void OnUndoRedo()
        {
            PopulateView(tree);
            AssetDatabase.SaveAssets();
        }

        /// <summary>
        /// Map the view to the window
        /// </summary>
        /// <param name="behaviourTreeWindow">The parent window</param>
        public void InitializeView(BehaviourTreeWindow behaviourTreeWindow)
        {
            this.window = behaviourTreeWindow;

            window.recenterGridButton.clicked += RecenterGrid;
            window.reorganizeNodesButton.clicked += ReorganizeNodes;
        }

        /// <summary>
        /// Retrieve the NodeView of a specific Node object
        /// </summary>
        /// <param name="node">The Node object to find</param>
        /// <returns></returns>
        NodeView FindNodeView(Node node)
        {
            return GetNodeByGuid(node.guid) as NodeView;
        }

        /// <summary>
        /// Populate the view with the Behaviour Tree
        /// </summary>
        /// <param name="tree">The Behaviour Tree to populate the view with</param>
        public void PopulateView(BehaviourTree tree)
        {
            // 1 - Keep the tree in cache
            this.tree = tree;

            // 2 - Reload the saved transform from the tree
            viewTransform.position = tree.viewTransform;
            viewTransform.scale = tree.viewScale;

            // 3 - Empty the previously loaded elements (deactivate trigger for this)
            graphViewChanged -= OnGraphViewChanged;
            DeleteElements(graphElements);
            graphViewChanged += OnGraphViewChanged;

            // 4 - Set the trigger on view transform changed
            viewTransformChanged = OnViewTransformChanged;

            // 5 - If the root node does not exist, instantiate it
            if (tree.rootNode == null)
            {
                tree.rootNode = tree.CreateNode(typeof(RootNode)) as RootNode;
                EditorUtility.SetDirty(tree);
                AssetDatabase.SaveAssets();
            }

            // 6 - Create node view
            tree.nodes.ForEach(n => CreateNodeView(n));

            // 7 - Create edges
            tree.nodes.ForEach(n =>
            {
                NodeView parentView = FindNodeView(n);
                List<Node> children = n.GetChildren();
                children.ForEach(c =>
                {
                    NodeView childView = FindNodeView(c);

                    Edge edge = parentView.output.ConnectTo(childView.input);
                    AddElement(edge);
                });
            });

            // 8 - Populate the blackboard
            blackboardView.ClearBlackboard();
            blackboardView.PopulateBlackboard();
        }

        /// <summary>
        /// Define the compatible ports where connection is possible
        /// </summary>
        /// <param name="startPort">The starting port</param>
        /// <param name="nodeAdapter"></param>
        /// <returns></returns>
        public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
        {
            return ports.ToList().Where(endPort =>
            endPort.direction != startPort.direction &&
            endPort.node != startPort.node).ToList();
        }

        /// <summary>
        /// Set the transform to the center of the tree
        /// </summary>
        public void RecenterGrid()
        {
            // 1 - Initialize the border coordinates of the transform
            float xMin = float.MaxValue;
            float xMax = float.MinValue;
            float yMin = float.MaxValue;
            float yMax = float.MinValue;

            // 2 - Find the most extreme position of the nodes
            this.Query<NodeView>().ForEach((n) =>
            {
                Rect position = n.GetPosition();

                float xPos = position.center.x;
                float yPos = position.center.y;

                xMin = Math.Min(xMin, xPos);
                xMax = Math.Max(xMax, xPos);
                yMin = Math.Min(yMin, yPos);
                yMax = Math.Max(yMax, yPos);
            });

            // 3 - Compute the new transform
            Vector3 diffCenter = new Vector3(contentRect.width / 2, contentRect.height / 2, 0);
            Vector3 scale = contentViewContainer.transform.scale;

            Vector3 viewPosition = new Vector3((xMax + xMin) / 2, (yMax + yMin) / 2, 0);
            viewPosition = Vector3.Scale(viewPosition, scale);
            viewPosition -= diffCenter;

            // 4 - Update the transform
            UpdateViewTransform(-viewPosition, scale);
        }

        /// <summary>
        /// Reorganize the nodes in the view
        /// </summary>
        public void ReorganizeNodes()
        {
            // 1 - Retrieve the list of node in a matrix
            List<List<Node>> nodesByLevel = tree.GetNodesByLevel();

            // 2 - Iterate over each node in the matrix
            float yPos = 0f;
            foreach (List<Node> level in nodesByLevel)
            {
                // 2.1 - Find the position of the left most node
                float xPos = -((level.Count - 1) * (level.Max(n => FindNodeView(n).GetPosition().width) + 16 + 20)) / 2;

                // 2.2 - Iterate over each node of this level
                float maxHeight = 0;
                foreach (Node node in level)
                {
                    // 2.2.1 - Retrieve the node view
                    NodeView nodeView = FindNodeView(node);

                    // 2.2.2 - Set the new coordinates
                    Rect currPos = nodeView.GetPosition();
                    nodeView.SetPosition(new Rect(xPos, yPos, currPos.width, currPos.height));

                    // 2.2.3 - Increase the x position for the next node to place
                    xPos += currPos.width + 40;
                    maxHeight = Math.Max(maxHeight, currPos.height);
                }
                // 2.3 - Increase the y position for the next line of nodes
                yPos += maxHeight + 40;
            }
        }

        /// <summary>
        /// Trigger when a modification happens on the graph
        /// </summary>
        /// <param name="graphViewChange">The container listing the changes</param>
        /// <returns></returns>
        private GraphViewChange OnGraphViewChanged(GraphViewChange graphViewChange)
        {
            // 1 - If any elements where removed
            if (graphViewChange.elementsToRemove != null)
            {
                graphViewChange.elementsToRemove.ForEach(elem =>
                {
                    if (elem is NodeView nodeView)
                    {
                        tree.DeleteNode(nodeView.node);
                    }

                    if (elem is DecoratorView decoratorView)
                    {
                        NodeView parentView = decoratorView.nodeView;
                        parentView.node.DeleteDecorator(decoratorView.decorator);
                    }

                    if (elem is Edge edge)
                    {
                        NodeView parentView = edge.output.node as NodeView;
                        NodeView childView = edge.input.node as NodeView;
                        tree.RemoveChild(parentView.node, childView.node);
                    }
                });

                if (graphViewChange.elementsToRemove.Count > 1)
                {
                    Undo.SetCurrentGroupName("Behaviour Tree (DeleteBulk)");
                }
            }

            // 2 - If any edge were created
            if (graphViewChange.edgesToCreate != null)
            {
                graphViewChange.edgesToCreate.ForEach(edge =>
                {
                    NodeView parentView = edge.output.node as NodeView;
                    NodeView childView = edge.input.node as NodeView;
                    tree.AddChild(parentView.node, childView.node);
                });
            }

            // 3 - If any element were moved
            if (graphViewChange.movedElements != null)
            {
                nodes.ForEach((n) =>
                {
                    CompositeNodeView view = n as CompositeNodeView;
                    view?.SortChildren();
                });
            }

            return graphViewChange;
        }

        /// <summary>
        /// Trigger the camera is moved
        /// </summary>
        /// <param name="graphView">The container of the view transform</param>
        private void OnViewTransformChanged(GraphView graphView)
        {
            // 1 - Retrieve the transform
            ITransform transform = graphView.viewTransform;
            
            // 2 - Save the transform into the behaviour tree
            if (transform != null)
            {
                tree.viewTransform = transform.position;
                tree.viewScale = transform.scale;
            }
        }

        /// <summary>
        /// Use to build a contextual menu (right click)
        /// </summary>
        /// <param name="evt">The contextual menu event</param>
        public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
        {
            base.BuildContextualMenu(evt);

            // 1 - Add one option for each action node available to create
            TypeCollection actionTypes = TypeCache.GetTypesDerivedFrom<ActionNode>();
            foreach (Type actionType in actionTypes)
            {
                evt.menu.AppendAction($"New Node/[{actionType.BaseType.Name}] {actionType.Name}", (a) => CreateNode(a.eventInfo.localMousePosition, actionType));
            }

            // 2 - Add one option for each composite node available to create
            TypeCollection compositeTypes = TypeCache.GetTypesDerivedFrom<CompositeNode>();
            foreach (Type compositeType in compositeTypes)
            {
                evt.menu.AppendAction($"New Node/[{compositeType.BaseType.Name}] {compositeType.Name}", (a) => CreateNode(a.eventInfo.localMousePosition, compositeType));
            }
        }

        /// <summary>
        /// Called when a node is being created
        /// </summary>
        /// <param name="mousePosition">The current mouse position where to create the node</param>
        /// <param name="type">The node type</param>
        private void CreateNode(Vector2 mousePosition, Type type)
        {
            // 1 - Instantiate the node and its view
            Node node = tree.CreateNode(type);
            NodeView nodeView = CreateNodeView(node);

            // 2 - Set the view transform
            Vector2 matrixPosition = viewTransform.matrix.inverse.MultiplyPoint(mousePosition);
            nodeView.SetPosition(new Rect(matrixPosition, new Vector2(200, 250)));
        }

        /// <summary>
        /// Called to instantiate the view of a node
        /// </summary>
        /// <param name="node">The node to show</param>
        /// <returns>The corresponding Node View</returns>
        NodeView CreateNodeView(Node node)
        {
            NodeView nodeView = NodeViewFactory.CreateNodeView(this, node);
            nodeView.LoadDecorator();
            AddElement(nodeView);
            return nodeView;
        }

        // TRIGGER DOWN EVENTS
        /// <summary>
        /// Load the node attributes
        /// </summary>
        public void LoadAttributes()
        {
            tree.nodes.ForEach(n =>
            {
                NodeView nodeView = FindNodeView(n);
                nodeView.LoadAttributes();
            });
        }

        /// <summary>
        /// Call to update the current state of the node (used at runtime)
        /// </summary>
        public void UpdateNodeStates()
        {
            nodes.ForEach(n =>
            {
                NodeView view = n as NodeView;
                view.UpdateState();
            });
        }

        /// <summary>
        /// Call to update the blackboard values (used at runtime)
        /// </summary>
        internal void UpdateBlackboard()
        {
            blackboardView.UpdateFieldValues();
        }

        // BUBBLE UP EVENTS
        public void OnNodeSelected(NodeView nodeView)
        {
            window.OnNodeSelectionChanged(nodeView);
        }

        public void OnDecoratorSelected(DecoratorView decoratorView)
        {
            window.OnDecoratorSelectionChanged(decoratorView);
        }
    }
}
