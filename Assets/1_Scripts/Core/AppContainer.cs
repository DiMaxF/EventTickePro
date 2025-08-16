using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AppContainer : MonoBehaviour
{
    [SerializeField] private NavigationBarView navigationBar;
    [SerializeField] private List<AppScreen> hideNavigationBar = new List<AppScreen>();

    [SerializeField] private AppScreen firstScreen;
    [SerializeField] private List<AppScreen> screens = new List<AppScreen>();

    private DataCore core => DataCore.Instance;
    private UIContainer ui => UIContainer.Instance;
    public UIContainer UIContainer => ui;


    public event Action<AppScreen> OnScreenChanged;

    AppScreen _openedScreen;
    public AppScreen OpenedScreen => _openedScreen;

    public async void UpdateViews(bool delay = false)
    {
        if (navigationBar == null) return;
        else navigationBar.SetUIContainer(ui);

        ui.RegisterView(navigationBar);
        ui.InitView(navigationBar, this);
        count++;
    }
    int count;

    private void Start()
    {

        foreach (var screen in screens)
        {
            screen.Init(ui, core, this);
        }


        if (firstScreen != null) Show(firstScreen);
        else if (screens.Count > 0) Show(screens[0]);
        else UpdateViews();
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
        OnScreenChanged?.Invoke(_openedScreen);
        if (navigationBar == null) return;
        if (hideNavigationBar.Contains(targetScreen)) 
        {
            navigationBar.Hide();
        }else navigationBar.Show();
        UpdateViews(count == 0);
    }


    public AppScreen FindScreen(string name) => 
        screens.Where(s => s.name == name).FirstOrDefault();

    public AScreen GetScreen<AScreen>() where AScreen : AppScreen
    {
        var view = screens.OfType<AScreen>().FirstOrDefault();
        return view;
    }
}
