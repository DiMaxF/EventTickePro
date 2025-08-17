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
    private bool _isOn = true;

    private void OnToggle()
    {
        _isOn = !_isOn;
        UpdateUI();
        TriggerAction(_isOn);
    }

    private void AnimateHandle()
    {
        var sequence = StartAnimation();
        var handleRectTransform = handle.rectTransform;
        float targetX = _isOn ? handleRectTransform.sizeDelta.x / 2 : -handleRectTransform.sizeDelta.x / 2;
        Vector3 targetPosition = new Vector3(targetX, handleRectTransform.anchoredPosition.y);
        sequence.Append(handleRectTransform.DOAnchorPos(targetPosition, 0.3f))
               .SetEase(Ease.OutQuad);
    }

    public override void UpdateUI()
    {
        background.color = _isOn ? active : inactive;
        handle.color = _isOn ? activeHandle : inactiveHandle;
        AnimateHandle();
    }

    public override void Init<T>(T data)
    {
        if (data is bool initialState)
        {
            _isOn = initialState;
        }
        base.Init(data);
        UIContainer.SubscribeToView<ButtonView, object>(button, _ => OnToggle());
    }
}