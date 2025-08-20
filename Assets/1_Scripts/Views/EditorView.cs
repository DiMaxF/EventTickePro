using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class EditorView : View
{
    protected bool isSelected = false;
    protected RectTransform rectTransform;
    public RectTransform RectTransform => rectTransform;
    protected Transform transform;
    [SerializeField] private ControllerView controllerView;
    [SerializeField] private ButtonView selectButton;
    [SerializeField] private float scaleSensitivity = 0.01f;
    [SerializeField] private float dragSensitivity = 2f;
    private Vector3 initialPosition;
    private Vector2 initialSizeDelta;
    private ViewState _state;

    private void OnEnable()
    {
        transform = GetComponent<Transform>();
        rectTransform = GetComponent<RectTransform>();
        UIContainer.RegisterView(controllerView, true);
        if (controllerView != null)
        {
            controllerView.Hide();
        }
        UIContainer.SubscribeToView<ButtonView, object>(selectButton, _ => OnClick(), true);
    }

    public virtual void Select()
    {
        if (isSelected) return;
        isSelected = true;
        selectButton.gameObject.SetActive(false);
        initialSizeDelta = rectTransform.sizeDelta;
        if (controllerView != null)
        {
            controllerView.Show();
            SubscribeToController();
        }
        UpdateUI();
    }

    public virtual void UpdateColor(Color newColor)
    {
    }

    public virtual void Deselect()
    {
        if (!isSelected) return;
        isSelected = false;
        selectButton.gameObject.SetActive(true);
        if (controllerView != null)
        {
            controllerView.Hide();
        }
        UpdateUI();
    }

    public virtual void Rotate(float angle)
    {
        float currentAngle = transform.eulerAngles.z;
        float newAngle = currentAngle + angle;
        transform.rotation = Quaternion.Euler(0, 0, newAngle);
    }

    public virtual void Scale(Vector2 sizeDelta)
    {
        if (rectTransform != null)
        {
            rectTransform.sizeDelta = new Vector2(
                Mathf.Max(10f, sizeDelta.x),
                Mathf.Max(10f, sizeDelta.y)
            );
        }
    }

    public virtual void OnClick()
    {
        if (!isSelected)
        {
            Select();
            TriggerAction(this);
        }
        else
        {
            Deselect();
        }
    }

    private void SubscribeToController()
    {
        UIContainer.RegisterView(controllerView);
        UIContainer.SubscribeToView<ControllerView, object>(controllerView, HandleControllerAction);
    }

    private void HandleControllerAction(object data)
    {
        if (data is ControllerData controllerData)
        {
            if (controllerData.state == ViewState.Default)
            {
                _state = ViewState.Default;
                initialPosition = transform.position;
                initialSizeDelta = rectTransform != null ? rectTransform.sizeDelta : Vector2.one;
            }
            else if (controllerData.delta != Vector2.zero)
            {
                Vector2 adjustedDelta = controllerData.state == ViewState.Scale ? AdjustDelta(controllerData.delta, controllerData.button) : controllerData.delta;
                if (controllerData.state == ViewState.Move)
                {
                    Vector2 worldDelta = Camera.main.ScreenToWorldPoint(adjustedDelta) - Camera.main.ScreenToWorldPoint(Vector2.zero);
                    worldDelta *= Time.deltaTime * 10f * dragSensitivity;
                    transform.position += new Vector3(worldDelta.x, worldDelta.y, 0);
                    initialPosition = transform.position;
                }
                else if (controllerData.state == ViewState.Scale)
                {
                    Vector2 sizeDelta = adjustedDelta * scaleSensitivity * 100f;
                    Vector2 newSizeDelta = new Vector2(
                        initialSizeDelta.x + sizeDelta.x,
                        initialSizeDelta.y + sizeDelta.y
                    );
                    Scale(newSizeDelta);
                    initialSizeDelta = rectTransform.sizeDelta;
                }
            }
        }
    }

    private Vector2 AdjustDelta(Vector2 delta, Button button)
    {
        if (button == null) return delta;

        Vector2 adjustedDelta = delta;

        if (button.transform.localPosition.x > 0) {}
        else if (button.transform.localPosition.x < 0)
        {
            adjustedDelta.x = -delta.x; 
        }
        if (button.transform.localPosition.y > 0) {}
        else if (button.transform.localPosition.y < 0)
        {
            adjustedDelta.y = -delta.y; 
        }

        return adjustedDelta;
    }
}