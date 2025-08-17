using Cysharp.Threading.Tasks;
using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NavigationBarView : View
{
    [SerializeField] private ListView buttons;
    [Header("Animations")]
    [SerializeField] AnimationConfig show;
    [SerializeField] AnimationConfig hide;

    private Vector3 _initialPosition;
    private RectTransform _rectTransform;
    List<AppContainer.NavigationButtonData> _data;

    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
        _initialPosition = _rectTransform.localPosition;
    }

    

    public override void Init<T>(T data)
    {
        if (data is List<AppContainer.NavigationButtonData> updateData) 
        {
            Loger.Log($"Init: {updateData.Count}", "NavigationBarView");
            
            _data = updateData;

            if(!buttons.isGenerated) UIContainer.InitView(buttons, _data);
        }
        base.Init(data);
    }

    public override void UpdateUI()
    {
        if (_data != null) 
        {
            buttons.UpdateViewsData(_data);
            Loger.Log($"screens: {_data.Count}", "NavigationBarView");
        }

    }

    public override void Subscriptions()
    {
        base.Subscriptions();

        UIContainer.SubscribeToView<ListView, AppContainer.NavigationButtonData>(buttons, selected => 
        {
            TriggerAction(selected);
            Loger.Log($"Selected data: {selected}", "NavigationBarView");
        }, true);

    }

    public override void Show()
    {
        base.Show();
        if (_initialPosition != _rectTransform.localPosition) 
        {
            float height = _rectTransform.rect.height;

            _rectTransform.localPosition = _initialPosition - new Vector3(0, height, 0);
            StartAnimation().Append(_rectTransform.DOLocalMove(_initialPosition, show.Duration).SetEase(show.Ease));
        }

    }

    public override void Hide()
    {
        if (_initialPosition == _rectTransform.localPosition)
        {
            float height = _rectTransform.rect.height;
            var target = _initialPosition - new Vector3(0, height, 0);
            StartAnimation()
                .Append(_rectTransform.DOLocalMove(target, hide.Duration).SetEase(hide.Ease))
                .OnComplete(() => base.Hide());
        }
    }


}