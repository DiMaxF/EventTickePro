using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
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

    private bool isOn = true; 
    private Sequence animation; 



    private void OnToggle()
    {
        isOn = !isOn;
        UpdateUI(); 
        TriggerAction(isOn);
    }



    private void AnimateHandle()
    {
        animation?.Kill(); 
        animation = DOTween.Sequence();
        var handleRectTransform = handle.rectTransform;
        float targetX = isOn ? handleRectTransform.sizeDelta.x / 2 : -handleRectTransform.sizeDelta.x / 2;
        Vector3 targetPosition = new Vector3(targetX, handleRectTransform.anchoredPosition.y);

        animation.Append(handleRectTransform.DOAnchorPos(targetPosition, 0.3f))
                .SetEase(Ease.OutQuad);
    }

    public override void UpdateUI()
    {

        background.color = isOn ? active : inactive;
        handle.color = isOn ? activeHandle : inactiveHandle;
        AnimateHandle();
    }

    public override void Init<T>(T data)
    {
        if (data is bool initialState)
        {
            isOn = initialState;
        }
        base.Init(data);
        UIContainer.SubscribeToView<ButtonView, object>(button, _ => OnToggle());
    }


}
