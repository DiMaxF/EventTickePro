using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NotificationsScreen : AppScreen
{
    [SerializeField] ToggleView masterToggle;
    [SerializeField] ListView notificationsList;
    [SerializeField] ButtonView save;
    [SerializeField] ButtonView back;
    protected override void OnStart()
    {
        base.OnStart();
        //ui.InitView(masterToggle, core.Notification);
        UIContainer.SubscribeToView<ToggleView, bool>(masterToggle, OnMasterToggle); 
        UIContainer.SubscribeToView<ListView, EventModel>(notificationsList, OnListAction);
        UIContainer.SubscribeToView<ButtonView, object>(save, _ => Save());
        UIContainer.SubscribeToView<ButtonView, object>(back, _ => Back());
        //UpdateNotifications().Forget();
    }
    private void Back()
    {
        Data.DiscardChanges();
        //container.Show<HomeScreen>();
    }
    protected override void UpdateViews()
    {
        base.UpdateViews();
        //ui.InitView(notificationsList, data.GetNotificationsEvents());
    }
    private void OnMasterToggle(bool value)
    {
        //core.Notification = value;
    }

    private void OnListAction(EventModel model)
    {
        
    }


    private void Save()
    {
        Data.SaveData();
        
    }

   /* private async UniTask UpdateNotifications()
    {
        if (core.Notification)
        {
            await NotificationManager.Instance.ScheduleNotificationsAsync(data.GetNotificationsEvents());
        }
        else
        {
            NotificationManager.Instance.ClearAllNotifications();
        }
    }*/
}
