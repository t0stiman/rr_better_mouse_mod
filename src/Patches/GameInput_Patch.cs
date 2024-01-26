using System;
using HarmonyLib;
using UI;
using UnityEngine;
using better_mouse_mod.Extensions;

namespace better_mouse_mod.Patches;

/// <summary>
/// Pause the game with the pause/break button, so you don't have to close all windows to do so.
/// </summary>
[HarmonyPatch(typeof(GameInput))]
[HarmonyPatch(nameof(GameInput.Update))]
public class GameInput_Update_Patch
{
	private static void Postfix(ref GameInput __instance)
	{
		if (!Main.MySettings.PauseWithPauseButton)
		{
			return;
		}

		if (Input.GetKeyDown(KeyCode.Pause))
		{
			Func<bool> func;
			if (GameInput._escapeHandlers.TryGetValue(GameInput.EscapeHandler.Pause, out func) && func())
			{
				return;
			}
			
			Main.Error($"{nameof(GameInput_Update_Patch)}: Failed to pause");
		}
	}
}

// ======================================================
// These following patches are for the RepeatInterval settings. 
// ======================================================

// reverser & throttle: see TrainInput_Patch

// train brake
[HarmonyPatch(typeof(GameInput))]
[HarmonyPatch(nameof(GameInput.TrainBrakeApply), MethodType.Getter)]
public class GameInput_TrainBrakeApply_Patch
{
	private static bool Prefix(ref bool __result, ref GameInput __instance)
	{
		__result = __instance._trainBrakeApplyRepeating.ActiveThisFrame(Main.MySettings.Smooth_RepeatInterval);

		return false; //skip original function
	}
}

[HarmonyPatch(typeof(GameInput))]
[HarmonyPatch(nameof(GameInput.TrainBrakeRelease), MethodType.Getter)]
public class GameInput_TrainBrakeRelease_Patch
{
	private static bool Prefix(ref bool __result, ref GameInput __instance)
	{
		__result = __instance._trainBrakeReleaseRepeating.ActiveThisFrame(Main.MySettings.Smooth_RepeatInterval);

		return false; //skip original function
	}
}

// locomotive brake
[HarmonyPatch(typeof(GameInput))]
[HarmonyPatch(nameof(GameInput.LocomotiveBrakeApply), MethodType.Getter)]
public class GameInput_LocomotiveBrakeApply_Patch
{
	private static bool Prefix(ref bool __result, ref GameInput __instance)
	{
		__result = __instance._locomotiveBrakeApplyRepeating.ActiveThisFrame(Main.MySettings.Smooth_RepeatInterval);

		return false; //skip original function
	}
}

[HarmonyPatch(typeof(GameInput))]
[HarmonyPatch(nameof(GameInput.LocomotiveBrakeRelease), MethodType.Getter)]
public class GameInput_LocomotiveBrakeRelease_Patch
{
	private static bool Prefix(ref bool __result, ref GameInput __instance)
	{
		__result = __instance._locomotiveBrakeReleaseRepeating.ActiveThisFrame(Main.MySettings.Smooth_RepeatInterval);

		return false; //skip original function
	}
}
