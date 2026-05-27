using UnityEngine;
using UnityEngine.EventSystems;

public class PanController : MonoBehaviour
{
    public RectTransform gridContainer;
    public RectTransform floatingScoresContainer;
    public RectTransform canvasRect;

    public float padding = 100f;
    public float rightMouseDragSensitivity = 1f;

    public static PanController Instance { get; private set; }

    public bool PanModeActive { get; private set; }

    private Vector2 originalGridPosition;
    private Vector2 originalFloatingPosition;
    private bool originalSaved;

    private bool isDragging;
    private Vector2 lastPointerPos;
    private int activeDragPointerId = int.MinValue;

    private void Awake()
    {
        Instance = this;
    }

    private void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }

    private void Start()
    {
        Debug.Assert(gridContainer != null, "PanController: gridContainer not assigned!");
        Debug.Assert(canvasRect != null, "PanController: canvasRect not assigned!");

        if (gridContainer != null)
        {
            originalGridPosition = gridContainer.anchoredPosition;
            originalSaved = true;
        }
        if (floatingScoresContainer != null)
        {
            originalFloatingPosition = floatingScoresContainer.anchoredPosition;
        }
    }

    public void TogglePanMode()
    {
        SetPanMode(!PanModeActive);
    }

    public void SetPanMode(bool active)
    {
        PanModeActive = active;
        if (!active)
        {
            isDragging = false;
            activeDragPointerId = int.MinValue;
        }
    }

    public bool ShouldBlockWordInput(int pointerId)
    {
        if (PanModeActive) return true;
        if (isDragging && pointerId == activeDragPointerId) return true;
        return false;
    }

    private void Update()
    {
        HandleRightMouseDrag();
        HandlePanModeDrag();
    }

    private void HandleRightMouseDrag()
    {
        if (PanModeActive) return;

#if UNITY_EDITOR || UNITY_STANDALONE || UNITY_WEBGL
        if (Input.GetMouseButtonDown(1))
        {
            isDragging = true;
            activeDragPointerId = -2;
            lastPointerPos = Input.mousePosition;
        }
        else if (Input.GetMouseButtonUp(1) && activeDragPointerId == -2)
        {
            isDragging = false;
            activeDragPointerId = int.MinValue;
        }
        else if (Input.GetMouseButton(1) && activeDragPointerId == -2)
        {
            Vector2 cur = Input.mousePosition;
            Vector2 delta = cur - lastPointerPos;
            lastPointerPos = cur;
            ApplyDelta(delta * rightMouseDragSensitivity);
        }
#endif
    }

    private void HandlePanModeDrag()
    {
        if (!PanModeActive) return;

        if (Input.GetMouseButtonDown(0))
        {
            isDragging = true;
            activeDragPointerId = -1;
            lastPointerPos = Input.mousePosition;
        }
        else if (Input.GetMouseButtonUp(0) && activeDragPointerId == -1)
        {
            isDragging = false;
            activeDragPointerId = int.MinValue;
        }
        else if (Input.GetMouseButton(0) && activeDragPointerId == -1)
        {
            Vector2 cur = Input.mousePosition;
            Vector2 delta = cur - lastPointerPos;
            lastPointerPos = cur;
            ApplyDelta(delta);
        }
    }

    private void ApplyDelta(Vector2 screenDelta)
    {
        if (gridContainer == null || canvasRect == null) return;

        float scale = canvasRect.rect.width / Screen.width;
        Vector2 canvasDelta = screenDelta * scale;

        gridContainer.anchoredPosition += canvasDelta;
        if (floatingScoresContainer != null)
            floatingScoresContainer.anchoredPosition += canvasDelta;

        Clamp();
    }

    private void Clamp()
    {
        if (gridContainer == null || canvasRect == null) return;

        var grid = gridContainer.GetComponent<HexGrid>();
        if (grid == null) return;

        float radius = grid.gridRadius;
        float cell = grid.cellSize;

        float halfWidth = (radius + 0.5f) * cell * Mathf.Sqrt(3f);
        float halfHeight = (radius + 0.5f) * cell * 2f * 0.75f;

        float canvasW = canvasRect.rect.width;
        float canvasH = canvasRect.rect.height;

        float maxX = Mathf.Max(0f, halfWidth - canvasW * 0.5f + padding);
        float maxY = Mathf.Max(0f, halfHeight - canvasH * 0.5f + padding);

        Vector2 p = gridContainer.anchoredPosition;
        Vector2 delta = p - originalGridPosition;
        delta.x = Mathf.Clamp(delta.x, -maxX, maxX);
        delta.y = Mathf.Clamp(delta.y, -maxY, maxY);
        gridContainer.anchoredPosition = originalGridPosition + delta;

        if (floatingScoresContainer != null)
        {
            floatingScoresContainer.anchoredPosition = originalFloatingPosition + delta;
        }
    }

    public void ResetPosition()
    {
        if (!originalSaved) return;
        if (gridContainer != null) gridContainer.anchoredPosition = originalGridPosition;
        if (floatingScoresContainer != null) floatingScoresContainer.anchoredPosition = originalFloatingPosition;
    }
}
