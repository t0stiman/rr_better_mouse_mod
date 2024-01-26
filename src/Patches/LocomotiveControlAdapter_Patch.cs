﻿using HarmonyLib;
using RollingStock;
using RollingStock.Steam;
using UnityEngine;

namespace better_mouse_mod.Patches;

// these patches are for changing the amount of notches on locomotive controls

// Steam

[HarmonyPatch(typeof(SteamLocomotiveControl))]
[HarmonyPatch(nameof(SteamLocomotiveControl.ThrottleNotches), MethodType.Getter)]
public class SteamLocomotiveControl_ThrottleNotches_Patch
{
	private static bool Prefix(ref int __result)
	{
		if (!Main.MySettings.ChangeThrottleNotchCount_Steam)
		{
			return true; //execute original function
		}
		
		__result = Main.MySettings.ThrottleNotchCount_Steam;
		return false; //skip original function
	}
}

// Diesel

[HarmonyPatch(typeof(DieselLocomotiveControl))]
[HarmonyPatch(nameof(DieselLocomotiveControl.ThrottleNotches), MethodType.Getter)]
public class DieselLocomotiveControl_ThrottleNotches_Patch
{
	private static bool Prefix(ref int __result)
	{
		if (!Main.MySettings.ChangeThrottleNotchCount_Diesel)
		{
			return true; //execute original function
		}
		
		__result = Main.MySettings.ThrottleNotchCount_Diesel;
		return false; //skip original function
	}
}

[HarmonyPatch(typeof(DieselLocomotiveControl))]
[HarmonyPatch(nameof(DieselLocomotiveControl.AbstractThrottle), MethodType.Getter)]
public class DieselLocomotiveControl_AbstractThrottle_Getter_Patch
{
	private static bool Prefix(ref float __result, DieselLocomotiveControl __instance)
	{
		if (!Main.MySettings.ChangeThrottleNotchCount_Diesel)
		{
			return true; //execute original function
		}
		
		__result = (float) __instance.primeMover.notch / __instance.ThrottleNotches;
		return false; //skip original function
	}
}

[HarmonyPatch(typeof(DieselLocomotiveControl))]
[HarmonyPatch(nameof(DieselLocomotiveControl.AbstractThrottle), MethodType.Setter)]
public class DieselLocomotiveControl_AbstractThrottle_Setter_Patch
{
	private static bool Prefix(DieselLocomotiveControl __instance, float value)
	{
		if (!Main.MySettings.ChangeThrottleNotchCount_Diesel)
		{
			return true; //execute original function
		}
		
		__instance.primeMover.notch = Mathf.RoundToInt(value * __instance.ThrottleNotches);
		if (__instance.audio != null && __instance.audio.primeMover != null)
		{
			__instance.audio.primeMover.Notch = __instance.primeMover.notch;
		}
		
		return false; //skip original function
	}
}
