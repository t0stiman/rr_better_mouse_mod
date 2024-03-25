using Character;
using HarmonyLib;
using UnityEngine;
using CharacterController = Character.CharacterController;

namespace better_mouse_mod.Patches;

/// <summary>
/// Make the characters rotation match the 1st person camera
/// </summary>
[HarmonyPatch(typeof(CharacterController))]
[HarmonyPatch(nameof(CharacterController.GetMotionSnapshot))]
public static class CharacterController_GetMotionSnapshot_Patch
{
  //todo test ladders
  private static bool Prefix(ref CharacterController __instance, ref MotionSnapshot __result)
  {
    if (!Main.MySettings.DisableCameraSmoothing)
    {
      return Constants.EXECUTE_ORIGINAL;
    }
    
    //this line is different:
    var cameraRotation = __instance.cameraContainer.rotation;
    
    var bodyRotation = cameraRotation;
    if (__instance._ladder != null)
    {
      bodyRotation = Quaternion.Euler(0.0f, 180f, 0.0f) * __instance._ladder.transform.rotation;
    }
    if (__instance._seat != null)
    {
      bodyRotation = __instance._seat.transform.rotation;
    }

    __result = new MotionSnapshot(__instance.motor.TransientPosition, bodyRotation, cameraRotation, __instance.motor.Velocity);

    return Constants.SKIP_ORIGINAL;
  }
}

/// <summary>
/// IDFK why this patch is necessary but without it leaning doesn't work
/// </summary>
[HarmonyPatch(typeof(CharacterController))]
[HarmonyPatch(nameof(CharacterController.AfterCharacterUpdate))]
public static class CharacterController_AfterCharacterUpdate_Patch
{
  private static Lean previousLean = (Lean)99;
  private static Vector3 localGoal = Vector3.zero;
  
  //todo test ladders
  private static bool Prefix(ref CharacterController __instance, float deltaTime)
  {
    if (!Main.MySettings.DisableCameraSmoothing)
    {
      return Constants.EXECUTE_ORIGINAL;
    }
    
    // =============================================
    
    switch (__instance.CurrentCharacterState)
    {
      case CharacterState.Default:
        if (__instance._jumpRequested && __instance._timeSinceJumpRequested > (double) __instance.jumpPreGroundingGraceTime)
          __instance._jumpRequested = false;
        if ((__instance.allowJumpingWhenSliding ? (__instance.motor.GroundingStatus.FoundAnyGround ? 1 : 0) : (__instance.motor.GroundingStatus.IsStableOnGround ? 1 : 0)) != 0)
        {
          if (!__instance._jumpedThisFrame)
            __instance._jumpConsumed = false;
          __instance._timeSinceLastAbleToJump = 0.0f;
        }
        else
        {
          __instance._timeSinceLastAbleToJump += deltaTime;
        }

        if (__instance._isCrouching && !__instance._shouldBeCrouching)
        {
          __instance.SetCrouched(false);
          if (__instance.motor.CharacterOverlap(__instance.motor.TransientPosition, __instance.motor.TransientRotation, __instance._probedColliders, __instance.motor.CollidableLayers, QueryTriggerInteraction.Ignore) > 0)
          {
            __instance.SetCrouched(true);
            break;
          }
          __instance._isCrouching = false;
        }
        break;
      case CharacterState.Seated:
        bool flag = __instance._anchoringTimer >= 0.15000000596046448;
        switch (__instance._attachState)
        {
          case CharacterController.AttachState.Anchoring:
            if (flag)
            {
              __instance.motor.SetMovementCollisionsSolvingActivation(true);
              __instance._attachState = CharacterController.AttachState.Stable;
              break;
            }
            __instance._anchoringTimer += deltaTime;
            __instance.SetCameraHeightParameter(__instance.AnchoringParameter);
            break;
          case CharacterController.AttachState.Deanchoring:
            if (flag)
            {
              __instance.TransitionToState(CharacterState.Default);
              break;
            }
            __instance._anchoringTimer += deltaTime;
            __instance.SetCameraHeightParameter(1f - __instance.AnchoringParameter);
            break;
        }
        __instance._seatStickyRemaining -= deltaTime;
        break;
      case CharacterState.Ladder:
        __instance._ladderDuration += deltaTime;
        break;
    }
    
    // =============================================
    
    var camera = __instance.cameraContainer;
    
    if (previousLean != __instance.Lean)
    {
      var cameraOffset = 0.0f;
      switch (__instance.Lean)
      {
        case Lean.Left:
          cameraOffset = -0.5f;
          break;
        case Lean.Right:
          cameraOffset = 0.5f;
          break;
      }
      
      localGoal = cameraOffset * camera.right;
      
      previousLean = __instance.Lean;
    }

    var t = deltaTime * 10f;
    
    camera.localPosition = new Vector3(
      Mathf.Lerp(camera.localPosition.x, localGoal.x, t),
      Mathf.Lerp(__instance.eyeHeightStanding, __instance.eyeHeightSeated, __instance._seatedParameter),
      Mathf.Lerp(camera.localPosition.z, localGoal.z, t)
    );
    
    return Constants.SKIP_ORIGINAL;
  }
}