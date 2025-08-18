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
    private Vector2 pos;
    private void Awake()
    {
        pos = scanning.transform.localPosition;
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

    void MoveScannerLine()
    {
        StartAnimation()
            .Append(scanning.transform.DOLocalMoveY(pos.y + 180, 2f).SetEase(Ease.InOutSine)) 
            .Append(scanning.transform.DOLocalMoveY(pos.y - 180, 2f).SetEase(Ease.InOutSine)) 
            .SetLoops(-1, LoopType.Yoyo); 
    }

}
public enum StatusScanning
{
    Scanning,
    Success,
    Failed
}