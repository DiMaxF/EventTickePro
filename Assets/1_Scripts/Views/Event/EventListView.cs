using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EventListView : View
{
    [SerializeField] RectTransform view;
    [SerializeField] AsyncImageView image;
    [SerializeField] Text name;
    [SerializeField] Text dateTime;
    [SerializeField] Text description;
    [SerializeField] ButtonView action;
    [Header("Animations")]
    [SerializeField] AnimationConfig scaleAnim;
    [SerializeField] AnimationConfig moveAnim;

    EventModel _event;
    private Vector3 _initialPosition;
    private Vector3 _initialScale;
    private void Awake()
    {
        _initialPosition = view.localPosition;
        _initialScale = view.localScale;
    }

    public override void UpdateUI()
    {
        base.UpdateUI();
        UIContainer.RegisterView(image);
        image.widthFill = true;
        UIContainer.InitView(image, _event.imgPath);
        dateTime.text = $"{_event.date} {_event.time}";
        description.text = $"{_event.description}";
        name.text = $"{_event.name}";
    }

    public override void Init<T>(T data)
    {
        if (data is EventModel model) _event = model;
        base.Init(data);
    }

    public override void Subscriptions()
    {
        base.Subscriptions();
        UIContainer.SubscribeToView<ButtonView, object>(action, _ => TriggerAction(_event));
    }

    public override void Show()
    {
        base.Show();
        /*float height = view.rect.height; 
        view.localPosition = _initialPosition - new Vector3(0, height, 0); 
        view.localScale = _initialScale * 0.8f;

        Tween positionTween = view.DOLocalMove(_initialPosition, moveAnim.Duration).SetEase(moveAnim.Ease); 
        Tween scaleTween = view.DOScale(_initialScale, scaleAnim.Duration).SetEase(scaleAnim.Ease);

        StartAnimation().Append(positionTween).Join(scaleTween);*/
    }
}
