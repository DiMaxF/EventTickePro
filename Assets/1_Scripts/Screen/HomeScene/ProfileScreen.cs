using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProfileScreen : AppScreen
{
    [SerializeField] InputTextView name;
    [SerializeField] InputTextView phone;
    [SerializeField] InputTextView email;
    [SerializeField] ToggleView sales;
    [SerializeField] ToggleView eventsUpdates;
    [SerializeField] Button button;
    [SerializeField] Text buttonText;
    bool editable;
    protected override void OnStart()
    {
        base.OnStart();
        SetEditable(false);
        buttonText.text = "UPDATE PROFILE";

    }

    protected override void UpdateViews()
    {
        base.UpdateViews();
        ui.InitView(name, data.name);
        ui.InitView(phone, data.phone.ToString());
        ui.InitView(email, data.email);
        ui.InitView(sales, data.notificationsSales);
        ui.InitView(eventsUpdates, data.notificationsEvents);
    }

    protected override void Subscriptions()
    {
        base.Subscriptions();
        ui.SubscribeToView<ToggleView, bool>(sales, OnToggleSales);
        ui.SubscribeToView<ToggleView, bool>(eventsUpdates, OnToggleEvents);
        ui.SubscribeToComponent<Button, object>(button, _ => OnButton());
    }

    private void OnToggleSales(bool val)
    {
        data.notificationsSales = val;
        core.SaveData();
    }

    private void OnToggleEvents(bool val)
    {
        data.notificationsEvents = val;
        core.SaveData();
    }


    private void OnButton() 
    {
        if (editable) 
        {
            SetEditable(false);
            data.name = name.Text;
            data.phone = phone.Text;
            data.email = email.Text;
            buttonText.text = "UPDATE PROFILE";
            core.SaveData();
        }
        else
        {
            SetEditable(true);
            buttonText.text = "SAVE";
        }
    }


    public void SetEditable(bool val) 
    {
        if (val) 
        {
            name.Unlock();
            phone.Unlock();
            email.Unlock();
        }
        else
        {
            name.Lock();
            phone.Lock();
            email.Lock();
        }
        editable = val;
    }
}
