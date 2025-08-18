using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EditorTextView : EditorView
{
    [SerializeField] InputTextView value;

    string _val;

    public string Text => value.text;
    public override void UpdateUI()
    {
        base.UpdateUI();
        UIContainer.InitView(value, _val);
    }

    public override void Init<T>(T data)
    {
        if (data is string s) 
        {
            _val = s;
            UIContainer.RegisterView(value, true);
        }
        base.Init(data);
    }

    public override void Subscriptions()
    {
        base.Subscriptions();
        UIContainer.SubscribeToView<InputTextView, string>(value, val => _val = val);
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
