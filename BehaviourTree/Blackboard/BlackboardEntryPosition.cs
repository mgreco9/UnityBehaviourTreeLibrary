using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

#if UNITY_EDITOR
using UnityEditor.UIElements;
#endif

public class BlackboardEntryPosition : BlackboardEntry
{
    [SerializeField] private string b_key = "";
    public override string B_key
    {
        get
        {
            return b_key;
        }
        set
        {
            b_key = value;
        }
    }

    [SerializeField] private Vector3 b_value;
    public override object B_value
    {
        get
        {
            return b_value;
        }
        set
        {
            if (value.GetType() == typeof(Vector3))
                b_value = (Vector3)value;
        }
    }
    public override string B_typeName
    {
        get { return "Position"; }
    }

#if UNITY_EDITOR
    private Vector3Field field;

    public override VisualElement BuildFieldView()
    {
        field = new Vector3Field("value:");
        field.value = (Vector3)B_value;
        field.RegisterValueChangedCallback((evt) => { UpdateValue(evt.newValue); });
        return field;
    }

    public override void UpdateFieldView()
    {
        field.value = b_value;
    }
#endif
}
