using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(WindowController))]
[RequireComponent(typeof(GameplaySpaceManager))]
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
    [SerializeField] private LayerMask clickableLayer;
    [SerializeField] private WorldObjectContextMenu contextMenu;
    [SerializeField] private WorldPlacementManager placementManager;

    [Header("Drag Settings")]
    [SerializeField] private float dragThreshold = 0.15f;
    [SerializeField] private float throwMultiplier = 0.35f;

    private WindowController window;
    private GameplaySpaceManager gameplaySpace;
    private EventSystem eventSystem;

    private IClickable currentClickable;
    private IDraggable currentDraggable;

    private bool dragging;
    private bool isClickCandidate;

    private Vector2 mouseDownWorld;
    private Vector2 lastWorld;
    private Vector2 velocity;

    private readonly List<RaycastResult> uiRaycastResults = new();
    private readonly Collider2D[] overlapResults = new Collider2D[32];

    void Awake()
    {
        window = GetComponent<WindowController>();
        gameplaySpace = GetComponent<GameplaySpaceManager>();
        eventSystem = EventSystem.current;

        if (placementManager == null)
            placementManager = WorldPlacementManager.Instance;
    }

    void OnEnable()
    {
        GlobalMouseHook.OnMouseDown += HandleDown;
        GlobalMouseHook.OnMouseUp += HandleUp;
        GlobalMouseHook.OnRightMouseDown += HandleRightDown;
    }

    void OnDisable()
    {
        GlobalMouseHook.OnMouseDown -= HandleDown;
        GlobalMouseHook.OnMouseUp -= HandleUp;
        GlobalMouseHook.OnRightMouseDown -= HandleRightDown;
    }

    void Update()
    {
        if (window == null || gameplaySpace == null)
            return;

        if (eventSystem == null)
            eventSystem = EventSystem.current;

        if (placementManager == null)
            placementManager = WorldPlacementManager.Instance;

        if (placementManager != null && placementManager.IsPlacing)
        {
            window.SetClickThrough(false);
            return;
        }

        Vector2 globalMouseScreen = GetGlobalMouseScreenPosition();
        Vector2 world = gameplaySpace.GlobalScreenToWorld(globalMouseScreen);

        if (currentClickable == null && currentDraggable == null)
        {
            bool isOverUi = IsPointerOverUi(globalMouseScreen);
            bool isOverWorldClickable = GetTopmostClickableCollider(world) != null;

            window.SetClickThrough(!isOverUi && !isOverWorldClickable);
            return;
        }

        window.SetClickThrough(false);

        velocity = (world - lastWorld) / Mathf.Max(Time.deltaTime, 0.0001f);

        if (!dragging &&
            currentDraggable != null &&
            Vector2.Distance(world, mouseDownWorld) > dragThreshold)
        {
            dragging = true;
            isClickCandidate = false;
            currentDraggable.OnDragStart(mouseDownWorld);
        }

        if (dragging && currentDraggable != null)
            currentDraggable.OnDrag(world);

        lastWorld = world;
    }

    void HandleDown(Vector2 screenPos)
    {
        if (gameplaySpace == null)
            return;

        if (eventSystem == null)
            eventSystem = EventSystem.current;

        if (placementManager == null)
            placementManager = WorldPlacementManager.Instance;

        if (placementManager != null && placementManager.IsPlacing)
        {
            ClearInteractionState();
            contextMenu?.Hide();
            placementManager.ConfirmCurrentPlacement();
            return;
        }

        bool isOverUi = IsPointerOverUi(screenPos);

        if (isOverUi)
        {
            ClearInteractionState();
            window.SetClickThrough(false);
            return;
        }

        if (contextMenu != null && contextMenu.IsVisible())
            contextMenu.Hide();

        Vector2 world = gameplaySpace.GlobalScreenToWorld(screenPos);
        Collider2D hit = GetTopmostClickableCollider(world);

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
        if (placementManager == null)
            placementManager = WorldPlacementManager.Instance;

        if (placementManager != null && placementManager.IsPlacing)
            return;

        if (isClickCandidate && currentClickable != null)
            currentClickable.OnClick();
        else if (dragging && currentDraggable != null)
            currentDraggable.OnDragEnd(velocity * throwMultiplier);

        ClearInteractionState();
    }

    void HandleRightDown(Vector2 screenPos)
    {
        if (gameplaySpace == null)
            return;

        if (eventSystem == null)
            eventSystem = EventSystem.current;

        if (placementManager == null)
            placementManager = WorldPlacementManager.Instance;

        if (placementManager != null && placementManager.IsPlacing)
        {
            ClearInteractionState();
            contextMenu?.Hide();
            placementManager.ConfirmCurrentPlacement();
            return;
        }

        if (IsPointerOverUi(screenPos))
            return;

        Vector2 world = gameplaySpace.GlobalScreenToWorld(screenPos);
        Collider2D hit = GetTopmostClickableCollider(world);

        if (hit == null)
        {
            contextMenu?.Hide();
            return;
        }

        IWorldContextActions contextTarget = hit.GetComponent<IWorldContextActions>();
        if (contextTarget == null || !contextTarget.CanShowContextMenu())
        {
            contextMenu?.Hide();
            return;
        }

        contextMenu?.Show(contextTarget);
    }

    void ClearInteractionState()
    {
        currentClickable = null;
        currentDraggable = null;
        dragging = false;
        isClickCandidate = false;
        velocity = Vector2.zero;
    }

    Vector2 GetGlobalMouseScreenPosition()
    {
        GetCursorPos(out POINT point);
        return new Vector2(point.x, point.y);
    }

    bool IsPointerOverUi(Vector2 globalScreenPos)
    {
        if (eventSystem == null)
            return false;

        PointerEventData pointerData = new(eventSystem)
        {
            position = new Vector2(globalScreenPos.x, Screen.height - globalScreenPos.y)
        };

        uiRaycastResults.Clear();
        eventSystem.RaycastAll(pointerData, uiRaycastResults);

        return uiRaycastResults.Count > 0;
    }

    Collider2D GetTopmostClickableCollider(Vector2 worldPos)
    {
        int hitCount = Physics2D.OverlapPointNonAlloc(worldPos, overlapResults, clickableLayer);

        if (hitCount <= 0)
            return null;

        Collider2D bestCollider = null;
        int bestSortingLayerValue = int.MinValue;
        int bestSortingOrder = int.MinValue;

        for (int i = 0; i < hitCount; i++)
        {
            Collider2D candidate = overlapResults[i];
            if (candidate == null)
                continue;

            if (!HasClickableBehavior(candidate))
                continue;

            if (!TryGetTopRenderSort(candidate, out int sortingLayerValue, out int sortingOrder))
            {
                sortingLayerValue = 0;
                sortingOrder = 0;
            }

            bool isBetter =
                bestCollider == null ||
                sortingLayerValue > bestSortingLayerValue ||
                (sortingLayerValue == bestSortingLayerValue && sortingOrder > bestSortingOrder);

            if (!isBetter)
                continue;

            bestCollider = candidate;
            bestSortingLayerValue = sortingLayerValue;
            bestSortingOrder = sortingOrder;
        }

        return bestCollider;
    }

    bool HasClickableBehavior(Collider2D collider)
    {
        return collider.GetComponent<IClickable>() != null ||
               collider.GetComponent<IDraggable>() != null ||
               collider.GetComponent<IWorldContextActions>() != null;
    }

    bool TryGetTopRenderSort(Collider2D collider, out int sortingLayerValue, out int sortingOrder)
    {
        sortingLayerValue = int.MinValue;
        sortingOrder = int.MinValue;

        bool found = false;

        Renderer[] ownAndChildren = collider.GetComponentsInChildren<Renderer>(true);
        Renderer[] parents = collider.GetComponentsInParent<Renderer>(true);

        EvaluateRendererArray(ownAndChildren, ref found, ref sortingLayerValue, ref sortingOrder);
        EvaluateRendererArray(parents, ref found, ref sortingLayerValue, ref sortingOrder);

        return found;
    }

    void EvaluateRendererArray(Renderer[] renderers, ref bool found, ref int bestLayerValue, ref int bestOrder)
    {
        if (renderers == null)
            return;

        for (int i = 0; i < renderers.Length; i++)
        {
            Renderer r = renderers[i];
            if (r == null || !r.enabled)
                continue;

            int layerValue = SortingLayer.GetLayerValueFromID(r.sortingLayerID);
            int order = r.sortingOrder;

            if (!found ||
                layerValue > bestLayerValue ||
                (layerValue == bestLayerValue && order > bestOrder))
            {
                found = true;
                bestLayerValue = layerValue;
                bestOrder = order;
            }
        }
    }
}