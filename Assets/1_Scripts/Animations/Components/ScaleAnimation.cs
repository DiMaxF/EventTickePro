using DG.Tweening;
using System;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class ScaleAnimation : MonoBehaviour, IViewAnimation
{
    [SerializeField] Transform target;
    [SerializeField] Vector3 startScale = Vector3.one * 0.8f;
    [SerializeField] AnimationConfig config;
    [SerializeField] int order = 0;
    [SerializeField] bool parallel = false;
    public int Order => order;

    public bool IsParallel => parallel;

    private Vector3 originalScale;

    private void Awake()
    {
        if (target == null) target = GetComponent<RectTransform>();
        originalScale = target.localScale;
    }

    public Tween AnimateShow()
    {
        target.localScale = startScale;
        return target.DOScale(originalScale, config.Duration)
            .SetEase(config.Ease)
            .SetDelay(config.Delay);
    }

    public Tween AnimateHide()
    {
        target.localScale = originalScale;
        return target.DOScale(startScale, config.Duration).SetEase(config.Ease);
    }
}
