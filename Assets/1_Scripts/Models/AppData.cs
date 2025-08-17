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
    
}
