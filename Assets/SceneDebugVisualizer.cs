using UnityEngine;

public class SceneDebugVisualizer : MonoBehaviour
{
    public Transform target;

    void Update()
    {
        if (target)
        {
            Debug.DrawLine(transform.position, target.position, Color.red);
            Debug.DrawRay(target.position, Vector3.up * 5, Color.green);
        }

        Debug.Log("🧭 Camera Position: " + transform.position + " | Rotation: " + transform.rotation.eulerAngles);
    }

    void OnGUI()
    {
        GUI.Label(new Rect(10, 10, 500, 20), "🟢 SceneDebugVisualizer Running");
    }
}
