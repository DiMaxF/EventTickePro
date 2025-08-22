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
        Animate();
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
        Animate();
    }

    public override void Subscriptions()
    {
        base.Subscriptions();
        UIContainer.SubscribeToView<ButtonView, object>(button, _ => OnToggle());

    }

    private void Animate()
    {
        if (_isOn) Show();
        else Hide();
    }
}