using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Cysharp.Threading.Tasks;

[RequireComponent(typeof(CanvasGroup))]
public abstract class AppScreen : MonoBehaviour
{
    protected AppContainer Container;
    protected DataCore Data;

    private CanvasGroup _canvasGroup;
    private readonly IAnimationController _animationController = new DOTweenAnimationController();
    [SerializeField] AnimationConfig _fadeIn;
    [SerializeField] AnimationConfig _fadeOut;

    private void Awake()
    {
        _canvasGroup = GetComponent<CanvasGroup>() ?? gameObject.AddComponent<CanvasGroup>();
        _canvasGroup.alpha = 0;
    }

    /// <summary>
    /// Initializes the screen.
    /// </summary>
    public void Init(DataCore data, AppContainer container)
    {
        this.Data = data;
        this.Container = container;
    }

    private void Loading(bool value)
    {
        _canvasGroup.interactable = !value;
    }

    /// <summary>
    /// Automatically fetches and registers child views.
    /// </summary>
    public void AutoFetchViews()
    {
        foreach (var view in GetComponentsInChildren<View>(true))
        {
            UIContainer.RegisterView(view);
        }
    }

    /// <summary>
    /// Preloads views asynchronously.
    /// </summary>
    protected virtual async UniTask PreloadViewsAsync()
    {
        AutoFetchViews();
        await UniTask.Yield();
    }

    /// <summary>
    /// Called when the screen is shown.
    /// </summary>
    public async void OnShow()
    {
        Loading(true);
        StopAnimation();
        _canvasGroup.alpha = 0f;
        await PreloadViewsAsync();
        OnStart();
        if (_fadeIn != null)
        {
            var sequence = StartAnimation();
            sequence.Append(_canvasGroup
                                .DOFade(1f, _fadeIn.Duration)
                                .SetDelay(_fadeIn.Delay)
                                .SetEase(_fadeIn.Ease));
            await sequence.AsyncWaitForCompletion();
        }
        else Logger.LogError("Animation Config not found", "AppScreen");
        Loading(false);
    }

    /// <summary>
    /// Hides the screen asynchronously.
    /// </summary>
    public async UniTask Hide()
    {
        var sequence = StartAnimation();
        sequence.Append(_canvasGroup
                            .DOFade(0, _fadeOut.Duration)
                            .SetDelay(_fadeOut.Delay)
                            .SetEase(_fadeOut.Ease));
        await sequence.AsyncWaitForCompletion();
        gameObject.SetActive(false);
    }

    /// <summary>
    /// Called on screen start.
    /// </summary>
    protected virtual void OnStart()
    {
        if (Data == null || Container == null)
        {
            Logger.LogError($"{gameObject.name} not initialized", "AppScreen");
            return;
        }

        Subscriptions();
        UpdateViews();
    }

    /// <summary>
    /// Sets up subscriptions and clears previous.
    /// </summary>
    protected virtual void Subscriptions()
    {
        UIContainer.Clear();
        AutoFetchViews();
    }

    /// <summary>
    /// Updates child views.
    /// </summary>
    protected virtual void UpdateViews() { }

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
}