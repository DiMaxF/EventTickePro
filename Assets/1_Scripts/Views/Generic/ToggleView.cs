using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class ToggleView : View
{
    [SerializeField] private Image background;
    [SerializeField] private Image handle;
    [SerializeField] private Color active;
    [SerializeField] private Color inactive;
    [SerializeField] private Color activeHandle;
    [SerializeField] private Color inactiveHandle;
    [SerializeField] private ButtonView button;
    [SerializeField] AnimationConfig moveAnim;
    private bool _isOn = true;

    private void OnToggle()
    {
        _isOn = !_isOn;
        UpdateUI();
        TriggerAction(_isOn);
    }

    private void AnimateHandle()
    {
        var handleRectTransform = handle.rectTransform;
        float targetX = handleRectTransform.sizeDelta.x / 2;
        targetX *= _isOn ? 0.8f: -0.8f;
        Vector3 targetPosition = new Vector3(targetX, handleRectTransform.anchoredPosition.y);
        StartAnimation().Append(handleRectTransform.DOAnchorPos(targetPosition, moveAnim.Duration))
               .SetEase(moveAnim.Ease);
    }

    public override void UpdateUI()
    {
        background.color = _isOn ? active : inactive;
        handle.color = _isOn ? activeHandle : inactiveHandle;
        AnimateHandle();
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