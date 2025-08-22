using DG.Tweening;
using System;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

public class FadeAnimation : MonoBehaviour, IViewAnimation
{
    [SerializeField] AnimationConfig config;

    [SerializeField] private CanvasGroup canvasGroup;
    public int Order => 0;

    public bool IsParallel => true;

    private void Awake()
    {
        if (canvasGroup == null) canvasGroup = gameObject.GetComponent<CanvasGroup>();
        if (canvasGroup == null) canvasGroup = gameObject.AddComponent<CanvasGroup>();    
    }

    public Tween AnimateHide()
    {
        canvasGroup.alpha = 1;
        return canvasGroup.DOFade(0, config.Duration).SetEase(config.Ease);
    }

    public Tween AnimateShow()
    {
        canvasGroup.alpha = 0;
        return canvasGroup.DOFade(1, config.Duration)
            .SetEase(config.Ease).SetDelay(config.Delay);
    }
}
