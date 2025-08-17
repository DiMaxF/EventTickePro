using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AnalyticsManager : IDataManager
{
    private readonly EventManager _eventManager;
    private readonly TicketManager _ticketManager;

    public AnalyticsManager(EventManager eventManager, TicketManager ticketManager)
    {
        _eventManager = eventManager ?? throw new ArgumentNullException(nameof(eventManager));
        _ticketManager = ticketManager ?? throw new ArgumentNullException(nameof(ticketManager));
    }

    public ChartData GetKPIs(TimePeriod period)
    {
        var filteredEvents = _eventManager.FilterEventsByPeriod(period);
        var values = new Dictionary<string, int>();
        foreach (var ev in filteredEvents)
        {
            int visits = ev.tickets.Count(ticket => ticket.valid);
            string key = period == TimePeriod.Year ? ev.Date.ToString("MMM yyyy") : ev.Date.ToString("dd.MM.yyyy");

            if (values.ContainsKey(key))
            {
                values[key] += visits;
            }
            else
            {
                values[key] = visits;
            }
        }

        return new ChartData("Visits", values.OrderBy(kvp => DateTime.Parse(kvp.Key)).ToDictionary(kvp => kvp.Key, kvp => kvp.Value));
    }

    public ChartData GetTicketSalesByPeriod(TimePeriod period)
    {
        var sales = new Dictionary<string, int>();
        var filteredEvents = _eventManager.FilterEventsByPeriod(period);
        foreach (var ticket in _ticketManager.AllTickets())
        {
            var ev = _eventManager.GetEventByTicket(ticket);
            if (ev == null || !filteredEvents.Contains(ev)) continue;

            string key = period switch
            {
                TimePeriod.Year => ev.Date.ToString("MMM yyyy"),
                TimePeriod.Month => ev.Date.ToString("dd.MM.yyyy"),
                TimePeriod.Week => ev.Date.ToString("dd.MM.yyyy"),
                _ => ev.Date.ToString("dd.MM.yyyy")
            };

            if (sales.ContainsKey(key))
            {
                sales[key]++;
            }
            else
            {
                sales[key] = 1;
            }
        }

        return new ChartData("Sales", sales.OrderBy(kvp => DateTime.Parse(kvp.Key)).ToDictionary(kvp => kvp.Key, kvp => kvp.Value));
    }

    public ChartData GetConversionRates(TimePeriod period)
    {
        var filteredEvents = _eventManager.FilterEventsByPeriod(period);
        var values = new Dictionary<string, int>();
        foreach (var ev in filteredEvents)
        {
            int sentTickets = ev.tickets.Count;
            int usedTickets = ev.tickets.Count(ticket => ticket.valid);
            int conversionRate = sentTickets > 0 ? (int)((usedTickets / (float)sentTickets) * 100) : 0;

            string key = period == TimePeriod.Year ? ev.Date.ToString("MMM yyyy") : ev.Date.ToString("dd.MM.yyyy");

            if (values.ContainsKey(key))
            {
                values[key] = (values[key] + conversionRate) / 2;
            }
            else
            {
                values[key] = conversionRate;
            }
        }

        return new ChartData("Convertion (%)", values.OrderBy(kvp => DateTime.Parse(kvp.Key)).ToDictionary(kvp => kvp.Key, kvp => kvp.Value));
    }

    public float GetPersentOccupancy(TimePeriod period)
    {
        var filteredEvents = _eventManager.FilterEventsByPeriod(period);
        int totalUsedTickets = 0;
        int totalSeats = 0;
        foreach (var ev in filteredEvents)
        {
            totalUsedTickets += ev.tickets.Count(ticket => ticket.valid);
            if (ev.seats != null) totalSeats += ev.seats.Count;
        }

        return totalSeats == 0 ? 0f : (float)totalUsedTickets / totalSeats;
    }
}