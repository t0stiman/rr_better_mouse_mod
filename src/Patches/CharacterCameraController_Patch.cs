using Cameras;
using Character;
using HarmonyLib;
using UI;
using UnityEngine;

namespace better_mouse_mod.Patches;

// todo when switching from 1st to 3rd cam, remember toggle status

/// <summary>
/// move the 1st person camera
/// </summary>
[HarmonyPatch(typeof(CharacterCameraController))]
[HarmonyPatch(nameof(CharacterCameraController.UpdateWithInput))]
public class CharacterCameraController_UpdateWithInput_Patch
{
	private static bool Prefix(
		ref CharacterCameraController __instance,
		float inputZoom,
		bool inputResetZoom)
	{
		if (!Main.MySettings.DisableCameraSmoothing)
		{
			return Constants.EXECUTE_ORIGINAL;
		}
		
		if (!CameraSelector.shared.character._mouseLookInput._mouseMovesCamera)
		{
			return Constants.SKIP_ORIGINAL;
		}
		
		var lookDelta = GameInput.shared.LookDelta;
		
		lookDelta.x *= MouseLookInput.CameraXMultiplier;
		lookDelta.y *= MouseLookInput.CameraYMultiplier;
		
		//vertical
		__instance._targetPitch -= lookDelta.y * __instance.rotationSpeed;
		__instance._targetPitch = Mathf.Clamp(__instance._targetPitch, __instance.minVerticalAngle, __instance.maxVerticalAngle);
		//horizontal
		__instance._targetYaw += lookDelta.x * __instance.rotationSpeed;
		
		//rotate the camera
		__instance._transform.localEulerAngles = new Vector3(__instance._targetPitch, __instance._targetYaw, 0);
		
		__instance.UpdateZoom(inputZoom, inputResetZoom);
		
		return Constants.SKIP_ORIGINAL;
	}
}
