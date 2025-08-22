using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.UI;
public class ColorAnimation : MonoBehaviour, IViewAnimation
{
    [SerializeField] AnimationConfig config;
    [SerializeField] Color show;
    [SerializeField] Color hide;
    public int Order => 0;
    public bool IsParallel => true;

    [SerializeField] private Image _image;


    public Tween AnimateHide()
    {
        _image.color = show;
        return _image.DOColor(hide, config.Duration).SetEase(config.Ease);
    }

    public Tween AnimateShow()
    {
        _image.color = hide;
        return _image.DOColor(show, config.Duration).SetEase(config.Ease);
    }
}
