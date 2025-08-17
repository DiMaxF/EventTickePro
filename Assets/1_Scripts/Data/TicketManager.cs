using System;
using System.Collections.Generic;
using System.Linq;

public class TicketManager : IDataManager
{
    private readonly AppData _appData;

    public TicketManager(AppData appData)
    {
        _appData = appData ?? throw new ArgumentNullException(nameof(appData));
    }

    public TicketModel FindTicket(EmailModel email)
    {
        return AllTickets().FirstOrDefault(t => t.contacts.email == email.email && t.contacts.name == email.name);
    }

    public List<TicketModel> AllTickets()
    {
        var list = new List<TicketModel>();
        foreach (var ev in _appData.events)
        {
            list.AddRange(ev.tickets);
        }
        return list;
    }

    public void AddTicket(EventModel ev, TicketModel ticket)
    {
        ev.tickets.Add(ticket);
        _appData.events.Remove(ev);
        _appData.events.Add(ev);
    }
}