using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class EventModel 
{
    public string name;
    public string date;
    public string time;
    public bool notification;
    public string description;
    public string venue;
    public string imgPath;
    public List<SeatModel> seats;
    public List<TicketModel> tickets; 

    public EventModel(string date, string time, string name, string venue = "", string description = "")
    {
        this.date = date;
        this.time = time;
        this.name = name;
        this.venue = venue;
        this.description = description;
        notification = false;
        tickets = new List<TicketModel>();
    }

    public bool DateCorrect()
    {
        return TryParseDate(this.date, out _);
    }
    private bool TryParseDate(string dateString, out DateTime result)
    {
        if (DateTime.TryParseExact(dateString, DateTimeUtils.Format, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out result))
        {
            return true;
        }
        return false;
    }
    public TimeSpan Time
    {
        get
        {
            if (TimeSpan.TryParse(time, out var parsedTime))
            {
                return parsedTime;
            }
            return TimeSpan.Zero; 
        }
        set
        {
            time = value.ToString(DateTimeUtils.TimeFormat); 
        }
    }
    public DateTime Date
    {
        get
        {
            if (TryParseDate(this.date, out var parsedDate))
            {
                return parsedDate;
            }
            return DateTime.Now;
        }
        set
        {
            date = value.ToString(DateTimeUtils.Format);
        }
    }

    public SeatModel GetFreeSeat()
    {
        if (seats == null || seats.Count == 0)
        {
            return null;
        }
        foreach (var seat in seats)
        {
            if (!seat.isTaken) return seat;
        }
        return null;
    }   
}
