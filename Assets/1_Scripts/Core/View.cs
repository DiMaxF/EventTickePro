using Cysharp.Threading.Tasks;
using DG.Tweening;
using System.Linq;
using UnityEngine;

public abstract class View : MonoBehaviour
{
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
    /// Triggers an action with data.
    /// </summary>
    protected void TriggerAction<T>(T data)
    {
        UIContainer.TriggerAction(this, data);
    }

    /// <summary>
    /// Shows the view.
    /// </summary>
    public virtual async void Show()
    {
        gameObject.SetActive(true);
        UpdateUI();
        await AnimationPlayer.PlayAnimationsAsync(gameObject, true);
    }

    /// <summary>
    /// Hides the view.
    /// </summary>
    public virtual async void Hide()
    {
        await AnimationPlayer.PlayAnimationsAsync(gameObject, false);
        gameObject.SetActive(false);
    }


    public void OnExit()
    {
        _subscribed = false;
    }
}