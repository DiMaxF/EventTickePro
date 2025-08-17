using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class TicketsDistributionScreen : AppScreen
{
    [SerializeField] ButtonView back;
    [SerializeField] ButtonView createEmail;
    [SerializeField] ButtonView importEmail;
    [SerializeField] EmailSenderView emailSender;
    [SerializeField] private QRCodeEncodeController qrCode;
    private string currentQRPath;
    protected override void OnStart()
    {
        base.OnStart();
        emailSender.Hide();
        qrCode.onQREncodeFinished.RemoveAllListeners();
        qrCode.onQREncodeFinished.AddListener(
            (texture) =>
            {
                HadnleTextureQR(texture);
            }
        );
        UIContainer.RegisterView(emailSender);


    }

    protected override void Subscriptions()
    {
        base.Subscriptions();
        UIContainer.RegisterView(emailSender);
        UIContainer.SubscribeToView<ButtonView, object>(createEmail, _ => OnButtonCreateEmails());
        UIContainer.SubscribeToView<ButtonView, object>(back, _ => OnButtonBack());
        UIContainer.SubscribeToView<ButtonView, object>(importEmail, _ => OnButtonImportEmails());
        UIContainer.SubscribeToView<View, List<EmailModel>>(emailSender, AddTicketsToData);
    }

    protected override void UpdateViews()
    {
        base.UpdateViews();
        UIContainer.RegisterView(emailSender);
    }

    private void OnButtonCreateEmails()
    {
        UIContainer.RegisterView(emailSender);
        UIContainer .InitView(emailSender, "");
        Loger.Log("Tickerts", "TicketsDistributionScreen");
        emailSender.Show();
    }

    private void OnButtonBack()
    {
        container.Show<EventScreen>();
    }

    private void OnButtonImportEmails()
    {
        ImportCsvFile();
    }
    private async void ImportCsvFile()
    {
        var permission = await NativeFilePicker.RequestPermissionAsync(true);
        if (permission != NativeFilePicker.Permission.Granted)
        {
            Debug.LogWarning("Разрешение на доступ к файлам не предоставлено");
            return;
        }

        NativeFilePicker.PickFile((path) =>
        {
            if (string.IsNullOrEmpty(path))
            {
                Debug.LogWarning("Файл не выбран");
                return;
            }

            try
            {
                
                string csvContent = File.ReadAllText(path);
                var list = ParseEmailCSV(csvContent);
                Loger.Log($"list {csvContent}", "ParseEmailCSV");
                AddTicketsToData(list);
                NativeMobilePlugin.Instance.ShowToast($"Emails from {Path.GetFileName(path)} imports successfully");
            }
            catch (Exception e)
            {
                Debug.LogError($"Ошибка чтения CSV: {e.Message}");
            }
        }, "*/*");
    }

    public List<EmailModel> ParseEmailCSV(string csvContent)
    {
        var emails = new List<EmailModel>();
        var lines = csvContent.Split('\n').Where(line => !string.IsNullOrWhiteSpace(line)).ToList();
        if (lines.Count == 0) return emails;

        foreach (var line in lines.Skip(1))
        {
            var columns = line.Split(',').Select(col => col.Trim()).ToArray();
            if (columns.Length < 2) continue;

            try
            {
                string name = columns[0];
                string email = columns[1];
                emails.Add(new EmailModel(name, email));
            }
            catch (Exception e)
            {
                Debug.LogError($"Ошибка парсинга строки CSV: {line}\n{e.Message}");
            }
        }

        return emails;
    }


    private void AddTicketsToData(List<EmailModel> emails) 
    {
        foreach (var email in emails)
        {
            var ev = data.Personal.GetSelectedEvent();
            var hash = $"Name: {email.name}\nEmail: {email.email}\nEvent: {ev.name}\nDate: {ev.date}";
            int errorlog = qrCode.Encode(hash);
            var seat = ev.GetFreeSeat();
            
            
            if(seat == null) 
            {
                NativeMobilePlugin.Instance.ShowToast("No free seats available for this event.");
                break;
            }
            else
            {
                var ticket = new TicketModel
                {
                    contacts = email,
                    qrPath = currentQRPath,
                    valid = true,
                    seat = seat
                };
                data.Tickets.AddTicket(ev, ticket);
                seat.isTaken = true;
                data.SaveData();
                OnButtonBack();
            }
        }

    }

    private void HadnleTextureQR(Texture2D tex)
    {   
        if (tex != null)
        {
            int width = tex.width;
            int height = tex.height;
            float aspect = width * 1.0f / height;
            currentQRPath = FileManager.SaveTexture(tex, "qr_ticket");
            Loger.Log($"QR TEXTURE {tex} null({tex == null}) width({width}), height({height})");

        }
        
    }

}


