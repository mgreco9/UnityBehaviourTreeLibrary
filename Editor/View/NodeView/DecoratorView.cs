using BehaviourTreeLibrary;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace BehaviourTreeLibrary
{
    public class DecoratorView : GraphElement
    {
        public DecoratorProperty decorator;

        private Label decoratorTitle;
        private Label decoratorDescription;

        public NodeView nodeView;

        public DecoratorView(NodeView nodeView, DecoratorProperty decorator)
        {
            // 1 - Keep the attributes in cache
            this.nodeView = nodeView;
            this.decorator = decorator;

            // 2 - Load the style of the visual asset
            ClearClassList();
            AddToClassList("decorator");
            var asset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/BehaviourTreeLibrary/Editor/Uss/DecoratorView.uxml");
            styleSheets.Add(AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/BehaviourTreeLibrary/Editor/Uss/NodeView.uss"));
            asset.CloneTree(this);

            // 3 - Set the graph element capabilities and usage hint
            capabilities |= Capabilities.Selectable | Capabilities.Deletable;
            usageHints = UsageHints.None;

            // 4 - Retrieve the visual elements
            decoratorTitle = this.Q<Label>("title");
            decoratorDescription = this.Q<Label>("description");

            // 5 - Set the view attributes to show
            decoratorTitle.text = decorator.name;
            decoratorDescription.text = decorator.Description;

            // 7 - Set the style class
            SetupClasses();
        }

        /// <summary>
        /// Used to set up the decorator class based on its type
        /// </summary>
        private void SetupClasses()
        {
            switch (decorator.Type)
            {
                case DecoratorType.START_CONDITION:
                    AddToClassList("start-condition");
                    break;
                case DecoratorType.UPDATE:
                    AddToClassList("update");
                    break;
                case DecoratorType.STOP_CONDITION:
                    AddToClassList("stop-condition");
                    break;
                case DecoratorType.OUTPUT:
                    AddToClassList("output");
                    break;
            }
        }

        /// <summary>
        /// Load the decorator attributes
        /// </summary>
        public void LoadAttributes()
        {
            if (decorator.updateView)
            {
                decoratorDescription.text = decorator.Description;
                decorator.updateView = false;
            }
        }

        /// <summary>
        /// Trigger when the decorator is selected
        /// </summary>
        public override void OnSelected()
        {
            base.OnSelected();
            nodeView.OnDecoratorSelected(this);
        }
    }
}