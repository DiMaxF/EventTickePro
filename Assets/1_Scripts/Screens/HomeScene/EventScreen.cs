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
    [SerializeField] ButtonView back;
    [SerializeField] ButtonView mapEditor;
    [SerializeField] ButtonView ticketsDistribution;
    [SerializeField] ButtonView tickets;

    protected override void UpdateViews()
    {
        base.UpdateViews();
        var selected = Data.Personal.GetSelectedEvent();
        name.text = selected.name;
        date.text = selected.date;
        time.text = selected.time;
        venue.text = selected.venue;
        desciption.text = selected.description;
        UIContainer.InitView(image, selected.imgPath);
        var map = Data.Maps.GetByEvent(selected);
        if (map != null) 
        {
            UIContainer.InitView(preview, map.pathPreview);
        }
        else
        {
            previewBg.color = Color.clear;
            UIContainer.InitView(preview, "");
        }
    }

    protected override void Subscriptions()
    {
        base.Subscriptions();
        UIContainer.SubscribeToView<ButtonView, object>(tickets, _ => OnButtonTickets());
        UIContainer.SubscribeToView<ButtonView, object>(back, _ => OnButtonBack());
        UIContainer.SubscribeToView<ButtonView, object>(ticketsDistribution, _ => OnButtonTicketsDistribution());
        UIContainer.SubscribeToView<ButtonView, object>(mapEditor, _ => OnButtonMapEditor());
    }
    private void OnButtonTickets() 
    {
        Container.Show<TicketsScreen>();
    }
    private void OnButtonTicketsDistribution()
    {
        var mapTickets = Data.Maps.GetByEvent(Data.Personal.GetSelectedEvent());
        if (mapTickets != null) 
        {
            Container.Show<TicketsDistributionScreen>();
        }
        else
        {
            NativeMobilePlugin.Instance.ShowToast("First set map seats");
        }
        
    }
    private void OnButtonBack()
    {
        Container.Show<HomeScreen>();
        Data.Personal.SetSelectedEvent(null);
    }

    private void OnButtonMapEditor()
    {
        SceneManager.LoadScene("MapEditor");
    }
}
