using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EditorTextView : EditorView
{
    [SerializeField] InputTextView name;

    string _val;

    public override void Show()
    {
        base.Show();
        UIContainer.RegisterView(name);
        UIContainer.SubscribeToView<InputTextView, string>(name, val => _val = val);
    }

    public string Text => name.Text;
    public override void UpdateUI()
    {
        base.UpdateUI();
    }

    public override void Init<T>(T data)
    {
        if (data is string s) 
        {
            UIContainer.RegisterView(name);
            UIContainer.InitView(name, s);
        }
        base.Init(data);
    }

    public override void Select()
    {
        base.Select();
        name.Unlock();
    }
    public override void Deselect()
    {
        base.Deselect();
        name.Lock();
    }

    public override void UpdateColor(Color newColor)
    {
        base.UpdateColor(newColor);
        name.UpdateColor(newColor);
    }
}
