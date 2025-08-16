using UnityEngine;
using System.Runtime.InteropServices;
using UnityEngine.UI;
using System;

public class DatePickerView : View
{
    [SerializeField] CalendarManager calendar;
    [SerializeField] Button cancel;
    [SerializeField] Button save;

    string _date;
    public override void Init<T>(T data)
    {
        if (data is string date)
        {
            _date = date;
            UIContainer.SubscribeToComponent<Button, object>(cancel, _ => TriggerAction(""));
            UIContainer.SubscribeToComponent<Button, object>(save, _ => TriggerAction(_date));
            calendar.OnSelect +=(d) => _date = d.ToString(DateFormatter.Format);
        }
        base.Init(data);
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