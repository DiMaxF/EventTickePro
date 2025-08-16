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

        // ����������� ���������� �� �����������
        bool hasPermission = await NotificationManager.Instance.RequestNotificationPermissionAsync();
        if (!hasPermission)
        {
            Debug.LogWarning("Cannot schedule notifications: permission denied");
            return;
        }

        // ��������� ����������� �� ������ ������
        await ScheduleNotifications();
    }

    private async UniTask ScheduleNotifications()
    {
        if (appData == null || appData.events == null)
        {
            Debug.LogWarning("AppData or events list is null, cannot schedule notifications");
            return;
        }

        // ������� ��� ������������ �����������
        NotificationManager.Instance.ClearAllNotifications();

        int notificationId = 1000; // BaseNotificationId �� NotificationManager
        DateTime now = DateTime.Now;

        foreach (var eventModel in appData.events)
        {
            if (!eventModel.DateCorrect())
            {
                Debug.LogWarning($"Invalid date format for event: {eventModel.name}");
                continue;
            }

            DateTime eventDateTime = eventModel.Date.Date + eventModel.Time;

            // 1. ����������� � ������� ������� (���� notificationsSales ��������)
            if (appData.notificationsSales && eventDateTime > now.AddDays(1))
            {
                // ��������� ����������� �� ���� �� �������
                string title = $"������� �������: {eventModel.name}";
                string message = $"�� �������� ���������� ������ �� {eventModel.name}! ����������� ��������� {eventModel.date} � {eventModel.time}.";
                await NotificationManager.Instance.ScheduleSingleNotificationAsync(
                    notificationId++,
                    title,
                    message,
                    eventDateTime.AddDays(-1),
                    eventModel.notification // ���������� ���� notification ��� ���������� ������
                );
            }

            // 2. ����������� � ��������� ������������ (���� notificationsEvents ��������)
            if (appData.notificationsEvents && eventDateTime < now)
            {
                // ���������� ����������� � ��������� �������
                string title = $"����������� ���������: {eventModel.name}";
                string message = $"����������� {eventModel.name} ({eventModel.date} {eventModel.time}) �����������.";
                await NotificationManager.Instance.ScheduleSingleNotificationAsync(
                    notificationId++,
                    title,
                    message,
                    now.AddSeconds(5), // ����������� � ��������� ������� ������������ ����� (����� 5 ������)
                    eventModel.notification
                );
            }
        }

        Debug.Log($"������������� {notificationId - 1000} �����������");
    }
}