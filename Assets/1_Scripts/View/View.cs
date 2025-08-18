using DG.Tweening;
using UnityEngine;

public abstract class View : MonoBehaviour
{
    private readonly IAnimationController _animationController = new DOTweenAnimationController();
    public bool IsActive => gameObject.activeSelf;
    protected bool _subscribed;

    public virtual void Init<T>(T data) 
    {
        if(!_subscribed) Subscriptions();
        UpdateUI(); 
    }

    public virtual void UpdateUI() { }

    public virtual void Subscriptions() 
    {
        _subscribed = true;
    }

    public virtual void Show()
    {
        gameObject.SetActive(true);
        UpdateUI();
    }

    public virtual void Hide()
    {
        gameObject.SetActive(false);

    }

    protected void TriggerAction<T>(T data)
    {
        UIContainer.TriggerAction(this, data);
    }

    protected Sequence StartAnimation()
    {
        _animationController.StartAnimation();
        return ((DOTweenAnimationController)_animationController).GetSequence();
    }

    protected void StopAnimation()
    {
        _animationController.StopAnimation();
    }
    protected bool HasActiveAnimation => _animationController.HasActiveAnimation;

    private void OnDisable()
    {
        _subscribed = false;
    }
}