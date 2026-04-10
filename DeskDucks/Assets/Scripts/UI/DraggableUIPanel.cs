using UnityEngine;
using UnityEngine.EventSystems;

public class DraggableUIPanel : MonoBehaviour, IPointerDownHandler, IDragHandler
{
    private const string PanelPosXKey = "SettingsPanel_PosX";
    private const string PanelPosYKey = "SettingsPanel_PosY";

    [SerializeField] private RectTransform panelToMove;
    [SerializeField] private Canvas canvas;

    private RectTransform canvasRect;
    private Vector2 pointerOffset;

    void Awake()
    {
        if (panelToMove == null)
            panelToMove = transform.parent as RectTransform;

        if (canvas == null)
            canvas = GetComponentInParent<Canvas>();

        if (canvas != null)
            canvasRect = canvas.transform as RectTransform;

        LoadPosition();
        ClampToCanvas();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (panelToMove == null || canvasRect == null)
            return;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRect,
            eventData.position,
            eventData.pressEventCamera,
            out Vector2 localPointerPosition
        );

        pointerOffset = panelToMove.anchoredPosition - localPointerPosition;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (panelToMove == null || canvasRect == null)
            return;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRect,
            eventData.position,
            eventData.pressEventCamera,
            out Vector2 localPointerPosition
        );

        panelToMove.anchoredPosition = localPointerPosition + pointerOffset;
        ClampToCanvas();
        SavePosition();
    }

    void LoadPosition()
    {
        if (panelToMove == null)
            return;

        float x = PlayerPrefs.GetFloat(PanelPosXKey, panelToMove.anchoredPosition.x);
        float y = PlayerPrefs.GetFloat(PanelPosYKey, panelToMove.anchoredPosition.y);
        panelToMove.anchoredPosition = new Vector2(x, y);
    }

    void SavePosition()
    {
        if (panelToMove == null)
            return;

        PlayerPrefs.SetFloat(PanelPosXKey, panelToMove.anchoredPosition.x);
        PlayerPrefs.SetFloat(PanelPosYKey, panelToMove.anchoredPosition.y);
        PlayerPrefs.Save();
    }

    void ClampToCanvas()
    {
        if (panelToMove == null || canvasRect == null)
            return;

        Vector3[] canvasCorners = new Vector3[4];
        Vector3[] panelCorners = new Vector3[4];

        canvasRect.GetWorldCorners(canvasCorners);
        panelToMove.GetWorldCorners(panelCorners);

        Vector3 offset = Vector3.zero;

        if (panelCorners[0].x < canvasCorners[0].x)
            offset.x = canvasCorners[0].x - panelCorners[0].x;
        else if (panelCorners[2].x > canvasCorners[2].x)
            offset.x = canvasCorners[2].x - panelCorners[2].x;

        if (panelCorners[0].y < canvasCorners[0].y)
            offset.y = canvasCorners[0].y - panelCorners[0].y;
        else if (panelCorners[2].y > canvasCorners[2].y)
            offset.y = canvasCorners[2].y - panelCorners[2].y;

        panelToMove.position += offset;
    }
}