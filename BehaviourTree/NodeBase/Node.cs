using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace BehaviourTreeLibrary
{
    public enum NodeState
    {
        RUNNING,
        SUCCESS,
        FAILURE,
        NONE
    }

    public abstract class Node : ScriptableObject
    {
        // NODE STATE
        [HideInInspector] public NodeState state;
        [HideInInspector] public bool started = false;

        // DECORATORS
        [HideInInspector] public List<DecoratorProperty> decorators = new List<DecoratorProperty>();

        // BLACKBOARD
        [HideInInspector] [NonSerialized] public Dictionary<string, BlackboardEntry> blackboard;

#if UNITY_EDITOR
        // NODE VIEW
        [HideInInspector] public string guid;
        [HideInInspector] public Vector2 gridPosition;
        [HideInInspector] public Vector2 gridSize;
        [HideInInspector] public bool updateView;
        public virtual string Description
        {
            get
            {
                return "";
            }
        }
#endif

        // NODE CHILDREN
        [HideInInspector] public abstract int ChildrenCount { get; }

        public NodeState Evaluate()
        {
            if(!started)
            {
                decorators.ForEach(d => { d.BeforeStart(); });
                OnStart();
                decorators.ForEach(d => { d.AfterStart(); });
                state = NodeState.RUNNING;
                started = true;
            }

            if (state == NodeState.RUNNING)
            {
                decorators.ForEach(d => { d.BeforeUpdate(); });
                state = OnUpdate();
                decorators.ForEach(d => { d.AfterUpdate(); });
            }

            if (state == NodeState.FAILURE || state == NodeState.SUCCESS)
            {
                decorators.ForEach(d => { d.BeforeStop(); });
                OnStop();
                decorators.ForEach(d => { d.AfterStop(); });
                started = false;
            }

            return state;
        }
        public void Bind()
        {
           decorators.ForEach(d =>
           {
               d.node = this;
               d.Load();
           });
        }
        public virtual void Load()
        {
        }

        public abstract void OnStart();
        public abstract NodeState OnUpdate();
        public abstract void OnStop();

#if UNITY_EDITOR
        
        public DecoratorProperty CreateDecorator(System.Type type)
        {
            // 1 - Create decorator
            DecoratorProperty decorator = ScriptableObject.CreateInstance(type) as DecoratorProperty;
            decorator.name = type.Name;
            decorator.node = this;

            // 2 - Undo record
            Undo.RecordObject(this, "Behaviour Tree (CreateDecorator)");
            decorators.Add(decorator);

            // 3 - Add asset if not playing
            if (!Application.isPlaying)
            {
                AssetDatabase.AddObjectToAsset(decorator, this);
            }

            // 4 - Save asset
            AssetDatabase.SaveAssets();
            return decorator;
        }

        public void DeleteAllDecorators()
        {
            List<DecoratorProperty> decoratorsCopy = new List<DecoratorProperty>(decorators);
            decoratorsCopy.ForEach(d => DeleteDecorator(d));
        }

        public void DeleteDecorator(DecoratorProperty decorator)
        {
            Undo.RecordObject(this, "");
            decorators.Remove(decorator);

            Undo.DestroyObjectImmediate(decorator);
            Undo.SetCurrentGroupName("Behaviour Tree (DeleteDecorator)");

            AssetDatabase.SaveAssets();
        }

        public abstract void AddChild(Node child);
        public abstract void RemoveChild(Node child);
        public abstract List<Node> GetChildren();

        private void OnValidate()
        {
            updateView = true;
        }

        private void OnDestroy()
        {
            DeleteAllDecorators();
        }
#endif
        public virtual Node Clone()
        {
            Node node = Instantiate(this);
            node.decorators = decorators.ConvertAll(d => d.Clone());

            return node;
        }
    }
}
