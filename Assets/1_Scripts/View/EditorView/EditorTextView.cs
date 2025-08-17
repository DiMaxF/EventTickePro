using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EditorTextView : EditorView
{
    [SerializeField] InputTextView value;

    string _val;

    public override void Show()
    {
        base.Show();
        UIContainer.RegisterView(value);
        UIContainer.SubscribeToView<InputTextView, string>(value, val => _val = val);
    }

    public string Text => value.text;
    public override void UpdateUI()
    {
        base.UpdateUI();
    }

    public override void Init<T>(T data)
    {
        if (data is string s) 
        {
            UIContainer.RegisterView(value);
            UIContainer.InitView(value, s);
        }
        base.Init(data);
    }

    public override void Select()
    {
        base.Select();
        value.interactable = true;
    }
    public override void Deselect()
    {
        base.Deselect();
        value.interactable = false;
    }

    public override void UpdateColor(Color newColor)
    {
        base.UpdateColor(newColor);
        value.UpdateColor(newColor);
    }
}
