using UnityEngine;

public abstract class View : MonoBehaviour
{
    public bool IsActive => gameObject.activeSelf;
    public bool Clearable = true;
    protected UIContainer UIContainer { get; private set; }

    public virtual void Init<T>(T data) { UpdateUI(); }
    public virtual void UpdateUI() { }
    public virtual void Show()
    {
        gameObject.SetActive(true);
        UpdateUI();
        OnShow();
    }
    public virtual void Hide()
    {
        gameObject.SetActive(false);

    }
    protected virtual void OnShow() { }

    public void SetUIContainer(UIContainer container)
    {
        UIContainer = container;
    }

    protected void TriggerAction<T>(T data)
    {
        if (UIContainer != null)
        {
            UIContainer.TriggerAction(this, data);
        }
        else
        {
            Debug.LogWarning($"[{name}] UIContainer не установлен");
        }
    }
}