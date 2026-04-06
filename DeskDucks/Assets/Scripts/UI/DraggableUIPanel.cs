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
}