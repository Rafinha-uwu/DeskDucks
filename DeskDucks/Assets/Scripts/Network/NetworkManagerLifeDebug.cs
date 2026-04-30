using UnityEngine;

public class NetworkManagerLifeDebug : MonoBehaviour
{
    private void OnEnable()
    {
        Debug.Log("NetworkManager object ENABLED");
    }

    private void OnDisable()
    {
        Debug.LogError("NetworkManager object DISABLED");
    }

    private void OnDestroy()
    {
        Debug.LogError("NetworkManager object DESTROYED");
    }

    private void OnApplicationQuit()
    {
        Debug.Log("Application quitting");
    }
}