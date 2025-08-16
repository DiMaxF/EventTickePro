using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NotificationsScreen : AppScreen
{
    [SerializeField] ToggleView masterToggle;
    [SerializeField] ListView notificationsList;
    [SerializeField] Button save;
    [SerializeField] Button back;
    protected override void OnStart()
    {
        base.OnStart();
        //ui.InitView(masterToggle, core.Notification);
        ui.SubscribeToView<ToggleView, bool>(masterToggle, OnMasterToggle); 
        ui.SubscribeToView<ListView, EventModel>(notificationsList, OnListAction);
        ui.SubscribeToComponent<Button, object>(save, _ => Save());
        ui.SubscribeToComponent<Button, object>(back, _ => Back());
        //UpdateNotifications().Forget();
    }
    private void Back()
    {
        core.DiscardChanges();
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
        core.SaveData();
        
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
