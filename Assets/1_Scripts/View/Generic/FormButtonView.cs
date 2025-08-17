using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FormButtonView : View
{
    [SerializeField] Image image;
    [SerializeField] ButtonView action;
    [SerializeField] GameObject selected;

    [Serializable]
    public class Data
    {
        public Sprite sprite;
        public bool active;

        public Data(Sprite sprite, bool active)
        {
            this.sprite = sprite;
            this.active = active;
        }
    }
    [SerializeField] Data _data;
    public override void Init<T>(T data)
    {
        if (data is Data d)
        {
            _data = d;
            UIContainer.SubscribeToView<ButtonView, object>(action, _ => TriggerAction(_data.sprite));
        }

        base.Init(data);
    }

    public override void UpdateUI()
    {
        base.UpdateUI();
        image.sprite = _data.sprite;
        image.type = Image.Type.Sliced; 
        selected.SetActive(_data.active);
    }
}
