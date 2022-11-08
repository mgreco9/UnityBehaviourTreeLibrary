using UnityEngine;
using UnityEngine.UIElements;

#if UNITY_EDITOR
using UnityEditor.UIElements;
#endif

public class BlackboardEntryFloat : BlackboardEntry
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

    [SerializeField] private float b_value = 0f;
    public override object B_value
    {
        get
        {
            return b_value;
        }
        set
        {
            if (value.GetType() == typeof(float))
                b_value = (float)value;
        }
    }
    public override string B_typeName
    {
        get { return "Float"; }
    }

#if UNITY_EDITOR
    private FloatField field;

    public override VisualElement BuildFieldView()
    {
        field = new FloatField("value:");
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