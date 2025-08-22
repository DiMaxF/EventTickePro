using Cysharp.Threading.Tasks;
using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NavigationBarView : View
{
    [SerializeField] private ListView buttons;


    List<AppContainer.NavigationButtonData> _data;

    public override void Init<T>(T data)
    {
        if (data is List<AppContainer.NavigationButtonData> updateData) 
        {
            Logger.Log($"Init: {updateData.Count}", "NavigationBarView");
            
            _data = updateData;

            if (!buttons.isGenerated) 
            {
                UIContainer.RegisterView(buttons, true);
                UIContainer.InitView(buttons, _data);
                buttons.UpdateViewsData(_data);
            }
            else
            {
                buttons.UpdateViewsData(_data);
            }
        }
        base.Init(data);
    }

    public override void Subscriptions()
    {
        base.Subscriptions();

        UIContainer.SubscribeToView<ListView, object>(buttons, selected =>
        {
            TriggerAction(selected);
            Logger.Log($"Selected data: {selected}", "NavigationBarView");
        }, true);

    }

    public override void Show()
    {
        if (IsActive) return;
        base.Show();
    }

    public override void Hide()
    {
        if (!IsActive) return;
        base.Hide();
    }

}