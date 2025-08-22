using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatusScanView : View
{
    [SerializeField] Image camFocus;
    [SerializeField] GameObject scanning;
    [SerializeField] GameObject success;
    [SerializeField] GameObject failed;

    private StatusScanning status;

    private void Awake()
    {
        scanning.transform.localScale = Vector3.zero;
    }

    public override void Init<T>(T data)
    {
        if (data is StatusScanning status) this.status = status;
        base.Init(data);
    }
    

    public override void UpdateUI()
    {
        base.UpdateUI();

        switch (status)
        {
            case StatusScanning.Scanning:
                scanning.SetActive(true);
                success.SetActive(false);
                failed.SetActive(false);
                MoveScannerLine();
                scanning.transform.localPosition = new Vector2(0, 180);
                camFocus.color = Color.white;
                break;
            case StatusScanning.Success:
                success.SetActive(true);
                failed.SetActive(false);
                scanning.SetActive(false);
                camFocus.color = new Color(92f/255f, 252f/255f, 119f/255f);
                break;
            case StatusScanning.Failed:
                failed.SetActive(true);
                success.SetActive(false);
                scanning.SetActive(false);
                camFocus.color = new Color(255f / 255f, 73f / 255f, 73f / 255f);
                break;
        }
    }

    async void MoveScannerLine()
    {
        await AnimationPlayer.PlayLoopAnimationsAsync(gameObject);
    }

}
public enum StatusScanning
{
    Scanning,
    Success,
    Failed
}