using System;

public class PersonalManager : IDataManager
{
    private readonly AppData _appData;

    public PersonalManager(AppData appData)
    {
        _appData = appData ?? throw new ArgumentNullException(nameof(appData));
    }

    public EventModel GetSelectedEvent()
    {
        return _appData.selectedEvent;
    }

    public void SetSelectedEvent(EventModel eventModel)
    {
        _appData.selectedEvent = eventModel;
        Logger.Log($"Selected event changed to: {eventModel?.name ?? "null"}", "PersonalManager");
    }

    public string GetName()
    {
        return _appData.name;
    }

    public void SetName(string name)
    {
        if (string.IsNullOrEmpty(name))
        {
            Logger.LogWarning("Attempted to set empty or null name", "PersonalManager");
            return;
        }
        _appData.name = name;
        Logger.Log($"Name changed to: {name}", "PersonalManager");
    }

    public string GetPhone()
    {
        return _appData.phone;
    }

    public void SetPhone(string phone)
    {
        if (string.IsNullOrEmpty(phone))
        {
            Logger.LogWarning("Attempted to set empty or null phone", "PersonalManager");
            return;
        }
        _appData.phone = phone;
        Logger.Log($"Phone changed to: {phone}", "PersonalManager");
    }

    public string GetEmail()
    {
        return _appData.email;
    }

    public void SetEmail(string email)
    {
        if (string.IsNullOrEmpty(email))
        {
            Logger.LogWarning("Attempted to set empty or null email", "PersonalManager");
            return;
        }
        _appData.email = email;
        Logger.Log($"Email changed to: {email}", "PersonalManager");
    }

    public bool GetNotificationsSales()
    {
        return _appData.notificationsSales;
    }

    public void SetNotificationsSales(bool enabled)
    {
        _appData.notificationsSales = enabled;
        Logger.Log($"NotificationsSales set to: {enabled}", "PersonalManager");
    }

    public bool GetNotificationsEvents()
    {
        return _appData.notificationsEvents;
    }

    public void SetNotificationsEvents(bool enabled)
    {
        _appData.notificationsEvents = enabled;
        Logger.Log($"NotificationsEvents set to: {enabled}", "PersonalManager");
    }

    public void UpdatePersonalData(string name, string phone, string email)
    {
        SetName(name);
        SetPhone(phone);
        SetEmail(email);
        Logger.Log("Personal data updated", "PersonalManager");
    }

    public void ClearPersonalData()
    {
        _appData.selectedEvent = null;
        _appData.name = null;
        _appData.phone = null;
        _appData.email = null;
        _appData.notificationsSales = true; 
        _appData.notificationsEvents = true;
    }
}