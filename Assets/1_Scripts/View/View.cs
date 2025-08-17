using UnityEngine;

public abstract class View : MonoBehaviour
{
    public bool IsActive => gameObject.activeSelf;
    public bool Clearable = true;
    private bool _subscribed;
    public virtual void Init<T>(T data) 
    {
        if(!_subscribed) Subscriptions();
        UpdateUI(); 
    }

    public virtual void UpdateUI() { }

    public virtual void Subscriptions() 
    {
        _subscribed = true;
    }

    public virtual void Show()
    {
        gameObject.SetActive(true);
        UpdateUI();
    }

    public virtual void Hide()
    {
        gameObject.SetActive(false);

    }

    protected void TriggerAction<T>(T data)
    {
        UIContainer.TriggerAction(this, data);
    }
}