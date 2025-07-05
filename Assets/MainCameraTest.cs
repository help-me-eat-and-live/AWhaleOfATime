using UnityEngine;

public class CameraDetector : MonoBehaviour
{
    void Start()
    {
        Debug.Log("🎥 Active Camera: " + Camera.main.name);
        Debug.Log("📍 Position: " + Camera.main.transform.position);
        Debug.Log("🧭 Rotation: " + Camera.main.transform.eulerAngles);
    }
}
