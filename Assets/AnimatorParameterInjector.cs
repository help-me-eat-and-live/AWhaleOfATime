using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;

public class AnimatorParameterInjector : MonoBehaviour
{
    [MenuItem("Tools/Inject Whale Animator Parameters")]
    static void InjectParameters()
    {
        string path = "Assets/WhaleControllerAnimator_FINAL_withParameters.controller";
        var controller = AssetDatabase.LoadAssetAtPath<AnimatorController>(path);

        if (controller == null)
        {
            Debug.LogError("Animator Controller not found at: " + path);
            return;
        }

        AddParameterIfMissing(controller, "Speed", AnimatorControllerParameterType.Float);
        AddParameterIfMissing(controller, "VerticalSpeed", AnimatorControllerParameterType.Float);
        AddParameterIfMissing(controller, "IsUnderwater", AnimatorControllerParameterType.Bool);

        Debug.Log("✅ Whale Animator parameters injected successfully.");
    }

    static void AddParameterIfMissing(AnimatorController controller, string name, AnimatorControllerParameterType type)
    {
        foreach (var param in controller.parameters)
        {
            if (param.name == name) return; // already exists
        }

        controller.AddParameter(name, type);
        Debug.Log("Added parameter: " + name);
    }
}
