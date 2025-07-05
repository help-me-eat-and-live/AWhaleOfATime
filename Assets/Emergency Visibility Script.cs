using UnityEngine;

public class EmergencyCameraAlign : MonoBehaviour
{
    void Start()
    {
        var cam = Camera.main;
        var whale = GameObject.Find("humpback_whale_exp1_0");

        if (cam != null && whale != null)
        {
            cam.transform.position = whale.transform.position + new Vector3(0, 2, -10);
            cam.transform.LookAt(whale.transform.position);
            Debug.Log("📷 Camera repositioned to focus on whale.");
        }
        else
        {
            Debug.LogWarning("❌ Whale or Main Camera not found!");
        }
    }
}
