using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.UI;

public class TicketListView : View
{
    [SerializeField] Text name;
    [SerializeField] Text seat;
    [SerializeField] Text dateTime;
    [SerializeField] GameObject valid;
    [SerializeField] ButtonView action;
    TicketModel _ticket;
    EventModel _event;

    public override void UpdateUI()
    {
        base.UpdateUI();
        name.text = _ticket.contacts.name;   
        seat.text = $"{_ticket.seat.seatId}-{_ticket.seat.count}";
        dateTime.text = $"{_event.date}, {_event.time}";
        valid.SetActive(_ticket.valid);
    }

    public override void Init<T>(T data)
    {
        if (data is TicketModel model) 
        {
            _ticket = model;
            UIContainer.SubscribeToView<ButtonView, object>(action, _ => TriggerAction(_ticket));
            _event = DataCore.Instance.Events.GetEventByTicket(_ticket);
        } 
        base.Init(data);
    }
}
