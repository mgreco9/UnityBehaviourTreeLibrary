using BehaviourTreeLibrary;
using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

[Serializable]
public abstract class BlackboardEntry : ScriptableObject
{
    public BehaviourTree tree;
    public abstract string B_key { get; set; }
    public abstract object B_value { get; set; }
    public abstract string B_typeName { get; }

#if UNITY_EDITOR
    public abstract VisualElement BuildFieldView();
    public abstract void UpdateFieldView();

    public void UpdateValue(object value)
    {
        Undo.RecordObject(tree, "Behaviour Tree (UpdateBlackboardEntry)");

        B_value = value;

        AssetDatabase.SaveAssets();
    }
#endif
    public BlackboardEntry Clone()
    {
        return Instantiate(this);
    }
}

