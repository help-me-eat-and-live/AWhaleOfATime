using UnityEngine;

public class CameraWhaleHardlock : MonoBehaviour
{
    public Transform whale;

    void Start()
    {
        GameObject go = GameObject.FindGameObjectWithTag("Player");
        if (go != null)
        {
            whale = go.transform;
            Debug.Log("🎯 Found Whale: " + whale.name);
        }
        else
        {
            Debug.LogError("❌ Whale GameObject not found. Name must be EXACTLY 'Whale'");
        }
    }

    void LateUpdate()
    {
        if (whale == null) return;

        // FORCE camera to fixed distance
        Vector3 pos = whale.position + new Vector3(0, 20f, -20f);
        transform.position = pos;
        transform.LookAt(whale.position + Vector3.up * 2f);  // Look at whale, offset upward
    }
}
