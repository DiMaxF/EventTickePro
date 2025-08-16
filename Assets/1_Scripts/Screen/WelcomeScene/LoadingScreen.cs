using Cysharp.Threading.Tasks;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadingScreen : AppScreen
{
    [SerializeField] private float animationDuration = 2f;
    async void Start()
    {
        Application.targetFrameRate = 60;
        bool permissionGranted = await NotificationManager.Instance.RequestNotificationPermissionAsync();
        if (!permissionGranted)
        {
            CheckPermission();
        }
        await UniTask.WaitForSeconds(animationDuration);
        container.Show<WelcomeScreen>();
    }

    private async void CheckPermission()
    {
       await NotificationManager.Instance.RequestNotificationPermissionAsync();
    }

}
