using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EventScreen : AppScreen
{
    [SerializeField] AsyncImageView image;
    [SerializeField] AsyncImageView preview;
    [SerializeField] Text name;
    [SerializeField] Text date;
    [SerializeField] Text time;
    [SerializeField] Text venue;
    [SerializeField] Text desciption;
    [SerializeField] Image previewBg;
    [Header("Buttons")]
    [SerializeField] Button back;
    [SerializeField] Button mapEditor;
    [SerializeField] Button ticketsDistribution;
    [SerializeField] Button tickets;

    protected override void UpdateViews()
    {
        base.UpdateViews();
        name.text = data.selectedEvent.name;
        date.text = data.selectedEvent.date;
        time.text = data.selectedEvent.time;
        venue.text = data.selectedEvent.venue;
        previewBg.color = Color.clear;
        desciption.text = data.selectedEvent.description;
        ui.InitView(image, data.selectedEvent.imgPath);
        var map = data.GetByEvent(data.selectedEvent);
        if (map != null) 
        {
            previewBg.color = Color.white;
            ui.InitView(preview, map.pathPreview);
        }
        else
        {
            ui.InitView(preview, "");
        }
    }

    protected override void Subscriptions()
    {
        base.Subscriptions();
        ui.SubscribeToComponent<Button, object>(tickets, _ => OnButtonTickets());
        ui.SubscribeToComponent<Button, object>(back, _ => OnButtonBack());
        ui.SubscribeToComponent<Button, object>(ticketsDistribution, _ => OnButtonTicketsDistribution());
        ui.SubscribeToComponent<Button, object>(mapEditor, _ => OnButtonMapEditor());
    }
    private void OnButtonTickets() 
    {
        container.Show<TicketsScreen>();
    }
    private void OnButtonTicketsDistribution()
    {
        var mapTickets = data.GetByEvent(data.selectedEvent);
        if (mapTickets != null) 
        {
            container.Show<TicketsDistributionScreen>();
        }
        else
        {
            NativeMobilePlugin.Instance.ShowToast("First set map seats");
        }
        
    }
    private void OnButtonBack()
    {
        container.Show<HomeScreen>();
        data.selectedEvent = null;
    }

    private void OnButtonMapEditor()
    {
        SceneManager.LoadScene("MapEditor");
    }
}
