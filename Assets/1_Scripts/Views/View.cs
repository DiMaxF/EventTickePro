using DG.Tweening;
using UnityEngine;

public abstract class View : MonoBehaviour
{
    private readonly IAnimationController _animationController = new DOTweenAnimationController();
    public bool IsActive => gameObject.activeSelf;
    protected bool _subscribed;

    /// <summary>
    /// Initializes the view with data.
    /// </summary>
    public virtual void Init<T>(T data)
    {
        if (!_subscribed)
        {
            Subscriptions();
        }
        UpdateUI();
    }

    /// <summary>
    /// Updates the UI elements.
    /// </summary>
    public virtual void UpdateUI() { }

    /// <summary>
    /// Sets up subscriptions.
    /// </summary>
    public virtual void Subscriptions()
    {
        _subscribed = true;
    }

    /// <summary>
    /// Shows the view.
    /// </summary>
    public virtual void Show()
    {
        gameObject.SetActive(true);
        UpdateUI();
    }

    /// <summary>
    /// Hides the view.
    /// </summary>
    public virtual void Hide()
    {
        gameObject.SetActive(false);
    }

    /// <summary>
    /// Triggers an action with data.
    /// </summary>
    protected void TriggerAction<T>(T data)
    {
        UIContainer.TriggerAction(this, data);
    }

    /// <summary>
    /// Starts a new animation sequence.
    /// </summary>
    protected Sequence StartAnimation()
    {
        _animationController.StartAnimation();
        return ((DOTweenAnimationController)_animationController).GetSequence();
    }

    /// <summary>
    /// Stops the current animation.
    /// </summary>
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