﻿using HarmonyLib;
using UI;
using UnityEngine;
using better_mouse_mod.Extensions;

namespace better_mouse_mod.Patches;

[HarmonyPatch(typeof(GameInput))]
[HarmonyPatch(nameof(GameInput.Update))]
public class GameInput_Update_Patch
{
	private static void Postfix()
	{
		// TODO
		// SwitchButtonStuff();
		PauseStuff();
	}

	/// <summary>
	/// Cycle through your locomotives with the page up / page down keys
	/// </summary>
	// private static void SwitchButtonStuff()
	// {
	// 	if (!Main.MySettings.SwitchCarsWithButton)
	// 	{
	// 		return;
	// 	}
	//
	// 	var delta = Convert.ToInt32(Input.GetKeyDown(KeyCode.PageUp)) - 
	// 	               Convert.ToInt32(Input.GetKeyDown(KeyCode.PageDown));
	// 	if (delta == 0)
	// 	{
	// 		return;
	// 	}
	// 	
	// 	var locomotives = TrainController.Shared.Cars.Where(car => car.IsLocomotive).ToList();
	// 	if (locomotives.Count == 0)
	// 	{
	// 		return;
	// 	}
	// 	
	// 	var selectedLocomotive = TrainController.Shared.SelectedLocomotive;
	//
	// 	int newSelectionIndex;
	// 	if (selectedLocomotive == null) // no locomotive selected
	// 	{
	// 		newSelectionIndex = 0;
	// 	}
	// 	else
	// 	{
	// 		var selectedIndex = locomotives.IndexOf(selectedLocomotive);
	// 		if (selectedIndex == -1)
	// 		{
	// 			Main.Error("Can't find selected locomotive in list");
	// 		}
	// 		
	// 		newSelectionIndex = Stuff.CPPModulo(selectedIndex + delta, locomotives.Count);
	// 	}
	// 	
	// 	TrainController.Shared.SelectedCar = locomotives[newSelectionIndex];
	// 	CameraSelector.shared.JumpToCar(TrainController.Shared.SelectedCar);
	// }

	/// <summary>
	/// Custom pause key
	/// </summary>
	private static void PauseStuff()
	{
		if (Input.GetKeyDown(Main.MySettings.PauseKeyCode))
		{
			PauseMenu_OnEnable_Patch.TogglePause();
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

		return Constants.SKIP_ORIGINAL;
	}
}

[HarmonyPatch(typeof(GameInput))]
[HarmonyPatch(nameof(GameInput.TrainBrakeRelease), MethodType.Getter)]
public class GameInput_TrainBrakeRelease_Patch
{
	private static bool Prefix(ref bool __result, ref GameInput __instance)
	{
		__result = __instance._trainBrakeReleaseRepeating.ActiveThisFrame(Main.MySettings.Smooth_RepeatInterval);

		return Constants.SKIP_ORIGINAL;
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

		return Constants.SKIP_ORIGINAL;
	}
}

[HarmonyPatch(typeof(GameInput))]
[HarmonyPatch(nameof(GameInput.LocomotiveBrakeRelease), MethodType.Getter)]
public class GameInput_LocomotiveBrakeRelease_Patch
{
	private static bool Prefix(ref bool __result, ref GameInput __instance)
	{
		__result = __instance._locomotiveBrakeReleaseRepeating.ActiveThisFrame(Main.MySettings.Smooth_RepeatInterval);

		return Constants.SKIP_ORIGINAL;
	}
}