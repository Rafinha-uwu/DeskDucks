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

    public LayerMask clickableLayer;
    public TransparentWindow window;
    public float dragThreshold = 0.15f;
    public float throwMultiplier = 0.35f;

    private IClickable current;
    private bool dragging;
    private bool isClickCandidate;

    private Vector2 mouseDownWorld;
    private Vector2 lastWorld;
    private Vector2 velocity;

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

        if (current == null)
        {
            Collider2D hoverHit = Physics2D.OverlapPoint(world, clickableLayer);
            window.SetClickThrough(hoverHit == null);
            return;
        }

        window.SetClickThrough(false);

        velocity = (world - lastWorld) / Mathf.Max(Time.deltaTime, 0.0001f);

        if (!dragging && Vector2.Distance(world, mouseDownWorld) > dragThreshold)
        {
            dragging = true;
            isClickCandidate = false;
            current.OnDragStart(mouseDownWorld);
        }

        if (dragging)
        {
            current.OnDrag(world);
        }

        lastWorld = world;
    }

    void HandleDown(Vector2 screenPos)
    {
        Vector2 world = ScreenToWorld(screenPos);
        Collider2D hit = Physics2D.OverlapPoint(world, clickableLayer);

        if (hit == null)
        {
            current = null;
            dragging = false;
            isClickCandidate = false;
            return;
        }

        current = hit.GetComponent<IClickable>();
        if (current == null)
        {
            dragging = false;
            isClickCandidate = false;
            return;
        }

        mouseDownWorld = world;
        lastWorld = world;
        velocity = Vector2.zero;

        dragging = false;
        isClickCandidate = true;
    }

    void HandleUp(Vector2 screenPos)
    {
        if (current != null)
        {
            if (isClickCandidate)
            {
                current.OnClick();
            }
            else if (dragging)
            {
                current.OnDragEnd(velocity * throwMultiplier);
            }
        }

        current = null;
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
        return Camera.main.ScreenToWorldPoint(new Vector3(globalScreenPos.x, flippedY, 0f));
    }
}