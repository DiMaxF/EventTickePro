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

    private void Start()
    {
        if(navigationBar != null) UIContainer.RegisterView(navigationBar, true);

        foreach (var screen in screens) screen.Init(core, this);

        if (firstScreen != null) Show(firstScreen);
        else if (screens.Count > 0) Show(screens[0]);
        

        if (navigationBar != null) 
            UIContainer.SubscribeToView(navigationBar, (NavigationButtonData data) => 
            {
                Show(data.screen);
                Logger.Log("SHOW", "AppContainer");
            }, true);
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

    private async void Show(string name)
    {
        var targetScreen = FindScreen(name);
        
        if (targetScreen == null || _openedScreen == targetScreen) return;

        foreach (var screen in screens) 
        {
            if (screen == _openedScreen) continue;
            else screen.gameObject.SetActive(screen.name.Equals(name));
        }



        if(_openedScreen != null) await _openedScreen.Hide();
        _openedScreen = targetScreen;
        _openedScreen.OnShow();

        if (navigationBar != null)
        {
            if (hideNavigationBar.Contains(targetScreen)) navigationBar.Hide();
            else navigationBar.Show();

            UpdateData();
        }
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
        public bool selected;
    }

    private void UpdateData() 
    {
        if (navigationBar == null) return;
        foreach (var data in _data) 
        {
            data.selected = data.screen == _openedScreen;   
        }

        UIContainer.InitView(navigationBar, _data);
    }
}
