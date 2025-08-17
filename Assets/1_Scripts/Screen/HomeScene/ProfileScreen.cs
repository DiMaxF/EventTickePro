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
    [SerializeField] ButtonView button;
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
        UIContainer.InitView(name, data.name);
        UIContainer.InitView(phone, data.phone.ToString());
        UIContainer.InitView(email, data.email);
        UIContainer.InitView(sales, data.notificationsSales);
        UIContainer.InitView(eventsUpdates, data.notificationsEvents);
    }

    protected override void Subscriptions()
    {
        base.Subscriptions();
        UIContainer.SubscribeToView<ToggleView, bool>(sales, OnToggleSales);
        UIContainer.SubscribeToView<ToggleView, bool>(eventsUpdates, OnToggleEvents);
        UIContainer.SubscribeToView<ButtonView, object>(button, _ => OnButton());
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
            data.name = name.text;
            data.phone = phone.text;
            data.email = email.text;
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
        name.interactable = val;
        phone.interactable = val;
        phone.interactable = val;

        editable = val;
    }
}
