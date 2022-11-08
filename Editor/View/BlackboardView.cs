using BehaviourTreeLibrary;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEditor.TypeCache;

namespace BehaviourTreeLibrary
{
    public class BlackboardView : Blackboard
    {
        public BehaviourTreeView treeView;
        private List<BlackboardEntry> instantiatedEntries = new List<BlackboardEntry>();

        public BlackboardView(BehaviourTreeView associatedGraphView) : base(associatedGraphView)
        {
            treeView = associatedGraphView; 

            this.addItemRequested = (b) => OpenTypeMenu();
            this.editTextRequested = (b, f, s) => OnKeyEdit(f, s);
        }

        /// <summary>
        /// Clear the blackboard
        /// </summary>
        public void ClearBlackboard()
        {
            Clear();
            instantiatedEntries.Clear();
        }

        /// <summary>
        /// Populate the blackboard view based on the instantiated Behaviour Tree's blackboard
        /// </summary>
        public void PopulateBlackboard()
        {
            // 1 - Retrieve the tree from the view
            BehaviourTree tree = treeView.tree;

            // 2 - If tree is not instantiated, nothing to do
            if (tree == null)
                return;

            // 3 - Iterate over the blackboard entries of the tree and populate the view with it
            foreach (BlackboardEntry entry in tree.blackboard.Values)
            {
                AddBlackboardRowView(entry);
            }
        }

        /// <summary>
        /// Trigger when the user click on the add button
        /// Present the different available options to create an entry
        /// </summary>
        public void OpenTypeMenu()
        {
            // 1 - Instantiate the menu to present
            GenericMenu menu = new GenericMenu();

            // 2 - Add one option for each existing blackboard entry type
            TypeCollection entryTypes = TypeCache.GetTypesDerivedFrom<BlackboardEntry>();
            foreach (Type entryType in entryTypes)
            {
                menu.AddItem(new GUIContent("New " + entryType.Name[15..]), false, () => { AddPropertyToBlackboard(entryType); });
            }

            // 3 - Show the menu
            menu.ShowAsContext();
        }

        /// <summary>
        /// Trigger when the user tries to add a new blackboard entry
        /// </summary>
        /// <param name="entryType">The type of the blackboard entry to add</param>
        public void AddPropertyToBlackboard(Type entryType)
        {
            // 1 - Default entry key
            string key = GetUniqueKeyName("new entry");

            // 2 - Create blackboard entry in the behaviour tree
            BlackboardEntry entry = treeView.tree.CreateBlackboardEntry(key, entryType);

            // 3 - Add the entry view
            AddBlackboardRowView(entry);
        }

        /// <summary>
        /// Add a new blackboard entry into the view
        /// </summary>
        /// <param name="entry">The blackboard entry to show</param>
        public void AddBlackboardRowView(BlackboardEntry entry)
        {
            // 0 - Instantiate the new contained
            VisualElement container = new VisualElement();

            // 1 - Create the visual element key field
            BlackboardField field = new BlackboardField { text = entry.B_key, typeText = entry.B_typeName };
            Button deleteButton = new Button(() =>
            {
                treeView.tree.DeleteBlackboardEntry(field.text);
                Remove(container);
            });
            deleteButton.text = "-";
            field.contentContainer.Add(deleteButton);

            // 2 - Create the visual element value field
            VisualElement objectField = entry.BuildFieldView();

            // 3 - Add both fields to a container
            BlackboardRow blackboardRow = new BlackboardRow(field, objectField);

            container.Add(blackboardRow);

            // 4 - Add the container to the main one
            Add(container);

            // 5 - Mark the key has instantiated
            instantiatedEntries.Add(entry);
        }

        /// <summary>
        /// Trigger when the user has modified the key of an entry
        /// </summary>
        /// <param name="visualElement">The blackboard field modified</param>
        /// <param name="newKey">The new input value used as key</param>
        public void OnKeyEdit(VisualElement visualElement, string newKey)
        {
            // 0 - Retrieve the behaviour tree
            BehaviourTree tree = treeView.tree;

            // 1 - Cast to blackboard field
            BlackboardField field = visualElement as BlackboardField;

            // 2 - Retrieve the old key and value
            string oldKey = field.text;
            BlackboardEntry oldEntry = tree.blackboard[oldKey];

            // 3 - Replace the blackboard entry
            tree.RenameBlackboardEntry(oldKey, newKey);

            // 4 - Update the text view
            field.text = newKey;
        }

        /// <summary>
        /// Method used to make sure the key is not already instantiated by adding a number to it if necessary
        /// </summary>
        /// <param name="keyBase">The original key name given to the entry</param>
        /// <returns></returns>
        private string GetUniqueKeyName(string keyBase)
        {
            string key = keyBase;
            int counter = 1;

            while (treeView.tree.blackboard.ContainsKey(key))
            {
                key = keyBase + $"({counter})";
                counter++;
            }

            return key;
        }

        /// <summary>
        /// Update the showed field valued
        /// </summary>
        public void UpdateFieldValues()
        {
            foreach (BlackboardEntry entry in instantiatedEntries)
            {
                entry.UpdateFieldView();
            }
        }
    }
}