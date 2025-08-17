using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HomeScreen : AppScreen
{
    [SerializeField, Space(20)] ListView events;
    [SerializeField] ButtonView addEvent;

    protected override void OnStart()
    {
        base.OnStart();
        data.Personal.SetSelectedEvent(null);
    }

    protected override void Subscriptions()
    {
        base.Subscriptions();
        UIContainer.SubscribeToView<ButtonView, object>(addEvent, _ => OnButtonAddEvent());
        UIContainer.SubscribeToView<ListView, EventModel>(events, OnEventsAction);
    }

    protected override void UpdateViews()
    {
        base.UpdateViews();
        UIContainer.InitView(events, data.Events.GetAll());
    }

    private void OnButtonAddEvent() 
    {
        container.Show<AddEventScreen>();
    }

    private void OnEventsAction(EventModel model)
    {
        data.Personal.SetSelectedEvent(model);
        data.SaveData();
        container.Show<EventScreen>();
    }
}
