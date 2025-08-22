using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public static class UIContainer
{
    private static readonly List<View> _currentViews = new List<View>();
    public static List<View> Views => _currentViews;
    private static readonly List<View> _persistentViews = new List<View>();
    private static readonly Dictionary<View, List<Action<object>>> _currentViewSubscriptions = new Dictionary<View, List<Action<object>>>();
    private static readonly Dictionary<View, List<Action<object>>> _persistentViewSubscriptions = new Dictionary<View, List<Action<object>>>();
    private static readonly Dictionary<View, object> _currentViewUpdateSources = new Dictionary<View, object>();

    /// <summary>
    /// Registers a view as persistent or non-persistent.
    /// </summary>
    public static void RegisterView<TView>(TView view, bool persistent = false) where TView : View
    {
        if (persistent)
        {
            if (!_persistentViews.Contains(view))
            {
                _persistentViews.Add(view);
                Logger.Log($"Registered persistent view {view.name}", "UIContainer");
            }
        }
        else
        {
            if (!_currentViews.Contains(view))
            {
                _currentViews.Add(view);
                Logger.Log($"Registered non-persistent view {view.name}", "UIContainer");
            }
        }
    }

    /// <summary>
    /// Subscribes a handler to view updates.
    /// </summary>
    public static void SubscribeToView<TView, TData>(TView view, Action<TData> handler, bool isPersistent = false) where TView : View
    {
        var targetSubscriptions = isPersistent ? _persistentViewSubscriptions : _currentViewSubscriptions;

        if (!targetSubscriptions.ContainsKey(view))
        {
            targetSubscriptions[view] = new List<Action<object>>();
        }
        targetSubscriptions[view].Add(obj => handler(obj is TData data ? data : default));
        Logger.Log($"Subscribed to view {view.name} for type {typeof(TData).Name} (Persistent: {isPersistent})", "UIContainer");
    }

    /// <summary>
    /// Triggers actions subscribed to the view.
    /// </summary>
    public static void TriggerAction<T>(View view, T data)
    {
        var allHandlers = new List<Action<object>>();

        if (_currentViewSubscriptions.TryGetValue(view, out var currentHandlers))
        {
            allHandlers.AddRange(currentHandlers);
        }

        if (_persistentViewSubscriptions.TryGetValue(view, out var persistentHandlers))
        {
            allHandlers.AddRange(persistentHandlers);
        }

        foreach (var handler in allHandlers)
        {
            handler(data);
        }

        Logger.Log($"[TriggerAction] Triggered action for view {view.name} with data: {data}", "UIContainer");
    }

    /// <summary>
    /// Gets the first view of the specified type.
    /// </summary>
    public static TView GetView<TView>() where TView : View
    {
        return _persistentViews.OfType<TView>().FirstOrDefault() ?? _currentViews.OfType<TView>().FirstOrDefault();
    }

    /// <summary>
    /// Initializes a view with data.
    /// </summary>
    public static void InitView<TView, TData>(TView view, TData data) where TView : View
    {
        if (view == null)
        {
            Logger.LogError("View is null", "UIContainer");
            return;
        }

        view.Init(data);
        _currentViewUpdateSources[view] = data;
        Logger.Log($"InitView: {view.name}, {data}", "UIContainer");
    }

    /// <summary>
    /// Finds a view by name.
    /// </summary>
    public static TView FindView<TView>(string viewName) where TView : View
    {
        return _persistentViews.FirstOrDefault(v => v.name == viewName) as TView ?? _currentViews.FirstOrDefault(v => v.name == viewName) as TView;
    }

    /// <summary>
    /// Unregisters a view.
    /// </summary>
    public static void UnregisterView<TView>(TView view) where TView : View
    {
        if (_persistentViews.Remove(view))
        {
            _persistentViewSubscriptions.Remove(view);
            Logger.Log($"Unregistered persistent view {view.name}", "UIContainer");
        }
        else if (_currentViews.Remove(view))
        {
            _currentViewSubscriptions.Remove(view);
            Logger.Log($"Unregistered non-persistent view {view.name}", "UIContainer");
        }

        _currentViewUpdateSources.Remove(view);
    }

    /// <summary>
    /// Unsubscribes from a view's non-persistent subscriptions.
    /// </summary>
    public static void UnsubscribeFromView<TView>(TView view) where TView : View
    {
        _currentViewSubscriptions.Remove(view);
        Logger.Log($"Unsubscribed from view {view.name} (non-persistent subscriptions only)", "UIContainer");
    }

    /// <summary>
    /// Clears non-persistent views and subscriptions.
    /// </summary>
    public static void Clear()
    {
        _currentViews.Clear();
        _currentViewSubscriptions.Clear();
        _currentViewUpdateSources.Clear();
        Logger.Log("Cleared non-persistent views and subscriptions", "UIContainer");
    }
}