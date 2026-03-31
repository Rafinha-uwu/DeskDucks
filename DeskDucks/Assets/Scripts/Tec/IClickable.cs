using UnityEngine;

public interface IClickable
{
    void OnClick();
    void OnDragStart(Vector2 worldPos);
    void OnDrag(Vector2 worldPos);
    void OnDragEnd(Vector2 velocity);
}