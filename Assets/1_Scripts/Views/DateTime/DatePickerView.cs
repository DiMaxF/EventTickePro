using DG.Tweening;
using System;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;

public class DatePickerView : View
{
    [SerializeField] CalendarManager calendar;
    [SerializeField] ButtonView cancel;
    [SerializeField] ButtonView save;
    [SerializeField] AnimationConfig fadeIn;
    private CanvasGroup canvasGroup;
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
    public override void Show()
    {
        base.Show();
        /*canvasGroup.alpha = 0f;
        StartAnimation().Append(canvasGroup.DOFade(1f, fadeIn.Duration).SetEase(fadeIn.Ease));*/
    }
    private void Awake()
    {
        canvasGroup = gameObject.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }
    }
}