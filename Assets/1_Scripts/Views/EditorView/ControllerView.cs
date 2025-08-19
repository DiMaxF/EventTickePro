using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ControllerView : View
{
    [SerializeField] Color seletedColor;
    [SerializeField] Color unseletedColor;
    [SerializeField] private Button moveButton;
    [SerializeField] private Button[] scaleButtons;
    [SerializeField] private float dragSensitivity = 1f;

    private Vector2 initialTouchPos;
    private bool isDragging;
    private ViewState currentState;
    private Button activeButton;

    public override void Show()
    {
        base.Show();
        currentState = ViewState.Default;
        activeButton = null;
        UpdateButtons();
    }

    public override void Hide()
    {
        base.Hide();
        gameObject.SetActive(false);
    }

    private void Awake()
    {
        AddDragHandlers();
    }

    private void StartAction(ViewState state, Button button)
    {
        currentState = state;
        activeButton = button;
        if (state == ViewState.Scale) 
        {
            _horizontalScale = Horizontal(button);
            _verticalScale = Vertical(button);
        }
        UpdateButtons(button);
        TriggerAction(new ControllerData(currentState, activeButton));
    }

    private void AddDragHandlers()
    {
        AddDragHandler(moveButton.gameObject, HandleMoveDrag, moveButton, ViewState.Move);
        foreach (var button in scaleButtons)
        {
            AddDragHandler(button.gameObject, HandleScaleDrag, button, ViewState.Scale);
        }
    }

    private void AddDragHandler(GameObject target, System.Action<Vector2> dragHandler, Button button, ViewState state)
    {
        var eventTrigger = target.GetComponent<EventTrigger>() ?? target.AddComponent<EventTrigger>();

        var beginEntry = new EventTrigger.Entry { eventID = EventTriggerType.BeginDrag };
        beginEntry.callback.AddListener((data) =>
        {
            initialTouchPos = ((PointerEventData)data).position;
            isDragging = true;
            StartAction(state, button); 
        });
        eventTrigger.triggers.Add(beginEntry);

        var dragEntry = new EventTrigger.Entry { eventID = EventTriggerType.Drag };
        dragEntry.callback.AddListener((data) =>
        {
            if (isDragging)
            {
                Vector2 currentPos = ((PointerEventData)data).position;
                Vector2 delta = (currentPos - initialTouchPos) * dragSensitivity;
                dragHandler(delta);
                initialTouchPos = currentPos;
            }
        });
        eventTrigger.triggers.Add(dragEntry);

        var endEntry = new EventTrigger.Entry { eventID = EventTriggerType.EndDrag };
        endEntry.callback.AddListener((data) =>
        {
            isDragging = false;
            currentState = ViewState.Default;
            activeButton = null;
            UpdateButtons(); 
            TriggerAction(new ControllerData(currentState, null));
        });
        eventTrigger.triggers.Add(endEntry);
    }

    private void HandleMoveDrag(Vector2 delta)
    {
        if (currentState == ViewState.Move)
        {
            TriggerAction(new ControllerData(currentState, delta)); 
        }
    }

    private void HandleScaleDrag(Vector2 delta)
    {
        if (currentState == ViewState.Scale)
        {
            if (!_horizontalScale) delta.x = 0;
            if (!_verticalScale) delta.y = 0;
            TriggerAction(new ControllerData(delta, activeButton));
        }
    }

    bool _horizontalScale;
    bool _verticalScale;

    public bool Horizontal(Button btn)
    {
        return btn.transform.localPosition.x != 0;
    }

    public bool Vertical(Button btn)
    {
        return btn.transform.localPosition.y != 0;
    }

    public void UpdateButtons(Button button = null)
    {
        foreach (var btn in scaleButtons)
        {
            btn.image.color = (button == btn) ? seletedColor : unseletedColor;
        }
        moveButton.transform.GetChild(0).GetComponent<Image>().color = (button == moveButton) ? seletedColor : unseletedColor;
    }
}
public struct ControllerData
{
    public ViewState state;
    public Vector2 delta;
    public Button button;

    public ControllerData(ViewState state, Button button)
    {
        this.state = state;
        this.delta = Vector2.zero;
        this.button = button;
    }
    public ControllerData(ViewState state, Vector2 delta)
    {
        this.state = state;
        this.delta = delta;
        this.button = null;
    }

    public ControllerData(Vector2 delta, Button button)
    {
        this.state = ViewState.Scale; 
        this.delta = delta;
        this.button = button;
    }
}