using System;
using UnityEngine;
using UnityEngine.UI;

public class InputTextView : View
{
    [SerializeField] private InputField inputField;
    [SerializeField] private string format;
    [SerializeField] private Image backgroundImage;
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color errorColor = Color.red;
    [SerializeField] private string errorMessage = "Field cannot be empty";

    public void Lock()
    {
        inputField.interactable = false;

    }

    public void Unlock()
    {
        inputField.interactable = true;
    }

    private Func<string, bool> _validationRule;

    public string Text => inputField != null ? inputField.text : "";


    public void Init(string initialText, Func<string, bool> validationRule = null)
    {
        _validationRule = validationRule ?? DefaultValidationRule;
        if (inputField != null)
        {
            inputField.onValueChanged.AddListener((val) => 
            {
                ValidateInput(val);
                TriggerAction(val);
            });

            inputField.text = FormatText(initialText) ?? "";
            ValidateInput(initialText);
        }
        UpdateUI();
    }

    public override void Init<T>(T data)
    {
        if (data is string text)
        {
            Init(text);
        }
    }

    public override void UpdateUI()
    {
        ValidateInput(inputField != null ? inputField.text : "");
        if(format != "") 
        {
            inputField.text = FormatText(inputField.text);
        }
    }


    private void ValidateInput(string text)
    {
        backgroundImage.color = _validationRule(text) ? normalColor : errorColor;
    }
    public void HighLightError() 
    {
        backgroundImage.color = errorColor;
    }
    private bool DefaultValidationRule(string text) => !string.IsNullOrEmpty(text);
    private string FormatText(string input)
    {
        if (string.IsNullOrEmpty(format) || string.IsNullOrEmpty(input))
            return input;

        try
        {
            if (int.TryParse(input, out int number))
            {
                return number.ToString(format);
            }
            else if (float.TryParse(input, out float floatNumber))
            {
                return floatNumber.ToString(format);
            }
        }
        catch (FormatException)
        {
            Loger.Log($"Invalid format '{format}' for input '{input}'", "InputTextView");
        }
        return input;
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