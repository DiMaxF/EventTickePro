using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HomeScreen : AppScreen
{
    [SerializeField] ListView events;
    [SerializeField] Button addEvent;


    protected override void OnStart()
    {
        base.OnStart();
        data.selectedEvent = null;
    }
    protected override void Subscriptions()
    {
        base.Subscriptions();
        ui.SubscribeToComponent<Button, object>(addEvent, _ => OnButtonAddEvent());
        ui.SubscribeToView<ListView, EventModel>(events, OnEventsAction);
    }

    protected override void UpdateViews()
    {
        base.UpdateViews();
        ui.InitView(events, data.events);
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
