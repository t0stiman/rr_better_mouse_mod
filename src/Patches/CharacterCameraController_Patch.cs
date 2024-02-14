using Character;
using HarmonyLib;
using UnityEngine;

namespace better_mouse_mod.Patches;

/// <summary>
/// __instance patch is for the 1st person camera (PlayerController_Patch is too)
/// </summary>
[HarmonyPatch(typeof(CharacterCameraController))]
[HarmonyPatch(nameof(CharacterCameraController.UpdateWithInput))]
public class CharacterCameraController_UpdateWithInput_Patch
{
	private static bool Prefix(ref CharacterCameraController __instance, float inputPitch, float inputYaw, Lean lean)
	{
		if (!Main.MySettings.DisableCameraSmoothing)
		{
			return true; //execute original function
		}
		
		__instance._lastLean = lean;
		
		//vertical
		__instance._targetPitch -= inputPitch * __instance.rotationSpeed;
		__instance._targetPitch = Mathf.Clamp(__instance._targetPitch, __instance.minVerticalAngle, __instance.maxVerticalAngle);
		//horizontal
		__instance._targetYaw += inputYaw * __instance.rotationSpeed;
		
		//rotate the camera
		__instance._transform.localEulerAngles = new Vector3(__instance._targetPitch, __instance._targetYaw, 0);
		// __instance._transform.localRotation = Quaternion.Slerp(__instance._transform.localRotation, Quaternion.Euler(__instance._targetPitch, __instance._targetYaw, 0.0f), 1);
		
		return false; //skip original function
	}
}
