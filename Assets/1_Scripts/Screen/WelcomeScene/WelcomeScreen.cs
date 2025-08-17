using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class WelcomeScreen : AppScreen
{
    [SerializeField] ButtonView buttonStart;
    protected override void OnStart()
    {
        base.OnStart();
        UIContainer.SubscribeToView<ButtonView, object>(buttonStart, _ => OnButtonStart());
    }

    private void OnButtonStart()
    {
        SceneManager.LoadScene("Home");
    }

}
