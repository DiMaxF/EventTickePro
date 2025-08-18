using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ColorButtonView : View
{
    [SerializeField] Image image;   
    [SerializeField] ButtonView action;
    [SerializeField] GameObject selected;

    [Serializable]
    public class Data 
    {
        public Color color;
        public bool active;

        public Data(Color color, bool active)
        {
            this.color = color;
            this.active = active;
        }
    }
    private Data _data;
    public override void Init<T>(T data)
    {
        if (data is Data d) 
        {
            _data= d;
        }

        base.Init(data);
    }
    public override void Subscriptions()
    {
        base.Subscriptions();
        UIContainer.SubscribeToView<ButtonView, object>(action, _ => TriggerAction(_data.color));

    }
    public override void UpdateUI()
    {
        base.UpdateUI();
        image.color = _data.color;
        selected.SetActive(_data.active);
    }
}
