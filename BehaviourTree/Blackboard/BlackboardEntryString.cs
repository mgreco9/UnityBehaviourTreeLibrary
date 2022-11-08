using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class BlackboardEntryString : BlackboardEntry
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

    [SerializeField] private string b_value = "";
    public override object B_value
    {
        get
        {
            return b_value;
        }
        set
        {
            if (value.GetType() == typeof(string))
                b_value = (string)value;
        }
    }
    public override string B_typeName
    {
        get { return "String"; }
    }

#if UNITY_EDITOR
    private TextField field;

    public override VisualElement BuildFieldView()
    {
        field = new TextField("value:");
        field.value = b_value;
        field.RegisterValueChangedCallback((evt) => { UpdateValue(evt.newValue); });
        return field;
    }

    public override void UpdateFieldView()
    {
        field.value = b_value;
    }
#endif
}
