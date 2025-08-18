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
        Data.Personal.SetSelectedEvent(null);
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
        UIContainer.InitView(events, Data.Events.GetAll());
    }

    private void OnButtonAddEvent() 
    {
        Container.Show<AddEventScreen>();
    }

    private void OnEventsAction(EventModel model)
    {
        Data.Personal.SetSelectedEvent(model);
        Data.SaveData();
        Container.Show<EventScreen>();
    }
}
