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
    [Header("Time")]
    [SerializeField] TimePickerView timePicker;
    [SerializeField] ButtonView timeOpen;
    [Header("Inputs")]
    [SerializeField] InputTextView name;
    [SerializeField] InputTextView venue;
    [SerializeField] InputTextView description;
    

    private EventModel model;

    protected override void OnStart()
    {
        model = new EventModel(DateTime.Now.ToString(DateTimeUtils.Format), DateTime.Now.ToString(DateTimeUtils.TimeFormat), "");
        base.OnStart();
        UIContainer.RegisterView(datePicker);
        UIContainer.RegisterView(timePicker);
        datePicker.Hide();
        timePicker.Hide();
        UIContainer.InitView(image, "");

    }
    protected override void Subscriptions()
    {
        base.Subscriptions();
        UIContainer.SubscribeToView<ButtonView, object>(create, _ => OnButtonCreate());
        UIContainer.SubscribeToView<ButtonView, object>(addImage, _ => OnButtonGallery());
        UIContainer.SubscribeToView<ButtonView, object>(back, _ => Container.Show<HomeScreen>());

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
        UIContainer.InitView(dateOpen, model.date.ToString());
        UIContainer.InitView(timeOpen, model.time.ToString());
        UIContainer.InitView(name, model.name);
        UIContainer.InitView(venue, model.venue);
        UIContainer.InitView(image, model.imgPath);
        ValidateModel();
    }

    private void OnNameEdit(string val) 
    {
        model.name = name.text;
        ValidateModel();
    }

    private void OnVenueEdit(string val)
    {
        model.venue = venue.text;
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
        model.name = name.text;
        model.venue = venue.text;
        model.description = description.text;

        Data.Events.Add(model);
        Data.SaveData();
        Container.Show<HomeScreen>();
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
        Logger.Log($"time: {time}", "TimeSave");
        if (TimeSpan.TryParse(time, out var t))
        {
            model.time = time;
        }
        timePicker.Hide();
        UpdateViews();
    }

    private void OnButtonGallery() 
    {
#if UNITY_WEBGL && !UNITY_EDITOR
    CrossplatformUtilsManager.PickFile((base64Data) =>
    {
        if (!string.IsNullOrEmpty(base64Data))
        {
            Debug.Log($"Received base64 data: {base64Data.Substring(0, Math.Min(base64Data.Length, 50))}...");
            var selectedImagePath = FileManager.SaveImage(base64Data, isBase64: true);
            if (!string.IsNullOrEmpty(selectedImagePath))
            {
                UIContainer.InitView(image, selectedImagePath);
                model.imgPath = selectedImagePath;
            }
            else
            {
                Debug.LogError("Failed to save image, selectedImagePath is null");
            }
        }
        else
        {
            Debug.LogWarning("No file selected or empty base64 data");
        }
    }, "image/*");
#else
        NativeGallery.GetImageFromGallery((path) =>
        {
            if (!string.IsNullOrEmpty(path))
            {
                var selectedImagePath = FileManager.SaveImage(path);
                if (!string.IsNullOrEmpty(selectedImagePath))
                {
                    UIContainer.InitView(image, selectedImagePath);
                    model.imgPath = selectedImagePath;
                }
                else
                {
                    Debug.LogError("Failed to save image, selectedImagePath is null");
                }
            }
            else
            {
                Debug.LogWarning("No file selected");
            }
        }, "Select Image", "image/*");
#endif
    }

    private void ValidateModel() 
    {
        name.DefaultColor();
        venue.DefaultColor();
        if (model.name == "" || model.venue == "")
        {
            if (model.name == "")name.HighlightError();
            if (model.venue == "") venue.HighlightError();

            create.interactable = false;
            return;
        }
        create.interactable = true;

    }
}
