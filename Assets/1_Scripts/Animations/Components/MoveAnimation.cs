using DG.Tweening;
using System;
using UnityEngine;

public class MoveAnimation : MonoBehaviour, IViewAnimation
{
    [SerializeField] RectTransform target;  
    [SerializeField] Vector3 offset = new Vector3(200, 0, 0); 
    [SerializeField] AnimationConfig config; 
    [SerializeField] int order = 0;
    [SerializeField] bool parallel = false;
    public int Order => order;

    public bool IsParallel => parallel;

    [SerializeField] private Vector3 originalPos;

    private void Awake()
    {
        if (target == null) target = GetComponent<RectTransform>();  
    }

    public Tween AnimateShow()
    {
        target.anchoredPosition = originalPos + offset;
        return target.DOAnchorPos(originalPos, config.Duration)
            .SetEase(config.Ease).SetDelay(config.Delay);
    }

    public Tween AnimateHide()
    {
        target.anchoredPosition = originalPos;
        return target.DOAnchorPos(originalPos + offset, config.Duration).SetEase(config.Ease);
    }
}
