using UnityEngine;
using UnityEngine.UIElements;

#if UNITY_EDITOR
using UnityEditor.UIElements;
#endif

public class BlackboardEntryGameObject : BlackboardEntry
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

    [SerializeField] private Object b_value;
    public override object B_value
    {
        get
        {
            return b_value;
        }
        set
        {
            b_value = value as Object;
        }
    }
    public override string B_typeName
    {
        get { return "GameObject"; }
    }

#if UNITY_EDITOR
    private ObjectField field;

    public override VisualElement BuildFieldView()
    {
        field = new ObjectField("value:");
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
