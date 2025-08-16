using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class UIContainer : MonoBehaviour
{
    private static UIContainer _instance;
    public static UIContainer Instance
    {
        get
        {
            if (_instance == null)
            {
                var go = new GameObject("UIContainer");
                _instance = go.AddComponent<UIContainer>();
                DontDestroyOnLoad(go);
            }
            return _instance;
        }
    }

    private List<View> _currentViews = new();
    private Dictionary<View, List<Action<object>>> _currentViewSubscriptions = new();
    private Dictionary<View, object> _currentViewUpdateSources = new();
    private Dictionary<Component, Action> _componentListeners = new();
    private Dictionary<Component, Action> _stableComponentListeners = new();

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;
        DontDestroyOnLoad(gameObject);
    }


    public void RegisterView<TView>(TView view, object updateSource = null) where TView : View
    {
        if (!_currentViews.Contains(view))
        {
            _currentViews.Add(view);
            _currentViewUpdateSources[view] = updateSource;
            view.SetUIContainer(this);
        }
    }


    public void SubscribeToView<TView, TData>(TView view, Action<TData> handler) where TView : View
    {

        if (!_currentViewSubscriptions.ContainsKey(view))
        {
            _currentViewSubscriptions[view] = new List<Action<object>>();
        }
        _currentViewSubscriptions[view].Add(obj => handler(obj is TData data ? data : default));
        Loger.Log($"Subscribed to view {view.name} for type {typeof(TData).Name}");
    }

    public void SubscribeToComponent<TComponent, TData>(TComponent component, Action<TData> handler) where TComponent : Component
    {
        if (_componentListeners.ContainsKey(component))
        {
            if (component is Button btn)
            {
                btn.onClick.RemoveAllListeners();
            }
            else if (component is InputField inputField)
            {
                inputField.onValueChanged.RemoveAllListeners();
            }else if(component is Slider slider) 
            {
                slider.onValueChanged.RemoveAllListeners();
            }
            _componentListeners.Remove(component);
        }

        Action listener = null;
        if (component is Button button)
        {
            listener = () => handler(default);
            button.onClick.AddListener(() => listener());
        }
        else if (component is InputField inputField)
        {
            listener = () => handler((TData)(object)inputField.text);
            inputField.onValueChanged.AddListener(text => listener());
        }
        else if (component is Slider slider)
        {
            listener = () => handler((TData)(object)slider.value);
            slider.onValueChanged.AddListener(value => listener());
        }

        if (listener != null)
        {
            _componentListeners[component] = listener;
            Loger.Log($"Subscribed to component {component.name} for type {typeof(TData).Name}");
        }
    }

    public void StableSubscribeToComponent<TComponent, TData>(TComponent component, Action<TData> handler) where TComponent : Component
    {
        if (_stableComponentListeners.ContainsKey(component))
        {
            if (component is Button btn)
            {
                btn.onClick.RemoveAllListeners();
            }
            else if (component is InputField inputField)
            {
                inputField.onValueChanged.RemoveAllListeners();
            }
            else if (component is Slider slider)
            {
                slider.onValueChanged.RemoveAllListeners();
            }
            _stableComponentListeners.Remove(component);
        }

        Action listener = null;
        if (component is Button button)
        {
            listener = () => handler(default);
            button.onClick.AddListener(() => listener());
        }
        else if (component is InputField inputField)
        {
            listener = () => handler((TData)(object)inputField.text);
            inputField.onValueChanged.AddListener(text => listener());
        }
        else if (component is Slider slider)
        {
            listener = () => handler((TData)(object)slider.value);
            slider.onValueChanged.AddListener(value => listener());
        }

        if (listener != null)
        {
            _stableComponentListeners[component] = listener;
            Loger.Log($"Stable subscribed to component {component.name} for type {typeof(TData).Name}");
        }
    }


    public void TriggerAction<T>(View view, T data)
    {
        if (_currentViewSubscriptions.TryGetValue(view, out var handlers))
        {
            var handlersCopy = handlers.ToList(); 
            foreach (var handler in handlersCopy)
            {
                handler(data);
            }
            Loger.Log($"[UIContainer][TriggerAction] Triggered action for view {view.name} with data: {data}");
        }
    }

    public TView GetView<TView>() where TView : View
    {
        return _currentViews.OfType<TView>().FirstOrDefault();
    }

    public void InitView<TView, TData>(TView view, TData data) where TView : View
    {
        if (view != null)
        {
            view.Init(data);
            _currentViewUpdateSources[view] = data;
        }
    }

    public TView FindView<TView>(string viewName) where TView : View
    {
        return _currentViews.Find(v => v.name == viewName) as TView;
    }

    public void UnregisterView<TView>(TView view) where TView : View
    {
        if (!view.Clearable)
        {
            Loger.LogWarning($"Cannot unregister view {view.name} because it is not clearable.");
            return;
        }

        _currentViews.Remove(view);
        _currentViewSubscriptions.Remove(view);
        _currentViewUpdateSources.Remove(view);
    }
    public void UnsubscribeFromView<TView>(TView view) where TView : View
    {
        if (!view.Clearable) return;
        _currentViewSubscriptions.Remove(view);
        var keysToRemove = _componentListeners.Keys.Where(k => k == view).ToList();
        foreach (var key in keysToRemove)
        {
            _componentListeners.Remove(key);
        }
    }

    public void Clear()
    {
        var viewsToRemove = _currentViews.Where(view => view.Clearable).ToList();
        foreach (var view in viewsToRemove)
        {
            UnsubscribeFromView(view);
            UnregisterView(view);
        }
        foreach (var component in _componentListeners.Keys)
        {
            if (component is Button button)
            {
                button.onClick.RemoveAllListeners();
            }
            else if (component is InputField inputField)
            {
                inputField.onValueChanged.RemoveAllListeners();
            }
        }
        _componentListeners.Clear();
    }


}