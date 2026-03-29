using UnityEngine;

public class ClickManager : MonoBehaviour
{
    public LayerMask clickableLayer;

    void OnEnable()
    {
        GlobalMouseHook.OnGlobalMouseClick += HandleClick;
    }

    void OnDisable()
    {
        GlobalMouseHook.OnGlobalMouseClick -= HandleClick;
    }

    void HandleClick(Vector2 screenPixelPos)
    {

        float flippedY = Screen.height - screenPixelPos.y;
        Vector2 worldPos = Camera.main.ScreenToWorldPoint(
            new Vector2(screenPixelPos.x, flippedY));

        Collider2D hit = Physics2D.OverlapPoint(worldPos, clickableLayer);
        if (hit != null)
        {
            hit.GetComponent<IClickable>()?.OnClick();
        }
    }
}