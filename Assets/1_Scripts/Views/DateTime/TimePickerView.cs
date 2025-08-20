using DG.Tweening;
using System;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.TerrainUtils;
using UnityEngine.UI;

public class TimePickerView : View
{

    [SerializeField] private InputTextView hours;
    [SerializeField] private InputTextView minutes;
    [SerializeField] private ButtonView cancel;
    [SerializeField] private ButtonView save;
    [SerializeField] AnimationConfig fadeIn;
    private CanvasGroup canvasGroup;

    int _hours;
    int _minutes;
    TimeSpan _time;
    private void Awake()
    {
        canvasGroup = gameObject.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }
    }
    public override void Init<T>(T data)
    {
        
        if (data is string time)
        {
            if(TimeSpan.TryParse(time, out var val)) _time = val;
            _hours = _time.Hours;
            _minutes = _time.Minutes;
        }
        base.Init(data);
    }
    public override void UpdateUI()
    {
        base.UpdateUI();
        UIContainer.RegisterView(hours);
        UIContainer.RegisterView(minutes);
        UIContainer.InitView(hours, _time.Hours.ToString());
        UIContainer.SubscribeToView<InputTextView, string>(hours, OnHoursEdit);
        
        UIContainer.InitView(minutes, _time.Minutes.ToString());
        UIContainer.SubscribeToView<InputTextView, string>(minutes, OnMinutesEdit);
    }

    public override void Subscriptions()
    {
        base.Subscriptions();
        UIContainer.SubscribeToView<ButtonView, object>(save, _ => SaveTime());
        UIContainer.SubscribeToView<ButtonView, object>(cancel, _ => TriggerAction(""));
    }

    private void OnHoursEdit(string val) 
    {
        if(int.TryParse(val, out var h)) 
        {
            _hours = h;
        }
        ValidateTime();
    }

    private void OnMinutesEdit(string val)
    {
        if (int.TryParse(val, out var m))
        {
            _minutes = m;
        }
        ValidateTime();
    }

    private void ValidateTime()
    {
        bool isValid = ValidateHours(_hours.ToString()) && ValidateMinutes(_minutes.ToString());

        if (save != null)
        {
            save.interactable = isValid; 
        }
    }

    private void SaveTime()
    {
        if (ValidateHours(_hours.ToString()) && ValidateMinutes(_minutes.ToString()))
        {
            Logger.Log($"{_hours} {_minutes}", "TimeValue");
            TriggerAction(new TimeSpan(_hours, _minutes, 0).ToString(DateTimeUtils.TimeFormat));
        }
    }

    private bool ValidateHours(string text)
    {
        if (int.TryParse(text, out var h))
        {
            return h >= 1 && h <= 23;
        }
        return false;
    }

    private bool ValidateMinutes(string text)
    {
        if (int.TryParse(text, out var m))
        {
            return m >= 0 && m <= 59; 
        }
        return false;
    }

    public override void Show()
    {
        base.Show();
        /*canvasGroup.alpha = 0f;
        StartAnimation().Append(canvasGroup.DOFade(1f, fadeIn.Duration).SetEase(fadeIn.Ease));*/
    }
}