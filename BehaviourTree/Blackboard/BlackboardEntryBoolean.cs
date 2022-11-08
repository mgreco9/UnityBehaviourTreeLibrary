using UnityEngine;
using UnityEngine.UIElements;

public class BlackboardEntryBoolean : BlackboardEntry
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

    [SerializeField] private bool b_value;
    public override object B_value
    {
        get
        {
            return b_value;
        }
        set
        {
            if (value.GetType() == typeof(bool))
                b_value = (bool)value;
        }
    }
    public override string B_typeName
    {
        get { return "Boolean"; }
    }

#if UNITY_EDITOR
    private Toggle field;
    public override VisualElement BuildFieldView()
    {
        field = new Toggle("value:");
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
