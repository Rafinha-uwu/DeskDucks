using System.Runtime.InteropServices;
using UnityEngine;

public class ClickManager : MonoBehaviour
{
    [DllImport("user32.dll")]
    private static extern bool GetCursorPos(out POINT lpPoint);

    [StructLayout(LayoutKind.Sequential)]
    private struct POINT
    {
        public int x;
        public int y;
    }

    [Header("References")]
    public LayerMask clickableLayer;
    public WindowController window;

    [Header("Drag Settings")]
    public float dragThreshold = 0.15f;
    public float throwMultiplier = 0.35f;

    private Camera mainCamera;

    private IClickable currentClickable;
    private IDraggable currentDraggable;

    private bool dragging;
    private bool isClickCandidate;

    private Vector2 mouseDownWorld;
    private Vector2 lastWorld;
    private Vector2 velocity;

    void Awake()
    {
        mainCamera = Camera.main;
    }

    void OnEnable()
    {
        GlobalMouseHook.OnMouseDown += HandleDown;
        GlobalMouseHook.OnMouseUp += HandleUp;
    }

    void OnDisable()
    {
        GlobalMouseHook.OnMouseDown -= HandleDown;
        GlobalMouseHook.OnMouseUp -= HandleUp;
    }

    void Update()
    {
        Vector2 world = GetGlobalMouseWorld();

        if (currentClickable == null && currentDraggable == null)
        {
            Collider2D hoverHit = Physics2D.OverlapPoint(world, clickableLayer);
            window.SetClickThrough(hoverHit == null);
            return;
        }

        window.SetClickThrough(false);

        velocity = (world - lastWorld) / Mathf.Max(Time.deltaTime, 0.0001f);

        if (!dragging && currentDraggable != null && Vector2.Distance(world, mouseDownWorld) > dragThreshold)
        {
            dragging = true;
            isClickCandidate = false;
            currentDraggable.OnDragStart(mouseDownWorld);
        }

        if (dragging && currentDraggable != null)
        {
            currentDraggable.OnDrag(world);
        }

        lastWorld = world;
    }

    void HandleDown(Vector2 screenPos)
    {
        Vector2 world = ScreenToWorld(screenPos);
        Collider2D hit = Physics2D.OverlapPoint(world, clickableLayer);

        if (hit == null)
        {
            ClearInteractionState();
            return;
        }

        currentClickable = hit.GetComponent<IClickable>();
        currentDraggable = hit.GetComponent<IDraggable>();

        if (currentClickable == null && currentDraggable == null)
        {
            ClearInteractionState();
            return;
        }

        mouseDownWorld = world;
        lastWorld = world;
        velocity = Vector2.zero;

        dragging = false;
        isClickCandidate = currentClickable != null;
    }

    void HandleUp(Vector2 screenPos)
    {
        if (isClickCandidate && currentClickable != null)
        {
            currentClickable.OnClick();
        }
        else if (dragging && currentDraggable != null)
        {
            currentDraggable.OnDragEnd(velocity * throwMultiplier);
        }

        ClearInteractionState();
    }

    void ClearInteractionState()
    {
        currentClickable = null;
        currentDraggable = null;
        dragging = false;
        isClickCandidate = false;
    }

    Vector2 GetGlobalMouseWorld()
    {
        GetCursorPos(out POINT point);
        return ScreenToWorld(new Vector2(point.x, point.y));
    }

    Vector2 ScreenToWorld(Vector2 globalScreenPos)
    {
        float flippedY = Screen.height - globalScreenPos.y;
        return mainCamera.ScreenToWorldPoint(new Vector3(globalScreenPos.x, flippedY, 0f));
    }
}