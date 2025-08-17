using Cysharp.Threading.Tasks;
using System;
using UnityEngine;

public class DataCore : MonoBehaviour
{
    public static DataCore Instance { get; private set; }

    [SerializeField]
    private AppData _appData = new AppData();
    private EventManager _eventManager;
    private TicketManager _ticketManager;
    private MapManager _mapManager;
    private AnalyticsManager _analyticsManager;
    private PersonalManager _personalManager;

   
    public EventManager Events => _eventManager;
    public TicketManager Tickets => _ticketManager;
    public MapManager Maps => _mapManager;
    public AnalyticsManager Analytics => _analyticsManager;
    public PersonalManager Personal => _personalManager;

    private const string AppDataFileName = "appData.json";

    private void Awake()
    {
        if (Instance == null)
        {
            Application.targetFrameRate = 60;
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeManagers();
            LoadData();
            UpdateNotificationsAsync().Forget();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void InitializeManagers()
    {
        _eventManager = new EventManager(_appData);
        _ticketManager = new TicketManager(_appData);
        _mapManager = new MapManager(_appData);
        _analyticsManager = new AnalyticsManager(_eventManager, _ticketManager);
        _personalManager = new PersonalManager(_appData);
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
                _appData = loadedData;
                InitializeManagers(); 
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
            string json = JsonUtility.ToJson(_appData, true);
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

        bool hasPermission = await NotificationManager.Instance.RequestNotificationPermissionAsync();
        if (!hasPermission)
        {
            Debug.LogWarning("Cannot schedule notifications: permission denied");
            return;
        }

        await ScheduleNotifications();
    }

    private async UniTask ScheduleNotifications()
    {
        if (_appData == null || _appData.events == null)
        {
            Debug.LogWarning("AppData or events list is null, cannot schedule notifications");
            return;
        }

        NotificationManager.Instance.ClearAllNotifications();
        int notificationId = 1000;
        DateTime now = DateTime.Now;

        foreach (var eventModel in _appData.events)
        {
            if (!eventModel.DateCorrect())
            {
                Debug.LogWarning($"Invalid date format for event: {eventModel.name}");
                continue;
            }

            DateTime eventDateTime = eventModel.Date.Date + eventModel.Time;

            if (_appData.notificationsSales && eventDateTime > now.AddDays(1))
            {
                string title = $"Продажа билетов: {eventModel.name}";
                string message = $"Не забудьте приобрести билеты на {eventModel.name}! Мероприятие состоится {eventModel.date} в {eventModel.time}.";
                await NotificationManager.Instance.ScheduleSingleNotificationAsync(
                    notificationId++,
                    title,
                    message,
                    eventDateTime.AddDays(-1),
                    eventModel.notification
                );
            }

            if (_appData.notificationsEvents && eventDateTime < now)
            {
                string title = $"Мероприятие завершено: {eventModel.name}";
                string message = $"Мероприятие {eventModel.name} ({eventModel.date} {eventModel.time}) завершилось.";
                await NotificationManager.Instance.ScheduleSingleNotificationAsync(
                    notificationId++,
                    title,
                    message,
                    now.AddSeconds(5),
                    eventModel.notification
                );
            }
        }

        Debug.Log($"Запланировано {notificationId - 1000} уведомлений");
    }
}