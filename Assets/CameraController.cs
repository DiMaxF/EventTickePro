using Cysharp.Threading.Tasks;
using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Vector2 gridSize = new Vector2(50f, 50f); // Размер сетки
    [SerializeField] private float cellSize = 1f; // Размер ячейки сетки
    [SerializeField] private Color gridLineColor = Color.gray; // Цвет линий сетки
    [SerializeField] private float lineWidth = 0.05f; // Толщина линий сетки
    [SerializeField] private float gridZ = 5f; // Z-позиция сетки
    [SerializeField] private Shader shaderGrid; // Z-позиция сетки
    [SerializeField] private Button ignoreButton;
    [SerializeField] private float moveSpeed = 10f; // Скорость движения камеры
    [SerializeField] private float inertiaDamping = 0.1f; // Коэффициент затухания инерции

    [SerializeField] private bool active = true; // Активность управления камерой

    private Camera mainCamera;
    private GameObject gridContainer;
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

        GenerateGrid();
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

    // Генерация сетки
    private void GenerateGrid()
    {
        gridContainer = new GameObject("GridLayout");
        gridContainer.transform.position = new Vector3(0, 0, gridZ);

        int horizontalLines = Mathf.CeilToInt(gridSize.y / cellSize) + 1;
        int verticalLines = Mathf.CeilToInt(gridSize.x / cellSize) + 1;

        for (int i = 0; i < horizontalLines; i++)
        {
            float y = -gridSize.y / 2 + i * cellSize;
            CreateLine(new Vector3(-gridSize.x / 2, y, gridZ), new Vector3(gridSize.x / 2, y, gridZ), $"horizontal_{i}");
        }

        for (int i = 0; i < verticalLines; i++)
        {
            float x = -gridSize.x / 2 + i * cellSize;
            CreateLine(new Vector3(x, -gridSize.y / 2, gridZ), new Vector3(x, gridSize.y / 2, gridZ), $"vertical_{i}");
        }
    }

    // Создание линии для сетки
    private void CreateLine(Vector3 start, Vector3 end, string name)
    {

        GameObject lineObj = new GameObject(name);
        lineObj.transform.SetParent(gridContainer.transform);
        LineRenderer lineRenderer = lineObj.AddComponent<LineRenderer>();

        lineRenderer.material = new Material(shaderGrid);
        
        lineRenderer.material.color = gridLineColor;
        lineRenderer.startColor = gridLineColor;
        lineRenderer.endColor = gridLineColor;
        lineRenderer.startWidth = lineWidth;
        lineRenderer.endWidth = lineWidth;
        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0, start);
        lineRenderer.SetPosition(1, end);
        lineRenderer.useWorldSpace = true;
    }

    // Расчет границ камеры
    private void CalculateCameraBounds()
    {
        float camHeight = 2f * mainCamera.orthographicSize;
        float camWidth = camHeight * mainCamera.aspect;

        minBounds = new Vector2(-gridSize.x / 2 + camWidth / 2, -gridSize.y / 2 + camHeight / 2);
        maxBounds = new Vector2(gridSize.x / 2 - camWidth / 2, gridSize.y / 2 - camHeight / 2);
    }

    // Обработка сенсорного ввода
    private void HandleTouchInput()
    {
        if (Input.touchCount == 1)
        {
            Touch touch = Input.GetTouch(0);
            if (EventSystem.current.IsPointerOverGameObject(touch.fingerId))
            {
                // Выполняем raycast для получения всех объектов под касанием
                PointerEventData eventData = new PointerEventData(EventSystem.current)
                {
                    position = touch.position
                };
                List<RaycastResult> results = new List<RaycastResult>();
                EventSystem.current.RaycastAll(eventData, results);

                // Проверяем, есть ли среди результатов другие UI-элементы, кроме ignoreButton
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
                    // Если ignoreButton не назначена, блокируем на любом UI
                    onlyIgnoreButton = false;
                }

                // Разрешаем движение, только если касание над ignoreButton и нет других UI-элементов
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

    // Применение инерции
    private void ApplyInertia()
    {
        if (!isDragging && velocity.magnitude > 0.01f)
        {
            MoveCamera(velocity * Time.deltaTime);
            velocity = Vector3.Lerp(velocity, Vector3.zero, inertiaDamping);
        }
    }

    // Перемещение камеры
    private void MoveCamera(Vector3 delta)
    {
        Vector3 newPos = mainCamera.transform.position + delta;
        newPos.x = Mathf.Clamp(newPos.x, minBounds.x, maxBounds.x);
        newPos.y = Mathf.Clamp(newPos.y, minBounds.y, maxBounds.y);
        newPos.z = mainCamera.transform.position.z;
        mainCamera.transform.position = newPos;
    }

    // Асинхронное перемещение камеры к заданной позиции
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
}
