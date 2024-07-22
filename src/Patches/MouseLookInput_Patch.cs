using Cameras;
using HarmonyLib;
using UI;
using UnityEngine;

namespace better_mouse_mod.Patches;

/// <summary>
/// this patch is for moving the player character while in 1st person view
/// </summary>
[HarmonyPatch(typeof(MouseLookInput))]
[HarmonyPatch(nameof(MouseLookInput.UpdateInput))]
public class MouseLookInput_UpdateInput_Patch
{
	private static bool Prefix(ref MouseLookInput __instance, bool selected)
	{
		if (!Main.MySettings.DisableCameraSmoothing)
		{
			return Constants.EXECUTE_ORIGINAL;
		}

		if (!GameInput.MovementInputEnabled || !selected)
		{
			__instance.SetMouseMovesCamera(false);
		}

		if (selected)
		{
			var shared = GameInput.shared;
			
			if (MouseLookInput.ToggleMouseLook)
			{
				if (shared.MouseLookToggle)
				{
					__instance.SetMouseMovesCamera(!__instance._mouseMovesCamera);
				}
			}
			else if (shared.SecondaryLongPressBeganThisFrame)
			{
				__instance.SetMouseMovesCamera(true);
			}
			else if (shared.SecondaryLongPressEndedThisFrame)
			{
				__instance.SetMouseMovesCamera(false);
			}

			if (!Main.MySettings.DisableCameraSmoothing)
			{
				var lookDelta = __instance._mouseMovesCamera ? shared.LookDelta : Vector2.zero;
				var cameraMoveX = MouseLookInput.CameraXMultiplier * lookDelta.x;
				var cameraMoveY = MouseLookInput.CameraYMultiplier * lookDelta.y;

				var t = 1f - Mathf.Exp(-20f * Time.deltaTime);
				__instance.Pitch = Mathf.Lerp(__instance.Pitch, cameraMoveY, t);
				__instance.Yaw = Mathf.Lerp(__instance.Yaw, cameraMoveX, t);
			}
		}
		else
		{
			__instance.Pitch = 0.0f;
			__instance.Yaw = 0.0f;
		}
		
		return Constants.SKIP_ORIGINAL;
	}
}