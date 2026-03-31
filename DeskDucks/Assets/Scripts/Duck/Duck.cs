using UnityEngine;

[RequireComponent(typeof(SimpleGravity))]
public class Duck : MonoBehaviour, IClickable
{
    private SimpleGravity gravity;
    private Vector3 dragOffset;

    void Awake()
    {
        gravity = GetComponent<SimpleGravity>();
    }

    public void OnClick()
    {
        Application.Quit();
    }

    public void OnDragStart(Vector2 worldPos)
    {
        dragOffset = transform.position - (Vector3)worldPos;
        gravity.ResetVelocity();
        gravity.SetEnabled(false);
    }

    public void OnDrag(Vector2 worldPos)
    {
        transform.position = worldPos + (Vector2)dragOffset;
    }

    public void OnDragEnd(Vector2 velocity)
    {
        gravity.SetEnabled(true);
        gravity.SetVelocity(velocity);
    }
}