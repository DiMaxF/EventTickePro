using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public static class UIContainer
{
    private static readonly List<View> _currentViews = new();
    private static readonly List<View> _persistentViews = new(); 
    private static readonly Dictionary<View, List<Action<object>>> _currentViewSubscriptions = new();
    private static readonly Dictionary<View, List<Action<object>>> _persistentViewSubscriptions = new();
    private static readonly Dictionary<View, object> _currentViewUpdateSources = new();

    public static void RegisterView<TView>(TView view, bool persistent = false) where TView : View
    {
        if (persistent)
        {
            if (!_persistentViews.Contains(view))
            {
                _persistentViews.Add(view);
                Loger.Log($"Registered persistent view {view.name}", "UIContainer");
            }
        }
        else
        {
            if (!_currentViews.Contains(view))
            {
                _currentViews.Add(view);
                Loger.Log($"Registered non-persistent view {view.name}", "UIContainer");
            }
        }
    }

    public static void SubscribeToView<TView, TData>(TView view, Action<TData> handler, bool isPersistent = false) where TView : View
    {
        var targetSubscriptions = isPersistent ? _persistentViewSubscriptions : _currentViewSubscriptions;

        if (!targetSubscriptions.ContainsKey(view))
        {
            targetSubscriptions[view] = new List<Action<object>>();
        }
        targetSubscriptions[view].Add(obj => handler(obj is TData data ? data : default));
        Loger.Log($"Subscribed to view {view.name} for type {typeof(TData).Name} (Persistent: {isPersistent})", "UIContainer");
    }

    public static void TriggerAction<T>(View view, T data)
    {
        if (_currentViewSubscriptions.TryGetValue(view, out var currentHandlers))
        {
            var currentHandlersCopy = currentHandlers.ToList();
            foreach (var handler in currentHandlersCopy)
            {
                handler(data);
            }
        }

        if (_persistentViewSubscriptions.TryGetValue(view, out var persistentHandlers))
        {
            var persistentHandlersCopy = persistentHandlers.ToList();
            foreach (var handler in persistentHandlersCopy)
            {
                handler(data);
            }
        }

        Loger.Log($"[TriggerAction] Triggered action for view {view.name} with data: {data}", "UIContainer");
    }

    public static TView GetView<TView>() where TView : View
    {
        return _persistentViews.OfType<TView>().FirstOrDefault() ?? _currentViews.OfType<TView>().FirstOrDefault();
    }

    public static void InitView<TView, TData>(TView view, TData data) where TView : View
    {
        Loger.Log($"View null: {view == null}", "UIContainer");

        if (view != null)
        {
            view.Init(data);
            _currentViewUpdateSources[view] = data;
        }
        Loger.Log($"InitView: {view}, {data}", "UIContainer");
    }

    public static TView FindView<TView>(string viewName) where TView : View
    {
        return _persistentViews.Find(v => v.name == viewName) as TView ?? _currentViews.Find(v => v.name == viewName) as TView;
    }

    public static void UnregisterView<TView>(TView view) where TView : View
    {
        if (_persistentViews.Contains(view))
        {
            _persistentViews.Remove(view);
            _persistentViewSubscriptions.Remove(view);
            Loger.Log($"Unregistered persistent view {view.name}", "UIContainer");
        }
        else
        {
            _currentViews.Remove(view);
            _currentViewSubscriptions.Remove(view);
            Loger.Log($"Unregistered non-persistent view {view.name}", "UIContainer");
        }
        _currentViewUpdateSources.Remove(view);
    }

    public static void UnsubscribeFromView<TView>(TView view) where TView : View
    {
        _currentViewSubscriptions.Remove(view);
        Loger.Log($"Unsubscribed from view {view.name} (non-persistent subscriptions only)", "UIContainer");
    }

    public static void Clear()
    {
        _currentViews.Clear();
        _currentViewSubscriptions.Clear();
        _currentViewUpdateSources.Clear();
        Loger.Log("Cleared non-persistent views and subscriptions", "UIContainer");
    }
}