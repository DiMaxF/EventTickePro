using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HomeScreen : AppScreen
{
    [SerializeField] ListView events;
    [SerializeField] ButtonView addEvent;


    protected override void OnStart()
    {
        base.OnStart();
        data.selectedEvent = null;
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
        UIContainer.InitView(events, data.events);
    }

    private void OnButtonAddEvent() 
    {
        container.Show<AddEventScreen>();
    }

    private void OnEventsAction(EventModel model)
    {
        data.selectedEvent = model;
        core.SaveData();
        container.Show<EventScreen>();
    }
}
