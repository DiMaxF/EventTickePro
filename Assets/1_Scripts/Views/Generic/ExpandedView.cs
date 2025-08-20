using Cysharp.Threading.Tasks;
using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class ExpandedView : View
{
    [SerializeField] View[] views;
    [SerializeField] ButtonView expand;
    [SerializeField] float spawnDelayPerItem;

    bool _active;

    private RectTransform _rectTransform; 
    VerticaUpdater _updater;
    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
        if (_updater == null) _updater = GetComponentInParent<VerticaUpdater>();
        InstantHide();
    }
    public override void Init<T>(T data)
    {
        if (data is bool active) 
        {
            _active = active;
        }

        base.Init(data);
    }

    public override void Subscriptions()
    {
        base.Subscriptions();
        UIContainer.SubscribeToView<ButtonView, object>(expand, _ => ToggleExpand());
    }

    private void ToggleExpand()
    {
        _active = !_active;
        AnimateExpand();
    }



    private async UniTask AnimateExpand()
    {
        if (_active)
        {
            Show();
            await UniTask.WaitForSeconds(0.3f);

            await AnimateItemsSpawn(false);
        }
        else
        {
            await AnimateItemsSpawn(true);
            await UniTask.WaitForSeconds(0.3f);

            Hide();
        }

        if (_updater != null) _updater.UpdateSpacing();
    }

    override public async void Hide()
    {
        await AnimationPlayer.PlayAnimationsAsync(gameObject, false);
    }

    private async UniTask AnimateItemsSpawn(bool reverse)
    {
        if (reverse)
        {
            for (int i = views.Length - 1; i >= 0; i--)
            {
                await SetVisibleView(views[i]);
            }
        }
        else
        {
            for (int i = 0; i < views.Length; i++)
            {
                await SetVisibleView(views[i]);
            }
        }
    }

    private async UniTask SetVisibleView(View view) 
    {
        await UniTask.Delay(TimeSpan.FromSeconds(spawnDelayPerItem), cancellationToken: this.GetCancellationTokenOnDestroy());
        if (_active) view.Show();
        else view.Hide();
    } 

    private void InstantHide() 
    {
        foreach (var view in views) view.gameObject.SetActive(false);
    }
}
