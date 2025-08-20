using Cysharp.Threading.Tasks;
using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CameraController : MonoBehaviour
{
    [SerializeField] private AreaConfig areaConfig;
    [SerializeField] private Button ignoreButton;
    [SerializeField] private float moveSpeed = 10f;
    [SerializeField] private float inertiaDamping = 0.1f; 
    [SerializeField] private bool active = true; 

    private Camera mainCamera;
    private Vector3 touchStartPos;
    private Vector3 velocity;
    private bool isDragging;
    private Vector2 minBounds;
    private Vector2 maxBounds;

    void Start()
    {
        mainCamera = GetComponent<Camera>();
        mainCamera.orthographic = true;
        mainCamera.orthographicSize = 5f;
        mainCamera.nearClipPlane = 0.3f;
        CalculateCameraBounds();
    }

    void Update()
    {
        if (active)
        {
            HandleTouchInput();
        }
        ApplyInertia();
    }


    private void HandleTouchInput()
    {
        if (Input.touchCount == 1)
        {
            Touch touch = Input.GetTouch(0);
            if (EventSystem.current.IsPointerOverGameObject(touch.fingerId))
            {
                PointerEventData eventData = new PointerEventData(EventSystem.current)
                {
                    position = touch.position
                };
                List<RaycastResult> results = new List<RaycastResult>();
                EventSystem.current.RaycastAll(eventData, results);

                bool onlyIgnoreButton = true;
                bool hasIgnoreButton = false;
                if (ignoreButton != null)
                {
                    foreach (var result in results)
                    {
                        if (result.gameObject == ignoreButton.gameObject)
                        {
                            hasIgnoreButton = true;
                        }
                        else if (result.gameObject.GetComponent<Graphic>()?.raycastTarget == true)
                        {
                            onlyIgnoreButton = false;
                            break;
                        }
                    }
                }
                else
                {
                    onlyIgnoreButton = false;
                }

                if (!(hasIgnoreButton && onlyIgnoreButton))
                {
                    isDragging = false;
                    return;
                }
            }
            switch (touch.phase)
            {
                case TouchPhase.Began:
                    touchStartPos = mainCamera.ScreenToWorldPoint(touch.position);
                    velocity = Vector3.zero;
                    isDragging = true;
                    break;

                case TouchPhase.Moved:
                    if (isDragging)
                    {
                        Vector3 currentTouchPos = mainCamera.ScreenToWorldPoint(touch.position);
                        Vector3 delta = currentTouchPos - touchStartPos;
                        MoveCamera(-delta * moveSpeed * Time.deltaTime);
                        velocity = -delta * moveSpeed;
                        touchStartPos = currentTouchPos;
                    }
                    break;

                case TouchPhase.Ended:
                case TouchPhase.Canceled:
                    isDragging = false;
                    break;
            }
        }
    }

    private void ApplyInertia()
    {
        if (!isDragging && velocity.magnitude > 0.01f)
        {
            MoveCamera(velocity * Time.deltaTime);
            velocity = Vector3.Lerp(velocity, Vector3.zero, inertiaDamping);
        }
    }

    private void MoveCamera(Vector3 delta)
    {
        Vector3 newPos = mainCamera.transform.position + delta;
        newPos.x = Mathf.Clamp(newPos.x, minBounds.x, maxBounds.x);
        newPos.y = Mathf.Clamp(newPos.y, minBounds.y, maxBounds.y);
        newPos.z = mainCamera.transform.position.z;
        mainCamera.transform.position = newPos;
    }

    public async UniTask MoveToPosition(Vector3 targetPosition, float duration = 0.5f)
    {
        targetPosition.z = transform.position.z;

        targetPosition.x = Mathf.Clamp(targetPosition.x, minBounds.x, maxBounds.x);
        targetPosition.y = Mathf.Clamp(targetPosition.y, minBounds.y, maxBounds.y);

        velocity = Vector3.zero;
        isDragging = false;

        await transform.DOMove(targetPosition, duration).SetEase(Ease.OutQuad).AsyncWaitForCompletion();
    }

    public bool ToggleCameraActive()
    {
        active = !active;
        if (!active)
        {
            isDragging = false;
            velocity = Vector3.zero;
        }
        return active;
    }


    private void CalculateCameraBounds()
    {
        float camHeight = 2f * mainCamera.orthographicSize;
        float camWidth = camHeight * mainCamera.aspect;

        minBounds = new Vector2(-areaConfig.gridSize.x / 2 + camWidth / 2, -areaConfig.gridSize.y / 2 + camHeight / 2);
        maxBounds = new Vector2(areaConfig.gridSize.x / 2 - camWidth / 2, areaConfig.gridSize.y / 2 - camHeight / 2);
    }
}
