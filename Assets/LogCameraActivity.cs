using UnityEngine;

public class CameraChangeSniffer : MonoBehaviour
{
    Camera lastMain;

    void Start()
    {
        lastMain = Camera.main;
        Debug.Log("👁️ Initial camera flags: " + lastMain.clearFlags);
    }

    void Update()
    {
        if (Camera.main.clearFlags != lastMain.clearFlags)
        {
            Debug.LogWarning("🚨 Something changed the camera clearFlags to: " + Camera.main.clearFlags);
            lastMain = Camera.main;
        }
    }
}
