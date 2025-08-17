using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class InputTextView : View
{
    [SerializeField] InputField inputField;
    [SerializeField] Outline outline;
    [SerializeField] Color normalColor;
    [SerializeField] Color errorColor;

    public string text => inputField != null ? inputField.text : "";
    public bool interactable
    {
        get => inputField.interactable;
        set => inputField.interactable = value;
    }
    public bool isValid => outline.effectColor != errorColor;

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
        outline.effectColor = errorColor;
    }

    public void DefaultColor()
    {
        outline.effectColor = normalColor;
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
