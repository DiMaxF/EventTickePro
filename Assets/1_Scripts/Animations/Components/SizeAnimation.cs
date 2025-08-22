using DG.Tweening;
using System;
using UnityEngine;


[RequireComponent(typeof(RectTransform))]
public class SizeAnimation : MonoBehaviour, IViewAnimation
{
    [SerializeField] private AnimationConfig config;
    [SerializeField] private Vector2 start = new Vector2(0, 170);
    [SerializeField] private Vector2 end = new Vector2(0, 900);
    [SerializeField] private int order = 0;
    [SerializeField] private bool parallel = false;

    public int Order => order;
    public bool IsParallel => parallel;

    private RectTransform rectTransform;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    public Tween AnimateShow()
    {
        rectTransform.sizeDelta = start;
        return DOTween.To(() => rectTransform.sizeDelta,
                          x => rectTransform.sizeDelta = x,
                          end,
                          config.Duration)
                     .SetEase(config.Ease)
                     .SetDelay(config.Delay);
    }

    public Tween AnimateHide()
    {
        rectTransform.sizeDelta = end;
        return DOTween.To(() => rectTransform.sizeDelta,
                          x => rectTransform.sizeDelta = x,
                          start,
                          config.Duration)
                     .SetEase(config.Ease)
                     .SetDelay(config.Delay);
    }
}