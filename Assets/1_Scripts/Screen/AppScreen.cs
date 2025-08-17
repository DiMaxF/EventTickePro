using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Cysharp.Threading.Tasks;

[RequireComponent(typeof(CanvasGroup))]
public abstract class AppScreen : MonoBehaviour
{
    protected AppContainer container;
    protected DataCore core;
    protected AppData data => core.AppData;
    
    private CanvasGroup canvasGroup;
    private readonly IAnimationController _animationController = new DOTweenAnimationController();
    [SerializeField] AnimationConfig fadeIn;
    [SerializeField] AnimationConfig fadeOut;
    
    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>() ?? gameObject.AddComponent<CanvasGroup>();
        canvasGroup.alpha = 0;
    }

    public void Init(DataCore core, AppContainer container)
    {
        this.core = core;
        this.container = container;
    }

    private void Loading(bool value)
    {
        canvasGroup.interactable = !value;
    }

    public void AutoFetchViews()
    {

        foreach (var view in GetComponentsInChildren<View>(true))
        {
            UIContainer.RegisterView(view);
        }
    }

    protected virtual async UniTask PreloadViewsAsync()
    {
        AutoFetchViews();
        await UniTask.Yield();
    }

    public async void OnShow()
    {
        Loading(true);
        StopAnimation();
        canvasGroup.alpha = 0f;
        await PreloadViewsAsync();
        OnStart();
        if (fadeIn != null)
        {
            var sequence = StartAnimation();
            sequence.Append(canvasGroup
                                .DOFade(1f, fadeIn.Duration)
                                .SetDelay(fadeIn.Delay)
                                .SetEase(fadeIn.Ease));
            await sequence.AsyncWaitForCompletion();
        }
        else Loger.LogError("Animation Config not found", "AppScreen");
        Loading(false);
    }


    public async UniTask Hide() 
    {
        var sequence = StartAnimation();
        sequence.Append(canvasGroup
                            .DOFade(0, fadeOut.Duration)
                            .SetDelay(fadeOut.Delay)
                            .SetEase(fadeOut.Ease));
        await sequence.AsyncWaitForCompletion();
        gameObject.SetActive(false);
    }

    protected virtual void OnStart()
    {
        if (core == null || data == null || container == null)
        {
            Loger.LogError($"{gameObject.name} not initialize", "AppScreen");
            Loger.LogError($"core {core == null}", "AppScreen");
            Loger.LogError($"data {data == null}", "AppScreen");
            Loger.LogError($"container {container == null}", "AppScreen");
            return;
        }
        
        Subscriptions();
        UpdateViews();
    }

    protected virtual void Subscriptions() 
    {
        UIContainer.Clear();
        AutoFetchViews();
    }

    protected virtual void UpdateViews()
    {

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
}