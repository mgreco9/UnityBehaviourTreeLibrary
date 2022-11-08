using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace BehaviourTreeLibrary
{
    [CreateAssetMenu]
    public class BehaviourTree : ScriptableObject, ISerializationCallbackReceiver
    {
        public Node rootNode = null;
        public List<Node> nodes = new List<Node>();

        public Dictionary<string, BlackboardEntry> blackboard = new Dictionary<string, BlackboardEntry>();

        [SerializeField]
        private List<BlackboardEntry> blackboardEntries = new List<BlackboardEntry>();

        public Vector3 viewTransform = Vector3.zero;
        public Vector3 viewScale = Vector3.one;

        public void OnBeforeSerialize()
        {
            blackboardEntries.RemoveAll(e => e is not null);
            foreach (BlackboardEntry entry in blackboard.Values)
            {
                blackboardEntries.Add(entry);
            }
        }
         
        public void OnAfterDeserialize() 
        {
        }

        public void OnValidate()
        {
            blackboard.Clear();
            foreach (BlackboardEntry entry in blackboardEntries)
            {
                if (entry is null)
                    continue;

                blackboard[entry.B_key] = entry;
            }
        }

        public void Bind()
        {
            Traverse(rootNode, (n) =>
            {
                n.blackboard = blackboard;
                n.Bind();
                n.Load();
            });
        }

        public NodeState Update()
        {
            if (rootNode == null)
                return NodeState.FAILURE;

            return rootNode.Evaluate(); 
        }

#if UNITY_EDITOR
        public Node CreateNode(System.Type type)
        {
            Node node = ScriptableObject.CreateInstance(type) as Node;
            node.name = type.Name;
            node.guid = GUID.Generate().ToString();

            Undo.RecordObject(this, "Behaviour Tree (CreateNode)");
            nodes.Add(node);

            if(!Application.isPlaying)
            {
                AssetDatabase.AddObjectToAsset(node, this);
            }

            AssetDatabase.SaveAssets();
            return node;
        }

        public void DeleteNode(Node node)
        {
            // 1 - Remove node
            Undo.RecordObject(this, "");
            nodes.Remove(node);

            Undo.DestroyObjectImmediate(node);

            // 2 - Rename undo operation
            Undo.SetCurrentGroupName("Behaviour Tree (DeleteNode)");
            AssetDatabase.SaveAssets();
        }

        public void AddChild(Node parent, Node child)
        {
            parent.AddChild(child);
        }

        public void RemoveChild(Node parent, Node child)
        {
            parent.RemoveChild(child);
        }

        public List<List<Node>> GetNodesByLevel()
        {
            List<List<Node>> nodesByLevel = new List<List<Node>>();

            int idx = 0;
            List<Node> nodesLevel0 = new List<Node>();
            nodesLevel0.Add(rootNode);
            nodesByLevel.Add(nodesLevel0);
            
            while(nodesByLevel[idx].Any(n => n.ChildrenCount > 0))
            {
                idx++;
                nodesByLevel.Add(new List<Node>());
                foreach(Node nodeAtSameLevel in nodesByLevel[idx - 1])
                {
                    nodesByLevel[idx].AddRange(nodeAtSameLevel.GetChildren());
                }
            }

            return nodesByLevel;
        }

        public BlackboardEntry CreateBlackboardEntry(string key, Type entryType)
        {
            // 1 - Check entry does not exist already
            if (blackboard.ContainsKey(key))
            {
                EditorUtility.DisplayDialog("Blackboard Error", "This property name already exists", "OK");
                throw new DuplicateWaitObjectException(key, "This property name already exists");
            }

            // 2 - Instantiate the entry;
            BlackboardEntry blackboardEntry = ScriptableObject.CreateInstance(entryType) as BlackboardEntry;
            blackboardEntry.tree = this;
            blackboardEntry.name = "Blackboard|" + key;
            blackboardEntry.B_key = key;

            // 3 - Add the object to cache
            Undo.RecordObject(this, "Behaviour Tree (AddBlackboardEntry)");
            blackboard[blackboardEntry.B_key] = blackboardEntry;

            // 4 - Register the asset
            if (!Application.isPlaying)
            {
                AssetDatabase.AddObjectToAsset(blackboardEntry, this);
            }

            AssetDatabase.SaveAssets();

            // 5 - Return the entry
            return blackboardEntry;
        }

        public BlackboardEntry RenameBlackboardEntry(string oldKey, string newKey)
        {
            // 1 - Check entry exists already
            if (!blackboard.ContainsKey(oldKey))
            {
                EditorUtility.DisplayDialog("Blackboard Error", "This property name does not exist", "OK");
                throw new DuplicateWaitObjectException(oldKey, "This property name does not exist");
            }

            // 2 - Update the entry;
            BlackboardEntry blackboardEntry = blackboard[oldKey];
            blackboardEntry.name = "Blackboard|" + newKey;
            blackboardEntry.B_key = newKey;

            // 3 - Add the object to cache
            Undo.RecordObject(this, "Behaviour Tree (Rename Blackboard Entry)");
            blackboard.Remove(oldKey);
            blackboard[newKey] = blackboardEntry;

            // 4 - Save the asset
            AssetDatabase.SaveAssets();

            // 5 - Return the entry
            return blackboardEntry;
        }
        
        public void DeleteBlackboardEntry(String key)
        {
            if (!blackboard.ContainsKey(key))
                return;

            DeleteBlackboardEntry(blackboard[key]);
        }

        public void DeleteBlackboardEntry(BlackboardEntry entry)
        {
            if (Application.isPlaying)
                return;

            Undo.RecordObject(this, "Behaviour Tree (DeleteBlackboardEntry)");
            blackboard.Remove(entry.B_key);

            Undo.DestroyObjectImmediate(entry);

            AssetDatabase.SaveAssets();
        }

#endif

        public BlackboardEntry GetBlackboardEntry(string key)
        {
            return blackboard[key];
        }

        public void Traverse(Node node, System.Action<Node> visiter)
        {
            if (node)
            {
                visiter.Invoke(node);
                List<Node> children = node.GetChildren();
                children.ForEach((n) => Traverse(n, visiter));
            }
        }

        public BehaviourTree Clone()
        {
            BehaviourTree tree = Instantiate(this);
            tree.OnValidate();
            tree.rootNode = tree.rootNode.Clone();
            tree.nodes = new List<Node>();
            Traverse(tree.rootNode, (n) =>
            {
                tree.nodes.Add(n);
            });

            foreach (BlackboardEntry entry in blackboard.Values)
            {
                tree.blackboard[entry.B_key] = entry.Clone();
            }

            return tree;
        }
    }
}
