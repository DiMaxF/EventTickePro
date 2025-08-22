using DG.Tweening;
using System;
using UnityEngine;

public class RotateAnimation : MonoBehaviour, IViewAnimation
{
    [SerializeField] private Transform target;
    [SerializeField] private Vector3 startRotation = Vector3.zero;
    [SerializeField] private Vector3 endRotation = new Vector3(0, 0, 90);
    [SerializeField] private AnimationConfig config;
    [SerializeField] private int order = 0;
    [SerializeField] private bool parallel = false;

    public int Order => order;
    public bool IsParallel => parallel;

    private void Awake()
    {
        if (target == null) target = GetComponent<Transform>();
    }

    public Tween AnimateShow()
    {
        target.localRotation = Quaternion.Euler(startRotation);
        return target.DORotate(endRotation, config.Duration, RotateMode.Fast)
            .SetEase(config.Ease)
            .SetDelay(config.Delay);
    }

    public Tween AnimateHide()
    {
        target.localRotation = Quaternion.Euler(endRotation);
        return target.DORotate(startRotation, config.Duration, RotateMode.Fast)
            .SetEase(config.Ease)
            .SetDelay(config.Delay);
    }
}
