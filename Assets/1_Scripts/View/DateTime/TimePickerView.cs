using UnityEngine;
using System.Runtime.InteropServices;
using System;
using UnityEngine.UI;
using UnityEngine.TerrainUtils;

public class TimePickerView : View
{

    [SerializeField] private InputTextView hours;
    [SerializeField] private InputTextView minutes;
    [SerializeField] private ButtonView cancel;
    [SerializeField] private ButtonView save;
    [SerializeField] private ButtonView pm;
    [SerializeField] private ButtonView am;
    int _hours;
    int _minutes;
    TimeSpan _time;
    private bool _isAm;
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
        SetButtonFormat(pm, !_isAm);
        SetButtonFormat(am, _isAm);
    }

    public override void Subscriptions()
    {
        base.Subscriptions();
        UIContainer.SubscribeToView<ButtonView, object>(save, _ => SaveTime());
        UIContainer.SubscribeToView<ButtonView, object>(cancel, _ => TriggerAction(""));
        UIContainer.SubscribeToView<ButtonView, object>(pm, _ => { _isAm = false; UpdateUI(); });
        UIContainer.SubscribeToView<ButtonView, object>(am, _ => { _isAm = true; UpdateUI(); });

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
            Loger.Log($"{_hours} {_minutes}", "TimeValue");
            int hours24 = _isAm ? (_hours % 12) : (_hours % 12 + 12);
            TriggerAction(new TimeSpan(hours24, _minutes, 0).ToString(DateTimeUtils.TimeFormat));
        }
    }

    private bool ValidateHours(string text)
    {
        if (int.TryParse(text, out var h))
        {
            return h >= 1 && h <= 12;
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

    private void SetButtonFormat(ButtonView btn, bool active) 
    {
        btn.image.color= active ? Color.white : Color.clear;
        btn.transform.GetChild(0).GetComponent<Text>().color = active ? Color.black : Color.white;
    }
}