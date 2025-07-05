using UnityEditor;
using UnityEngine;
using UnityEditor.Animations;

public class CreateWhaleAnimator
{
    [MenuItem("Tools/Create Whale Animator Controller")]
    public static void CreateAnimator()
    {
        string controllerPath = "Assets/WhaleControllerAnimator.controller";
        AnimatorController animatorController = AnimatorController.CreateAnimatorControllerAtPath(controllerPath);

        // Parameters
        animatorController.AddParameter("SwimSpeed", AnimatorControllerParameterType.Float);
        animatorController.AddParameter("IsJumping", AnimatorControllerParameterType.Bool);
        animatorController.AddParameter("IsDoingTrick", AnimatorControllerParameterType.Bool);
        animatorController.AddParameter("DoTrick", AnimatorControllerParameterType.Trigger);

        // Load Animation Clips
        AnimationClip idleClip = AssetDatabase.LoadAssetAtPath<AnimationClip>("Assets/Humpback whale/swim.anim");
        AnimationClip swimClip = AssetDatabase.LoadAssetAtPath<AnimationClip>("Assets/Humpback whale/fastswim.anim");
        AnimationClip jumpClip = AssetDatabase.LoadAssetAtPath<AnimationClip>("Assets/Humpback whale/dive.anim");
        AnimationClip trickClip = AssetDatabase.LoadAssetAtPath<AnimationClip>("Assets/Humpback whale/fastswim2.anim");

        AnimatorStateMachine sm = animatorController.layers[0].stateMachine;

        AnimatorState idleState = sm.AddState("Idle");
        if (idleClip) idleState.motion = idleClip; else Debug.LogWarning("Idle clip not found");

        AnimatorState swimState = sm.AddState("Swim");
        if (swimClip) swimState.motion = swimClip; else Debug.LogWarning("Swim clip not found");

        AnimatorState jumpState = sm.AddState("Jump");
        if (jumpClip) jumpState.motion = jumpClip; else Debug.LogWarning("Jump clip not found");

        AnimatorState trickState = sm.AddState("Trick");
        if (trickClip) trickState.motion = trickClip; else Debug.LogWarning("Trick clip not found");

        // Transitions
        AnimatorStateTransition idleToSwim = idleState.AddTransition(swimState);
        idleToSwim.AddCondition(AnimatorConditionMode.Greater, 0.1f, "SwimSpeed");

        AnimatorStateTransition swimToIdle = swimState.AddTransition(idleState);
        swimToIdle.AddCondition(AnimatorConditionMode.Less, 0.1f, "SwimSpeed");

        AnimatorStateTransition jumpTrans = sm.AddAnyStateTransition(jumpState);
        jumpTrans.AddCondition(AnimatorConditionMode.If, 0, "IsJumping");
        jumpTrans.hasExitTime = false;

        AnimatorStateTransition trickTrans = sm.AddAnyStateTransition(trickState);
        trickTrans.AddCondition(AnimatorConditionMode.If, 0, "IsDoingTrick");
        trickTrans.hasExitTime = false;
        trickTrans.duration = 0;

        AssetDatabase.SaveAssets();

        // Auto-assign to Whale object
        GameObject whale = GameObject.Find("Whale");
        if (whale)
        {
            Animator animator = whale.GetComponent<Animator>();
            if (!animator) animator = whale.AddComponent<Animator>();
            animator.runtimeAnimatorController = animatorController;
            Debug.Log("✅ Assigned WhaleControllerAnimator to Whale GameObject");
        }
        else
        {
            Debug.LogWarning("⚠️ Whale GameObject not found in scene to assign animator.");
        }

        Debug.Log("✅ WhaleControllerAnimator.controller created successfully.");
    }
}
