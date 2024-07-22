using Game;
using HarmonyLib;
using UnityEngine;

namespace better_mouse_mod.Patches;

/// <summary>
/// Higher zoom -> click stuff further away
/// </summary>
[HarmonyPatch(typeof(SwitchStandClick))]
[HarmonyPatch(nameof(SwitchStandClick.MaxPickDistance), MethodType.Getter)]
public class SwitchStandClick_MaxPickDistance_Patch
{
	private static void Postfix(ref float __result)
	{
		if (!Main.MySettings.ZoomDependentSwitchRange)
		{
			return;
		}
		
		var firstPersonCamera = Camera.main;
		if (firstPersonCamera is null)
		{
			Main.Error($"{nameof(SwitchStandClick_MaxPickDistance_Patch)}: {nameof(firstPersonCamera)} is null");
			return;
		}
		
		var zoom = Preferences.DefaultFOV / firstPersonCamera.fieldOfView;
		if (zoom > 1)
		{
			__result *= zoom;
		}
	}
}