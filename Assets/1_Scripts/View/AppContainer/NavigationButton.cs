using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(LayoutElement))]
public class NavigationButton : View
{
    [SerializeField] private Image icon;
    [SerializeField] private ButtonView button;
    [SerializeField] private Image selected;
    [SerializeField] private Color selectedColor = Color.white;
    [SerializeField] private Color inactiveColor = Color.white;
    [Header("Animation Configs")]
    [SerializeField] AnimationConfig colorAnim;
    [SerializeField] AnimationConfig scaleAnim;
    int count;
    private bool val;
    private AppContainer.NavigationButtonData screenData;
    private LayoutElement layoutElement;

    private void Awake()
    {
        layoutElement = GetComponent<LayoutElement>();
    }

    public override void Init<T>(T data)
    {
        if (data is AppContainer.NavigationButtonData screen)
        {
            count++;
            Loger.Log($"Count init {count}", "NavigationButton");
            screenData = screen;
        }
        base.Init(data);    
    }

    public override void Subscriptions()
    {
        base.Subscriptions();
        UIContainer.RegisterView(button, true);
        UIContainer.SubscribeToView<ButtonView, object>(button, _ => 
        {
            TriggerAction(screenData);
        }, true);
    }

    public override void UpdateUI()
    {
        if (screenData != null)
        {
            icon.sprite = screenData.icon;
        
            icon.color = screenData.selected ?  selectedColor : inactiveColor;
            AnimateSelected(screenData.selected);

            val = screenData.selected;
            Loger.Log($"AnimateSelected {screenData.selected}", "NavigationButton");

        }
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
        bool isAlreadyInTargetState =
            this.selected.color == targetC &&
            this.selected.transform.localScale == targetS &&
            Mathf.Approximately(layoutElement.flexibleWidth, targetFlexibleWidth);
        if (selected == val)
        {
            this.selected.color = targetC;
            Loger.Log("VAL == SELECTED", "NavigationButton");
            return;
        }
        this.selected.transform.localScale = startS;
        this.selected.color = startC;
        button.interactable = false;

        StartAnimation().Join(this.selected.DOColor(targetC, colorAnim.Duration).SetEase(colorAnim.Ease))
            .Join(this.selected.transform.DOScale(targetS, scaleAnim.Duration).SetEase(scaleAnim.Ease))
            .Join(
            DOTween.To(() => layoutElement.flexibleWidth, 
                        x => layoutElement.flexibleWidth = x, 
                        targetFlexibleWidth, 
                        scaleAnim.Duration).SetEase(scaleAnim.Ease)).OnComplete(() => button.interactable = true);
    }
}