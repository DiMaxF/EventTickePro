using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AddEventScreen : AppScreen
{
    [Header("Buttons")]
    [SerializeField] ButtonView back;
    [SerializeField] ButtonView create;
    [Header("Image Input")]
    [SerializeField] ButtonView addImage;
    [SerializeField] AsyncImageView image;
    [Header("Date")]
    [SerializeField] DatePickerView datePicker;
    [SerializeField] ButtonView dateOpen;
    [SerializeField] Text selectedDate;
    [Header("Time")]
    [SerializeField] TimePickerView timePicker;
    [SerializeField] ButtonView timeOpen;
    [SerializeField] Text selectedTime;
    [Header("Inputs")]
    [SerializeField] InputTextView name;
    [SerializeField] InputTextView venue;
    [SerializeField] InputTextView description;
    

    private EventModel model;

    protected override void OnStart()
    {
        model = new EventModel(DateTime.Now.ToString(DateFormatter.Format), DateTime.Now.ToString(DateFormatter.TimeFormat), "");
        base.OnStart();
        UIContainer.RegisterView(datePicker);
        UIContainer.RegisterView(timePicker);
        datePicker.Hide();
        timePicker.Hide();

        UIContainer.InitView(name, model.name);
        UIContainer.InitView(venue, model.venue);
        addImage.gameObject.SetActive(true);
    }
    protected override void Subscriptions()
    {
        base.Subscriptions();
        UIContainer.SubscribeToView<ButtonView, object>(create, _ => OnButtonCreate());
        UIContainer.SubscribeToView<ButtonView, object>(addImage, _ => OnButtonGallery());
        UIContainer.SubscribeToView<ButtonView, object>(back, _ => container.Show<HomeScreen>());

        UIContainer.SubscribeToView<ButtonView, object>(dateOpen, _ => OnButtonDate());
        UIContainer.SubscribeToView<ButtonView, object>(timeOpen, _ => OnButtonTime());



        UIContainer.SubscribeToView<DatePickerView, string>(datePicker, OnDateSave);
        UIContainer.SubscribeToView<TimePickerView, string>(timePicker, OnTimeSave);

        UIContainer.SubscribeToView<InputTextView, string>(name, OnNameEdit);
        UIContainer.SubscribeToView<InputTextView, string>(venue, OnVenueEdit);


    }
    protected override void UpdateViews()
    {
        base.UpdateViews();
        selectedTime.text = model.time;
        selectedDate.text = model.date;
        ValidateModel();
    }

    private void OnNameEdit(string val) 
    {
        model.name = name.Text;
        ValidateModel();
    }

    private void OnVenueEdit(string val)
    {
        model.venue = venue.Text;
        ValidateModel();
    }

    private void OnButtonDate()
    {
        datePicker.Show();
        UIContainer.InitView(datePicker, model.date);
        ValidateModel();
    }

    private void OnButtonTime()
    {
        timePicker.Show();
        UIContainer.InitView(timePicker, model.time);
        ValidateModel();
    }

    private void OnButtonCreate()
    {
        model.name = name.Text;
        model.venue = venue.Text;
        model.description = description.Text;

        data.events.Add(model);
        core.SaveData();
        container.Show<HomeScreen>();
    }

    private void OnDateSave(string date)
    {
        if (DateTime.TryParse(date, out DateTime endDate))
        {
            model.date = date;
        }
        datePicker.Hide();
        UpdateViews();
    }

    private void OnTimeSave(string time)
    {
        Loger.Log($"time: {time}", "TimeSave");
        if (TimeSpan.TryParse(time, out var t))
        {
            model.time = time;
        }
        timePicker.Hide();
        UpdateViews();
    }

    private void OnButtonGallery() 
    {
        NativeGallery.GetImageFromGallery((path) =>
        {
            if (!string.IsNullOrEmpty(path))
            {
                var selectedImagePath = FileManager.SaveImage(path);
                UIContainer.InitView(image, selectedImagePath);
                model.imgPath = selectedImagePath;
                addImage.gameObject.SetActive(false);
            }
        }, "Select Image", "image/*");
    }

    private void ValidateModel() 
    {
        
        if(model.name == "")
        {
            name.HighLightError();
            create.interactable = false;
        }
        else if(model.venue == "") 
        {
            venue.HighLightError();
            create.interactable = false;
        }
        else
    {
        create.interactable = true;
    }

    }
}
