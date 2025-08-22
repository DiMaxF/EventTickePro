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
        Data = data;
        Container = container;
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
        _canvasGroup.alpha = 0f;
        await PreloadViewsAsync();
        OnStart();
        await AnimationPlayer.PlayAnimationsAsync(gameObject, true);
        Loading(false);
    }

    /// <summary>
    /// Hides the screen asynchronously.
    /// </summary>
    public async UniTask Hide()
    {
        foreach (var view in UIContainer.Views) view.OnExit();
        await AnimationPlayer.PlayAnimationsAsync(gameObject, false);
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

}