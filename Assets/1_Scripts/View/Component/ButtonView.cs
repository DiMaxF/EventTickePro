using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ButtonView : View
{
    [SerializeField] Text _text;
    private Button _button;
    private string _textButton;
    public RectTransform rect { private set; get; }
    public Image image => _button.image;
    public string Text => _text.text;
    public bool interactable
    {
        get => _button.interactable;
        set => _button.interactable = value;
    }

    private void Awake()
    {
        _button = GetComponent<Button>();
        rect = GetComponent<RectTransform>();   
        _button.onClick.AddListener(() => TriggerAction(_textButton)); 
    }

    public override void Init<T>(T data)
    {
        if (data is string text) _textButton = text;
        base.Init(data);
    }

    public override void UpdateUI()
    {
        base.UpdateUI();
        if(_text != null) _text.text = _textButton;
    }

}
