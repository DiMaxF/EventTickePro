using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EventListView : View
{
    [SerializeField] AsyncImageView image;
    [SerializeField] Text name;
    [SerializeField] Text dateTime;
    [SerializeField] Text description;
    [SerializeField] ButtonView action;

    EventModel _event;

    public override void UpdateUI()
    {
        base.UpdateUI();
        UIContainer.RegisterView(image);
        image.widthFill = true;
        UIContainer.InitView(image, _event.imgPath);
        dateTime.text = $"{_event.date} {_event.time}";
        description.text = $"{_event.description}";
        name.text = $"{_event.name}";
    }

    public override void Init<T>(T data)
    {
        if (data is EventModel model) _event = model;
        base.Init(data);
    }

    public override void Subscriptions()
    {
        base.Subscriptions();
        UIContainer.SubscribeToView<ButtonView, object>(action, _ => TriggerAction(_event));
    }
}
