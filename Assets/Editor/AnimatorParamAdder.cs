// Assets/Editor/AnimatorParamAdder.cs
using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;

public class AnimatorParamAdder : MonoBehaviour
{
    [MenuItem("Tools/Fix Missing Whale Animator Parameters")]
    private static void FixWhaleAnimatorParameters()
    {
        string controllerPath = "Assets/WhaleControllerAnimator.controller";
        AnimatorController controller = AssetDatabase.LoadAssetAtPath<AnimatorController>(controllerPath);

        if (!controller)
        {
            Debug.LogError($"AnimatorController not found at: {controllerPath}");
            return;
        }

        AddParameterIfMissing(controller, "Speed", AnimatorControllerParameterType.Float);
        AddParameterIfMissing(controller, "VerticalSpeed", AnimatorControllerParameterType.Float);
        AddParameterIfMissing(controller, "IsUnderwater", AnimatorControllerParameterType.Bool);

        EditorUtility.SetDirty(controller);
        AssetDatabase.SaveAssets();

        Debug.Log("✅ Whale Animator parameters verified and updated if needed.");
    }

    private static void AddParameterIfMissing(AnimatorController controller, string name, AnimatorControllerParameterType type)
    {
        foreach (var param in controller.parameters)
        {
            if (param.name == name)
                return;
        }

        controller.AddParameter(name, type);
        Debug.Log($"➕ Added missing parameter: {name} ({type})");
    }
}
