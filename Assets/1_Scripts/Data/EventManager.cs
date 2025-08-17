using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EventManager : IDataManager
{
    private readonly AppData _appData;

    public EventManager(AppData appData)
    {
        _appData = appData ?? throw new ArgumentNullException(nameof(appData));
    }


    public void Add(EventModel model) 
    {
        _appData.events.Add(model);
    }

    public List<EventModel> FilterEventsByPeriod(TimePeriod period)
    {
        DateTime now = DateTime.Now;
        DateTime startDate;
        switch (period)
        {
            case TimePeriod.Week:
                startDate = now.AddDays(-7);
                break;
            case TimePeriod.Month:
                startDate = now.AddDays(-30);
                break;
            case TimePeriod.Year:
                startDate = now.AddDays(-365);
                break;
            default:
                startDate = DateTime.MinValue;
                break;
        }

        return _appData.events.Where(ev => ev.Date >= startDate && ev.Date <= now).ToList();
    }

    public EventModel GetEventByTicket(TicketModel ticket)
    {
        return _appData.events.FirstOrDefault(ev => ev.tickets.Contains(ticket));
    }

    public List<EventModel> GetAll() => _appData.events;
}