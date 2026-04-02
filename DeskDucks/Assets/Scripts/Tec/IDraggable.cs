using UnityEngine;

public interface IDraggable
{
    void OnDragStart(Vector2 worldPos);
    void OnDrag(Vector2 worldPos);
    void OnDragEnd(Vector2 velocity);
}