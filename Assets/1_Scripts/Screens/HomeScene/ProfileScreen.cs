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
        UIContainer.InitView(button, "Update profile");
    }

    protected override void UpdateViews()
    {
        base.UpdateViews();
        UIContainer.InitView(name, Data.Personal.GetName());
        UIContainer.InitView(phone, Data.Personal.GetPhone());
        UIContainer.InitView(email, Data.Personal.GetEmail());

        UIContainer.InitView(sales, Data.Personal.GetNotificationsSales());
        UIContainer.InitView(eventsUpdates, Data.Personal.GetNotificationsEvents());
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
        Data.Personal.SetNotificationsSales(val);
        Data.SaveData();
    }

    private void OnToggleEvents(bool val)
    {
        Data.Personal.SetNotificationsEvents(val);
        Data.SaveData();
    }


    private void OnButton() 
    {
        if (editable) 
        {
            if (name.isValid && phone.isValid && email.isValid) 
            {
                SetEditable(false);
                Data.Personal.SetName(name.text);
                Data.Personal.SetPhone(phone.text);
                Data.Personal.SetEmail(email.text);
                UIContainer.InitView(button, "Update profile");
                Data.SaveData();
            }

        }
        else
        {
            SetEditable(true);
            email.DefaultColor();
            phone.DefaultColor();
            name.DefaultColor();
            UIContainer.InitView(button, "Save");

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
