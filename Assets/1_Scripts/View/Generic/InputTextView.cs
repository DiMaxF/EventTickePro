using System;
using UnityEngine;
using UnityEngine.UI;

public class InputTextView : View
{
    [SerializeField] InputField inputField;
    [SerializeField] Image outline;
    [SerializeField] Color normalColor;
    [SerializeField] Color errorColor;

    public string text => inputField != null ? inputField.text : "";
    public bool interactable
    {
        get => inputField.interactable;
        set => inputField.interactable = value;
    }
    public bool isValid => outline.color != errorColor;

    public override void Init<T>(T data)
    {
        if (data != null) inputField.text = data.ToString();
        base.Init<T>(data);
    }

    public override void Subscriptions()
    {
        base.Subscriptions();
        inputField.onValueChanged.AddListener((string text) =>
        {
            TriggerAction(text);
            UpdateUI();

        });
    }


    public void Lock(bool value)
    {
        inputField.interactable = value;
    }
    public void HighlightError()
    {
        outline.color = errorColor;
    }

    public void DefaultColor()
    {
        outline.color = normalColor;
    }
    public void UpdateColor(Color color)
    {
        inputField.colors = new ColorBlock
        {
            normalColor = color,
            highlightedColor = color,
            pressedColor = color,
            selectedColor = color,
            disabledColor = color
        };
        inputField.placeholder.color = color;
        inputField.textComponent.color = color;
    }
}
