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

    private bool val;
    private AppContainer.NavigationButtonData screenData;
    private LayoutElement layoutElement;

    private void Awake()
    {
        layoutElement = GetComponent<LayoutElement>();
    }

    public override void Init<T>(T data)
    {
        Loger.Log($"Init object {data}", "NavigationButton");

        if (data is AppContainer.NavigationButtonData screen)
        {
            Loger.Log($"Init {screen.screen.name}", "NavigationButton");
            screenData = screen;
        }
        base.Init(data);    
    }

    public override void Subscriptions()
    {
        base.Subscriptions();
        Loger.Log($"UpdateUI", "NavigationButton");
        UIContainer.RegisterView(button, true);
        UIContainer.SubscribeToView<ButtonView, object>(button, _ => TriggerAction(screenData), true);
    }

    public override void UpdateUI()
    {
        Loger.Log($"UpdateUI", "NavigationButton");

        if (screenData != null)
        {
            Loger.Log($"{screenData.screen.name}" , "NavigationButton");
            icon.sprite = screenData.icon;
        
            icon.color = screenData.selected ?  selectedColor : inactiveColor;
            val = screenData.selected;

            AnimateSelected(screenData.selected);
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

        if (val == selected)
        {
            this.selected.color = targetC;
            return;
        }
        this.selected.transform.localScale = startS;
        this.selected.color = startC;

        StartAnimation().Append(this.selected.DOColor(targetC, colorAnim.Duration).SetEase(colorAnim.Ease))
            .Append(this.selected.transform.DOScale(targetS, scaleAnim.Duration).SetEase(scaleAnim.Ease))
            .Append(DOTween.To(() => layoutElement.flexibleWidth, x => layoutElement.flexibleWidth = x, targetFlexibleWidth, scaleAnim.Duration)
            .SetEase(scaleAnim.Ease));
        ;
        
    }
}