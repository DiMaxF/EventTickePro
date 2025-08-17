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
    [SerializeField] int defaultSize;
    [SerializeField] int expandedSize;
    [SerializeField] float spawnDelayPerItem;
    [SerializeField] AnimationConfig moveAnim;

    bool _active;

    private RectTransform _rectTransform; 
    VerticaUpdater _updater;
    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
        if (_updater == null) _updater = GetComponentInParent<VerticaUpdater>();
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


    public override async void UpdateUI()
    {
        base.UpdateUI();

        if (_active)
        {
            await AnimateExpand();

            await AnimateItemsSpawn(false);
        }
        else
        {
            await AnimateItemsSpawn(true);

            await AnimateExpand();
        }

        if (_updater != null) _updater.UpdateSpacing();
    }

    private void ToggleExpand()
    {
        _active = !_active;
        UpdateUI();
        if (_updater != null) _updater.UpdateSpacing();
    }



    private async UniTask AnimateExpand()
    {
        float targetSize = _active ? expandedSize : defaultSize;
        float targetRotation = _active ? 0f : 180f;

        var sequence = StartAnimation();
        sequence.Append(_rectTransform.DOSizeDelta(new Vector2(_rectTransform.sizeDelta.x, targetSize), moveAnim.Duration)
                       .SetEase(moveAnim.Ease))
               .Join(expand.rect.DORotate(new Vector3(0, 0, targetRotation), moveAnim.Duration)
                       .SetEase(moveAnim.Ease));

        sequence.Play();
        await sequence.AsyncWaitForCompletion();
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
}
