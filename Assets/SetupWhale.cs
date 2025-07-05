using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class SetupWhale : MonoBehaviour
{
    void Awake()
    {
        // Attach WhaleTrickController if missing
        if (GetComponent<WhaleTrickController>() == null)
            gameObject.AddComponent<WhaleTrickController>();

        // TrickRegistry is a ScriptableObject, not added via AddComponent

        Rigidbody rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
        rb.linearDamping = 1.5f;
        rb.angularDamping = 2f;

        Debug.Log("✅ Whale setup complete.");
    }

    void Start()
    {
        Camera cam = Camera.main;
        if (cam != null)
        {
            // Attach follow cam if missing
            if (cam.GetComponent<WhaleFollowCamera>() == null)
                cam.gameObject.AddComponent<WhaleFollowCamera>();

            // Attach underwater effect if needed
            if (cam.GetComponent<UnderwaterEffect>() == null)
                cam.gameObject.AddComponent<UnderwaterEffect>();

            Debug.Log("🎥 Main camera setup complete.");
        }
        else
        {
            Debug.LogWarning("⚠️ No Main Camera found.");
        }
    }
}
