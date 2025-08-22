using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using static Unity.VisualScripting.StickyNote;
using static UnityEngine.GraphicsBuffer;

[RequireComponent(typeof(LayoutElement))]
public class LayoutElementAnimation : MonoBehaviour, IViewAnimation
{
    [SerializeField] AnimationConfig config;
    [SerializeField] float show;
    [SerializeField] float hide;
    public int Order => 0;

    public bool IsParallel => true;
    private LayoutElement layoutElement;


    private void Awake()
    {
        layoutElement = GetComponent<LayoutElement>();  
    }

    public Tween AnimateHide()
    {
        layoutElement.flexibleWidth = show;
        return DOTween.To(() => layoutElement.flexibleWidth,
                        x => layoutElement.flexibleWidth = x,
                        hide,
                        config.Duration).SetEase(config.Ease);
    }

    public Tween AnimateShow()
    {
        layoutElement.flexibleWidth = hide;
        return DOTween.To(() => layoutElement.flexibleWidth,
                        x => layoutElement.flexibleWidth = x,
                        show,
                        config.Duration).SetEase(config.Ease);
    }
}
