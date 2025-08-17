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

        canvasGroup.DOKill();
        canvasGroup.alpha = 0f;
        await PreloadViewsAsync();
        OnStart();
        canvasGroup.DOFade(1f, 0.25f).SetDelay(0.1f);
        Loading(false);
        
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
}