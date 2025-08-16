using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using UnityEngine;

public class DataCore : MonoBehaviour
{
    public static DataCore Instance { get; private set; }

    public bool NeedSettings
    {
        get => PlayerPrefs.GetInt(nameof(NeedSettings), 1) == 1;
        set => PlayerPrefs.SetInt(nameof(NeedSettings), value ? 1 : 0);
    }
    [SerializeField] AppData appData = new AppData();

    public AppData AppData => appData;

    const string AppDataFileName = "appData.json";

    private void Awake()
    {

        if (Instance == null)
        {
            Application.targetFrameRate = 60;
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadData();
            UpdateNotificationsAsync().Forget();
        }
        else
        {
            Destroy(gameObject);
        }

    }

   
    public void DiscardChanges()
    {
        LoadData();

    }
    private void LoadData()
    {
        if (!FileManager.FileExist(AppDataFileName))
        {
            Loger.Log($"{AppDataFileName} not found, starting with default data");
            return;
        }

        string json = FileManager.ReadFile(AppDataFileName);

        try
        {
            AppData loadedData = JsonUtility.FromJson<AppData>(json);
            if (loadedData != null)
            {
                appData = loadedData;
            }
        }
        catch (Exception ex)
        {
            Loger.LogError($"Error loading {AppDataFileName}: {ex.Message}");
        }
    }

    public void SaveData()
    {
        Loger.TryCatch(() => 
        {
            string json = JsonUtility.ToJson(appData, true);
            FileManager.WriteToFile(AppDataFileName, json);
            Loger.Log("GlobalData saved successfully");
        });
    }

    private void OnApplicationQuit()
    {
        SaveData();
    }
    private async UniTask UpdateNotificationsAsync()
    {
        if (NotificationManager.Instance == null)
        {
            Debug.LogError("NotificationManager is not initialized!");
            return;
        }

        // Запрашиваем разрешение на уведомления
        bool hasPermission = await NotificationManager.Instance.RequestNotificationPermissionAsync();
        if (!hasPermission)
        {
            Debug.LogWarning("Cannot schedule notifications: permission denied");
            return;
        }

        // Планируем уведомления на основе данных
        await ScheduleNotifications();
    }

    private async UniTask ScheduleNotifications()
    {
        if (appData == null || appData.events == null)
        {
            Debug.LogWarning("AppData or events list is null, cannot schedule notifications");
            return;
        }

        // Очищаем все существующие уведомления
        NotificationManager.Instance.ClearAllNotifications();

        int notificationId = 1000; // BaseNotificationId из NotificationManager
        DateTime now = DateTime.Now;

        foreach (var eventModel in appData.events)
        {
            if (!eventModel.DateCorrect())
            {
                Debug.LogWarning($"Invalid date format for event: {eventModel.name}");
                continue;
            }

            DateTime eventDateTime = eventModel.Date.Date + eventModel.Time;

            // 1. Уведомления о продаже билетов (если notificationsSales включено)
            if (appData.notificationsSales && eventDateTime > now.AddDays(1))
            {
                // Планируем уведомление за день до события
                string title = $"Продажа билетов: {eventModel.name}";
                string message = $"Не забудьте приобрести билеты на {eventModel.name}! Мероприятие состоится {eventModel.date} в {eventModel.time}.";
                await NotificationManager.Instance.ScheduleSingleNotificationAsync(
                    notificationId++,
                    title,
                    message,
                    eventDateTime.AddDays(-1),
                    eventModel.notification // Используем флаг notification для управления звуком
                );
            }

            // 2. Уведомления о прошедших мероприятиях (если notificationsEvents включено)
            if (appData.notificationsEvents && eventDateTime < now)
            {
                // Отправляем уведомление о прошедшем событии
                string title = $"Мероприятие завершено: {eventModel.name}";
                string message = $"Мероприятие {eventModel.name} ({eventModel.date} {eventModel.time}) завершилось.";
                await NotificationManager.Instance.ScheduleSingleNotificationAsync(
                    notificationId++,
                    title,
                    message,
                    now.AddSeconds(5), // Уведомление о прошедшем событии отправляется сразу (через 5 секунд)
                    eventModel.notification
                );
            }
        }

        Debug.Log($"Запланировано {notificationId - 1000} уведомлений");
    }
}