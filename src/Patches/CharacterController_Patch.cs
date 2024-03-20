using Character;
using HarmonyLib;
using UnityEngine;
using CharacterController = Character.CharacterController;

namespace better_mouse_mod.Patches;

/// <summary>
/// Make the characters rotation match the 1st person camera
/// </summary>
[HarmonyPatch(typeof(CharacterController))]
[HarmonyPatch(nameof(CharacterController.SetInputs))]
public static class CharacterController_SetInputs_Patch
{
	private static void Postfix(ref CharacterController __instance, PlayerCharacterInputs inputs)
	{
		if (!Main.MySettings.DisableCameraSmoothing)
		{
			return;
		}
		
		var firstPersonCamHorizontalRotation = CameraSelector.shared.character.cameraController._targetYaw;
		__instance.motor.RotateCharacter(Quaternion.Euler(__instance.transform.localRotation.x, firstPersonCamHorizontalRotation, __instance.transform.localRotation.z));
	}
}