using UnityEngine;

public class Duck : MonoBehaviour, IClickable
{
    public void OnClick()
    {
        gameObject.SetActive(false);
    }
}