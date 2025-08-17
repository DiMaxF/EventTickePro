using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AppContainer : MonoBehaviour
{
    [Header("Base")]
    [SerializeField] AppScreen firstScreen;
    [SerializeField] List<AppScreen> screens = new List<AppScreen>();
    [Header("Navbar")]
    [SerializeField] NavigationBarView navigationBar;
    [SerializeField] List<NavigationButtonData> _data;
    [SerializeField] List<AppScreen> hideNavigationBar = new List<AppScreen>();

    private DataCore core => DataCore.Instance;

    private AppScreen _openedScreen;
    public AppScreen OpenedScreen => _openedScreen;

    public async void UpdateViews(bool delay = false)
    {
        UIContainer.RegisterView(navigationBar);
        UIContainer.InitView(navigationBar, _data);
        count++;
    }
    int count;

    private void Start()
    {
        foreach (var screen in screens) screen.Init(core, this);

        if (firstScreen != null) Show(firstScreen);
        else if (screens.Count > 0) Show(screens[0]);
        else UpdateViews();

        UIContainer.SubscribeToView(navigationBar, (NavigationButtonData data) => Show(data.screen), true);
    }

    public void Show(AppScreen target)
    {
        Show(target.name);
    }

    public void Show<AScreen>() where AScreen : AppScreen
    {
        var view = screens.OfType<AScreen>().FirstOrDefault();
        Show(view);
    }


    private void Show(string name)
    {
        var targetScreen = FindScreen(name);
        
        if (targetScreen == null || _openedScreen == targetScreen) return;

        foreach (var screen in screens)
        {
            screen.gameObject.SetActive(screen.name.Equals(name));
        }

        _openedScreen = targetScreen;
        _openedScreen.OnShow();
        if (navigationBar == null) return;
        if (hideNavigationBar.Contains(targetScreen)) 
        {
            navigationBar.Hide();
        }
        else navigationBar.Show();
        UpdateViews(count == 0);
    }


    public AppScreen FindScreen(string name) => 
        screens.Where(s => s.name == name).FirstOrDefault();

    public AScreen GetScreen<AScreen>() where AScreen : AppScreen
    {
        var view = screens.OfType<AScreen>().FirstOrDefault();
        return view;
    }

    [Serializable]
    public class NavigationButtonData
    {
        public AppScreen screen;
        public Sprite icon;
        [HideInInspector] public bool selected;
    }
}
