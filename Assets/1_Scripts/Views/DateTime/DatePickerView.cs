using UnityEngine;
using System.Runtime.InteropServices;
using UnityEngine.UI;
using System;

public class DatePickerView : View
{
    [SerializeField] CalendarManager calendar;
    [SerializeField] ButtonView cancel;
    [SerializeField] ButtonView save;

    string _date;
    public override void Init<T>(T data)
    {
        if (data is string date)
        {
            _date = date;
        }
        base.Init(data);
    }

    public override void Subscriptions()
    {
        base.Subscriptions();
        UIContainer.SubscribeToView<ButtonView, object>(cancel, _ => TriggerAction(""));
        UIContainer.SubscribeToView<ButtonView, object>(save, _ => TriggerAction(_date));
        calendar.OnSelect += (d) => _date = d.ToString(DateTimeUtils.Format);
    }
    public override void UpdateUI()
    {
        base.UpdateUI();
        if (DateTime.TryParse(_date, out DateTime endDate))
        {
            calendar.UpdateCalenderWithSelectedDate(endDate);
        }
        else
        {
            calendar.UpdateCalender(DateTime.Now.Year, DateTime.Now.Month);
        }

    }
}