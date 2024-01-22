using System;
using HarmonyLib;
using UI;
using UnityEngine;

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