using UnityEngine;

public class SimpleCameraTest : MonoBehaviour
{
    public Transform whale;
    
    void Update()
    {
        if (whale != null)
        {
            // Simple camera positioning behind whale
            Vector3 targetPosition = whale.position + new Vector3(0, 3, -8);
            transform.position = Vector3.Lerp(transform.position, targetPosition, 2f * Time.deltaTime);
            
            // Look at whale
            transform.LookAt(whale.position + Vector3.up * 1);
        }
    }
}