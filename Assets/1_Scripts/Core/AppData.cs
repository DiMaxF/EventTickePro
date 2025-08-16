using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class AppData
{

    public EventModel selectedEvent;
    public List<EventModel> events;
    public List<MapData> maps;
    public string name;
    public string phone;
    public string email;
    public bool notificationsSales = true;
    public bool notificationsEvents = true;
    public TicketModel FindTicket(EmailModel email) 
    {
        return AllTickets().Where(t => t.contacts.email == email.email && t.contacts.name == email.name ).FirstOrDefault();
    }
    public List<TicketModel> AllTickets() 
    {
        var list = new List<TicketModel>();
        foreach (var ev in events)
        {
            list.AddRange(ev.tickets);
        }
        return list;
    }

   public void AddTicket(EventModel ev, TicketModel ticket) 
    {
        ev.tickets.Add(ticket);
        events.Remove(ev);
        events.Add(ev); 
    }

    public EventModel GetEventByTicket(TicketModel ticket) 
    {
        return events.Where(ev => ev.tickets.Contains(ticket)).FirstOrDefault();    
    }
    public MapData GetByEvent(EventModel model) 
    {
        foreach (var map in maps)
        {
            if (map.Event.date == model.date && map.Event.time == model.time)
            {
                return map;
            }
        }
        return null;
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

        return events.Where(ev => ev.Date >= startDate && ev.Date <= now).ToList();
    }

    // 1. KPIs: Количество посещений и процент заполняемости
    public ChartData GetKPIs(TimePeriod period)
    {
        var filteredEvents = FilterEventsByPeriod(period);
        var values = new Dictionary<string, int>();

        foreach (var ev in filteredEvents)
        {
            // Количество использованных билетов (посещений)
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

        // Процент заполняемости можно добавить как отдельный график, если нужно
        return new ChartData("Visits", values.OrderBy(kvp => DateTime.Parse(kvp.Key)).ToDictionary(kvp => kvp.Key, kvp => kvp.Value));
    }

    // 2. Ticket Sales: График продаж билетов по дням, месяцам или годам
    public ChartData GetTicketSalesByPeriod(TimePeriod period)
    {
        var sales = new Dictionary<string, int>();
        var filteredEvents = FilterEventsByPeriod(period);

        foreach (var ticket in AllTickets())
        {
            var ev = GetEventByTicket(ticket);
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

    // 3. Conversion Rate: Соотношение отправленных и использованных билетов
    public ChartData GetConversionRates(TimePeriod period)
    {
        var filteredEvents = FilterEventsByPeriod(period);
        var values = new Dictionary<string, int>();

        foreach (var ev in filteredEvents)
        {
            // Количество отправленных билетов
            int sentTickets = ev.tickets.Count;
            // Количество использованных билетов
            int usedTickets = ev.tickets.Count(ticket => ticket.valid);
            // Процент конверсии (использованные / отправленные * 100)
            int conversionRate = sentTickets > 0 ? (int)((usedTickets / (float)sentTickets) * 100) : 0;

            string key = period == TimePeriod.Year ? ev.Date.ToString("MMM yyyy") : ev.Date.ToString("dd.MM.yyyy");

            if (values.ContainsKey(key))
            {
                values[key] = (values[key] + conversionRate) / 2; // Средняя конверсия для одинаковых дат
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
        var filteredEvents = FilterEventsByPeriod(period);
        int totalUsedTickets = 0;
        int totalSeats = 0;

        foreach (var ev in filteredEvents)
        {
            totalUsedTickets += ev.tickets.Count(ticket => ticket.valid);
            if(ev.seats != null) totalSeats += ev.seats.Count; 
        }

        if (totalSeats == 0)
        {
            return 0f;
        }

        return (float)totalUsedTickets / totalSeats;
    }
}
