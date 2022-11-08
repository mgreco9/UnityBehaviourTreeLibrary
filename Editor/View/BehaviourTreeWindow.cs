using BehaviourTreeLibrary;
using System;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;
using UnityEngine.UIElements;

namespace BehaviourTreeLibrary
{
    public class BehaviourTreeWindow : EditorWindow
    {
        public BehaviourTreeView treeView;
        public InspectorView inspectorView;

        public Button recenterGridButton;
        public Button reorganizeNodesButton;

        [MenuItem("Window/Behaviour Tree Editor")]
        public static void OpenWindow()
        {
            // Get existing open window or if none, make a new one:
            BehaviourTreeWindow window = GetWindow<BehaviourTreeWindow>();
            window.titleContent = new GUIContent("BehaviourTreeEditor");
        }

        [OnOpenAsset]
        public static bool OnOpenAsset(int instanceId, int line)
        {
            if (Selection.activeObject is BehaviourTree)
            {
                OpenWindow();
                return true;
            }

            return false;
        }

        /// <summary>
        /// Instantiate and initialize the window content
        /// </summary>
        public void CreateGUI()
        {
            // 1 - Import UXML
            VisualTreeAsset visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/BehaviourTreeLibrary/Editor/Uss/BehaviourTreeWindow.uxml");
            visualTree.CloneTree(rootVisualElement);

            // 2 - Set stylesheet 
            StyleSheet uss = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/BehaviourTreeLibrary/Editor/Uss/BehaviourTreeWindow.uss");
            rootVisualElement.styleSheets.Add(uss);

            // 3 - Retrieve the views
            treeView = rootVisualElement.Q<BehaviourTreeView>();
            inspectorView = rootVisualElement.Q<InspectorView>();
            recenterGridButton = rootVisualElement.Q<Button>("recenter-button");
            reorganizeNodesButton = rootVisualElement.Q<Button>("reorganize-button");

            // 4 - Connect window to tree view
            treeView.InitializeView(this);

            // 5 - Initialize tree on current selection
            OnSelectionChange();
        }

        private void OnEnable()
        {
            EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
        }

        private void OnDisable()
        {
            EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
        }

        private void OnPlayModeStateChanged(PlayModeStateChange obj)
        {
            switch (obj)
            {
                case PlayModeStateChange.EnteredEditMode:
                    OnSelectionChange();
                    break;
                case PlayModeStateChange.ExitingEditMode:
                    break;
                case PlayModeStateChange.EnteredPlayMode:
                    OnSelectionChange();
                    break;
                case PlayModeStateChange.ExitingPlayMode:
                    break;
            }
        }

        /// <summary>
        /// When a Behaviour Tree is selected, load it in the window
        /// </summary>
        private void OnSelectionChange()
        {
            BehaviourTree tree = Selection.activeObject as BehaviourTree;
            if (!tree && Selection.activeGameObject)
            {
                BehaviourTreeRunner runner = Selection.activeGameObject.GetComponent<BehaviourTreeRunner>();
                if (runner)
                {
                    tree = runner.behaviourTree;
                }
            }

            if (tree && (Application.isPlaying || AssetDatabase.CanOpenAssetInEditor(tree.GetInstanceID())))
            {
                treeView.PopulateView(tree);
            }
        }

        /// <summary>
        /// Trigger when a node in the view is selected.
        /// Update the inspector view to show the details of the node
        /// </summary>
        /// <param name="node">The NodeView selected</param>
        public void OnNodeSelectionChanged(NodeView node)
        {
            inspectorView.UpdateSelection(node);
        }

        /// <summary>
        /// Trigger when a decorator in the view is selected.
        /// Update the inspector view to show the details of the decorator
        /// </summary>
        /// <param name="decorator">The DecoratorView selected</param>
        public void OnDecoratorSelectionChanged(DecoratorView decorator)
        {
            inspectorView.UpdateSelection(decorator);
        }

        /// <summary>
        /// Trigger every few seconds, update the view at runtime
        /// </summary>
        private void OnInspectorUpdate()
        {
            treeView?.UpdateNodeStates();
            treeView?.UpdateBlackboard();
            treeView?.LoadAttributes();
        }
    }
}