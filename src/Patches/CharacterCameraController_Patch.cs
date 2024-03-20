using Character;
using HarmonyLib;
using UI;
using UnityEngine;

namespace better_mouse_mod.Patches;

/// <summary>
/// move the 1st person camera
/// </summary>
[HarmonyPatch(typeof(CharacterCameraController))]
[HarmonyPatch(nameof(CharacterCameraController.UpdateWithInput))]
public class CharacterCameraController_UpdateWithInput_Patch
{
	private static bool Prefix(ref CharacterCameraController __instance, Lean lean)
	{
		if (!Main.MySettings.DisableCameraSmoothing)
		{
			return true; //execute original function
		}

		if (!PlayerController._mouseMovesCamera)
		{
			return false;
		}
		
		if (lean != __instance._lastLean)
		{
			__instance._targetYaw = 0.0f;
			__instance._lastLean = lean;
		}

		// X and Y multiplier should be the same
		var lookDelta = GameInput.shared.LookDelta * CameraSelector.shared.character.CameraXMultiplier;
		
		//vertical
		__instance._targetPitch -= lookDelta.y * __instance.rotationSpeed;
		__instance._targetPitch = Mathf.Clamp(__instance._targetPitch, __instance.minVerticalAngle, __instance.maxVerticalAngle);
		//horizontal
		__instance._targetYaw += lookDelta.x * __instance.rotationSpeed;
		
		//rotate the camera
		__instance._transform.eulerAngles = new Vector3(__instance._targetPitch, __instance._targetYaw, 0);
		
		return false; //skip original function
	}
}
