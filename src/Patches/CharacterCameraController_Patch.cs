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
	private static bool Prefix(ref CharacterCameraController __instance)
	{
		if (!Main.MySettings.DisableCameraSmoothing)
		{
			return Constants.EXECUTE_ORIGINAL;
		}
		
		if (!PlayerController._mouseMovesCamera)
		{
			return false;
		}
		
		var lookDelta = GameInput.shared.LookDelta;
		lookDelta.x *= CameraSelector.shared.character.CameraXMultiplier;
		lookDelta.y *= CameraSelector.shared.character.CameraYMultiplier;
		
		//vertical
		__instance._targetPitch -= lookDelta.y * __instance.rotationSpeed;
		__instance._targetPitch = Mathf.Clamp(__instance._targetPitch, __instance.minVerticalAngle, __instance.maxVerticalAngle);
		//horizontal
		__instance._targetYaw += lookDelta.x * __instance.rotationSpeed;
		
		//rotate the camera
		__instance._transform.localEulerAngles = new Vector3(__instance._targetPitch, __instance._targetYaw, 0);
		
		return Constants.SKIP_ORIGINAL;
	}
}
