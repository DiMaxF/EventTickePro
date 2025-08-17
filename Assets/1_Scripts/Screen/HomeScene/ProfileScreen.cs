using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
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
    bool editable;
    protected override void OnStart()
    {
        base.OnStart();
        SetEditable(false);
        UIContainer.InitView(button, "UPDATE PROFILE");
    }

    protected override void UpdateViews()
    {
        base.UpdateViews();
        UIContainer.InitView(name, data.Personal.GetName());
        UIContainer.InitView(phone, data.Personal.GetPhone());
        UIContainer.InitView(email, data.Personal.GetEmail());

        UIContainer.InitView(sales, data.Personal.GetNotificationsSales());
        UIContainer.InitView(eventsUpdates, data.Personal.GetNotificationsEvents());
    }

    protected override void Subscriptions()
    {
        base.Subscriptions();
        UIContainer.SubscribeToView<ToggleView, bool>(sales, OnToggleSales);
        UIContainer.SubscribeToView<ToggleView, bool>(eventsUpdates, OnToggleEvents);

        UIContainer.SubscribeToView<ButtonView, object>(button, _ => OnButton());

        UIContainer.SubscribeToView<InputTextView, string>(name, val => ValidateName(val));
        UIContainer.SubscribeToView<InputTextView, string>(phone, val => ValidatePhone(val));
        UIContainer.SubscribeToView<InputTextView, string>(email, val => ValidateEmail(val));
    }

    private void ValidateName(string val) 
    {
        if (val == "") name.HighlightError();
        else name.DefaultColor();
    }

    private void ValidatePhone(string val)
    {
        if (val == "") phone.HighlightError();
        else phone.DefaultColor();
    }

    private void ValidateEmail(string val)
    {
        if (string.IsNullOrEmpty(val) || !Regex.IsMatch(val, @"^[\w-\.]+@([\w-]+\.)+[\w-]{2,4}$"))
             email.HighlightError();
        else email.DefaultColor();
        
    }

    private void OnToggleSales(bool val)
    {
        data.Personal.SetNotificationsSales(val);
        data.SaveData();
    }

    private void OnToggleEvents(bool val)
    {
        data.Personal.SetNotificationsEvents(val);
        data.SaveData();
    }


    private void OnButton() 
    {
        if (editable) 
        {
            if (name.isValid && phone.isValid && email.isValid) 
            {
                SetEditable(false);
                data.Personal.SetName(name.text);
                data.Personal.SetPhone(phone.text);
                data.Personal.SetEmail(email.text);
                UIContainer.InitView(button, "UPDATE PROFILE");
                data.SaveData();
            }

        }
        else
        {
            SetEditable(true);
            email.DefaultColor();
            phone.DefaultColor();
            name.DefaultColor();
            UIContainer.InitView(button, "SAVE");

        }
    }


    public void SetEditable(bool val) 
    {
        name.interactable = val;
        phone.interactable = val;
        email.interactable = val;

        editable = val;
    }
}
