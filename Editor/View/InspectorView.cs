using BehaviourTreeLibrary;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace BehaviourTreeLibrary
{
    [CustomEditor(typeof(Node))]
    public class InspectorView : VisualElement
    {
        Editor editor;

        public new class UxmlFactory : UxmlFactory<InspectorView, UxmlTraits> { }

        internal void UpdateSelection(NodeView nodeView)
        {
            Clear();

            UnityEngine.Object.DestroyImmediate(editor);

            editor = Editor.CreateEditor(nodeView.node);
            IMGUIContainer container = new IMGUIContainer(() =>
            {
                if (editor.target)
                {
                    editor.OnInspectorGUI();
                }
            });
            Add(container);
        }

        internal void UpdateSelection(DecoratorView decoratorView)
        {
            Clear();

            UnityEngine.Object.DestroyImmediate(editor);

            editor = Editor.CreateEditor(decoratorView.decorator);
            IMGUIContainer container = new IMGUIContainer(() =>
            {
                if (editor.target)
                {
                    editor.OnInspectorGUI();
                }
            });
            Add(container);
        }
    }
}