using UnityEngine;

[CreateAssetMenu(menuName = "WhaleTricks/Trick", fileName = "NewTrick")]
public class Trick : ScriptableObject
{
    public string trickName = "Spin Flip";
    public KeyCode activationKey = KeyCode.Alpha1;
    public Vector3 rotationAxis = Vector3.forward;
    public float rotationAmount = 360f;
    public int score = 100;
    public float cooldown = 2f;
    public float staminaCost = 10f;
    [HideInInspector] public float lastUsedTime = -Mathf.Infinity;
}
