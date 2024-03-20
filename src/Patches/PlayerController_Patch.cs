using better_mouse_mod.Extensions;
using Character;
using HarmonyLib;
using UI;
using UnityEngine;

namespace better_mouse_mod.Patches;

[HarmonyPatch(typeof(PlayerController))]
[HarmonyPatch(nameof(PlayerController.JumpToCar))]
public class PlayerController_JumpToCar_Patch
{
	private static bool Prefix(Model.Car car)
	{
		if (!Main.MySettings.SwitchCarsWithButton)
		{
			return true;
		}

		TrainController.Shared.SelectedCar = car;
		CameraSelector.shared.JumpToSeatWithoutCameraChange();
		return false; //skip original function
	}
}

/// <summary>
/// this patch is for moving the player character while in 1st person view
/// </summary>
[HarmonyPatch(typeof(PlayerController))]
[HarmonyPatch(nameof(PlayerController.Update))]
public class PlayerController_Update_Patch
{
	private static bool cameraMovingMode = false;
	
	private static bool Prefix(ref PlayerController __instance)
	{
		if (!Main.MySettings.ToggleModeEnabled && !Main.MySettings.DisableCameraSmoothing)
		{
			return true; //execute original function
		}
		
		if (__instance._isSelected)
		{
			bool startLooking = false;
			bool stopLooking = false;
			
			if (Main.MySettings.ToggleModeEnabled && GameInput.shared.LookEnableDown)
			{
				cameraMovingMode = !cameraMovingMode;
				startLooking = cameraMovingMode;
				stopLooking = !cameraMovingMode;
			}
			else if(!Main.MySettings.ToggleModeEnabled)
			{
				var shared = GameInput.shared;
				startLooking = shared.LookEnableDown;
				stopLooking = shared.LookEnableUp;
			}

			if (startLooking)
			{
				PlayerController._mouseMovesCamera = true;
				Cursor.lockState = CursorLockMode.Locked;
			}
			else if(stopLooking)
			{
				PlayerController._mouseMovesCamera = false;
				Cursor.lockState = CursorLockMode.None;
			}
		
			var lookDelta = PlayerController._mouseMovesCamera ? GameInput.shared.LookDelta : Vector2.zero;
			float cameraMoveX = __instance.CameraXMultiplier * lookDelta.x;
			float cameraMoveY = __instance.CameraYMultiplier * lookDelta.y;
			
			if (Main.MySettings.DisableCameraSmoothing)
			{
				__instance._inputLookPitch = cameraMoveY;
				__instance._inputLookYaw = cameraMoveX;
			}
			else
			{
				float t = 1f - Mathf.Exp(-__instance.cameraInputSharpness * Time.deltaTime);
				__instance._inputLookPitch = Mathf.Lerp(__instance._inputLookPitch, cameraMoveY, t);
				__instance._inputLookYaw = Mathf.Lerp(__instance._inputLookYaw, cameraMoveX, t);
			}
		}
		else
		{
			__instance._inputLookPitch = 0.0f;
			__instance._inputLookYaw = 0.0f;
		}
		__instance.HandleCharacterInput();
		
		return false; //skip original function
	}
}
