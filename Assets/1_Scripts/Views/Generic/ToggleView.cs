using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class ToggleView : View
{
    [SerializeField] private ButtonView button;
    private bool _isOn = true;

    private void OnToggle()
    {
        _isOn = !_isOn;
        if (_isOn) Show();
        else Hide();
        TriggerAction(_isOn);
    }

    override public async void Hide()
    {
        await AnimationPlayer.PlayAnimationsAsync(gameObject, false);
    }

    public override void Init<T>(T data)
    {
        if (data is bool initialState) _isOn = initialState;
        base.Init(data);
    }

    public override void Subscriptions()
    {
        base.Subscriptions();
        UIContainer.SubscribeToView<ButtonView, object>(button, _ => OnToggle());

    }
}