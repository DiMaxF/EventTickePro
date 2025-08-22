using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NavigationButton : View
{
    [SerializeField] Image icon;
    [SerializeField] ButtonView button;

    private AppContainer.NavigationButtonData screenData;
    private bool val;
    private bool firstIteration = true;
    
    public override void Init<T>(T data)
    {
        if (data is AppContainer.NavigationButtonData screen)
        {
            screenData = screen;
        }
        base.Init(data);    
    }

    public override void Subscriptions()
    {
        base.Subscriptions();
        UIContainer.RegisterView(button, true);
        UIContainer.SubscribeToView<ButtonView, object>(button, _ => 
        {
            TriggerAction(screenData);
        }, true);
    }

    public override void UpdateUI()
    {
        if (screenData != null)
        {
            icon.sprite = screenData.icon;
        
            AnimateSelected(screenData.selected);
            val = screenData.selected;
        }
    }

    private async void AnimateSelected(bool selected) 
    {
        button.interactable = false;
        if (selected == val)
        {
            if (firstIteration) 
            {
                await AnimationPlayer.PlayAnimationsAsync(gameObject, selected);
            }
            button.interactable = true;
            return;
        }

        await AnimationPlayer.PlayAnimationsAsync(gameObject, selected);
        button.interactable = true;
    }

    public override void Show()
    {
        if(!firstIteration) base.Show();
        firstIteration = false;
    }
}