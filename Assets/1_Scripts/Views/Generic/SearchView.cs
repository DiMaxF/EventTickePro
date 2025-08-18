using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class SearchView : View
{
    [SerializeField] private InputTextView inputTextView;
    [SerializeField] private ButtonView searchButton;

    private List<TicketModel> _dataSource = new List<TicketModel>();
    private List<TicketModel> _lastSearchResults = new List<TicketModel>();
    private Func<TicketModel, string, bool> _matchCriteria;
    private Func<TicketModel, string> _displayFormatter;

    public IReadOnlyList<TicketModel> LastSearchResults => _lastSearchResults.AsReadOnly();

    public void Init(List<TicketModel> dataSource, Func<TicketModel, string, bool> matchCriteria, Func<TicketModel, string> displayFormatter)
    {
        _dataSource = dataSource ?? new List<TicketModel>();
        _matchCriteria = matchCriteria ?? DefaultMatchCriteria;
        _displayFormatter = displayFormatter ?? DefaultDisplayFormatter;

        _lastSearchResults.Clear();
        //UIContainer.SubscribeToView<InputTextView, string>(inputTextView, _ => PerformSearch());
        UIContainer.SubscribeToView<ButtonView, object>(searchButton, _ => 
        {
            Logger.Log("Action", "SearchView");
            PerformSearch();
        });
        UpdateUI();
    }

    public override void Init<TData>(TData data)
    {
        Logger.Log("Try Init", "SearchView");

        if (data is List<TicketModel> dataSource)
        {
            Init(dataSource, DefaultMatchCriteria, DefaultDisplayFormatter);
        }
        else
        {
            Init(new List<TicketModel>(), DefaultMatchCriteria, DefaultDisplayFormatter);
        }
    }

    public override void UpdateUI()
    {
        _lastSearchResults.Clear();
        UIContainer.RegisterView(inputTextView);
        UIContainer.InitView<InputTextView, string>(inputTextView, "");
    }


    private void PerformSearch()
    {
        try
        {
            Debug.Log("Search VIEW ACTION");
            string query = inputTextView?.text?.ToLowerInvariant() ?? "";
            _lastSearchResults.Clear();

            if (string.IsNullOrEmpty(query))
            {
                _lastSearchResults.AddRange(_dataSource);
            }
            else
            {
                _lastSearchResults = _dataSource
                    .Where(item => item != null && _matchCriteria(item, query))
                    .OrderBy(item => _displayFormatter(item))
                    .ToList();
            }

            Debug.Log($"Search results count: {_lastSearchResults.Count}");
            TriggerAction(_lastSearchResults);
        }
        catch (Exception ex)
        {
            Debug.LogError($"[{name}] Поиск не удался: {ex.Message}", this);
            _lastSearchResults.Clear();
            TriggerAction(_dataSource);
        }
    }
    private DataCore data => DataCore.Instance;
    private bool DefaultMatchCriteria(TicketModel ticket, string query)
    {
        if (ticket == null || ticket.contacts == null) return false;

        var eventModel = data.Events.GetEventByTicket(ticket);
        if (eventModel == null) return false;

        // Проверяем дату, время, имя и email
        string dateStr = eventModel.date?.ToLowerInvariant() ?? "";
        string timeStr = eventModel.time?.ToLowerInvariant() ?? "";
        string nameStr = ticket.contacts.name?.ToLowerInvariant() ?? "";
        string emailStr = ticket.contacts.email?.ToLowerInvariant() ?? "";

        return dateStr.Contains(query) || timeStr.Contains(query) || nameStr.Contains(query) || emailStr.Contains(query);
    }

    private string DefaultDisplayFormatter(TicketModel ticket)
    {
        var eventModel = data.Events.GetEventByTicket(ticket);
        if (eventModel == null) return ticket.contacts?.name ?? "Unknown Ticket";

        return $"{eventModel.name} - {eventModel.date} {eventModel.time} - {ticket.contacts?.name}";
    }
}