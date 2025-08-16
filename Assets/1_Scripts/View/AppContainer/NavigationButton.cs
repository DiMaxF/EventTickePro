using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(LayoutElement))]
public class NavigationButton : View
{
    [SerializeField] private Image icon;
    [SerializeField] private Button button;
    [SerializeField] private Image selected;
    [SerializeField] private Color selectedColor = Color.white;
    [SerializeField] private Color inactiveColor = Color.white;
    [SerializeField] private float animationDuration = 0.25f;
    private NavigationBarView.Data screenData;
    private LayoutElement layoutElement;

    bool val;
    private void Awake()
    {
        layoutElement = GetComponent<LayoutElement>();
    }
    public override void Init<T>(T data)
    {

        if (data is NavigationBarView.Data screen)
        {
            screenData = screen;
            UIContainer.StableSubscribeToComponent<Button, object>(button, _ => TriggerAction(screenData));
        }
        UpdateUI();
    }

    public override void UpdateUI()
    {
        if(screenData != null)
        {
            icon.sprite = screenData.icon;
        }
        
        icon.color = screenData.selected ?  selectedColor : inactiveColor;
        AnimateSelected(screenData.selected);
        val = screenData.selected;
    }
    private void AnimateSelected(bool selected) 
    {
        Color startC;
        Color targetC;
        Vector3 targetS;
        Vector3 startS;
        float targetFlexibleWidth;

        if (selected) 
        {
            startC = Color.clear;
            startS = Vector3.zero;
            targetC = selectedColor;
            targetS = Vector3.one;
            targetFlexibleWidth = 1.7f;
        }
        else
        {
            startC = selectedColor;
            startS = Vector3.one;
            targetC = Color.clear;
            targetS = Vector3.zero;
            targetFlexibleWidth = 1;
        }

        if (val == selected)
        {
            this.selected.color = targetC;
            return;
        }
        this.selected.transform.localScale = startS;
        this.selected.color = startC;
        this.selected.DOColor(targetC, animationDuration).SetEase(Ease.OutQuad);
        this.selected.transform.DOScale(targetS, animationDuration).SetEase(Ease.OutBack);
        DOTween.To(() => layoutElement.flexibleWidth, x => layoutElement.flexibleWidth = x, targetFlexibleWidth, animationDuration)
            .SetEase(Ease.OutBack);
    }
}