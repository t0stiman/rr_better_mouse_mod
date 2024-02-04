using System;
using better_mouse_mod.Extensions;
using Game.Messages;
using Game.State;
using HarmonyLib;
using Model;
using RollingStock;
using RollingStock.Controls;
using UI;
using UnityEngine;
using Model.Definition;

namespace better_mouse_mod.Patches;

// ======================================================
// This patch is for the RepeatInterval settings. 
// ======================================================
[HarmonyPatch(typeof(TrainInput))]
[HarmonyPatch(nameof(TrainInput.Update))]
public class TrainInput_Update_Patch
{
	private static bool Prefix(ref TrainInput __instance)
	{
		Yes(ref __instance);

		return false; //skip original function
	}

	//i should really learn to transpile
	private static void Yes(ref TrainInput __instance)
	{
		BaseLocomotive loco;
		LocomotiveControlAdapter adapter;

		if (!__instance.TryGetLocomotiveControlAdapter(out loco, out adapter))
		{
			return;
		}

		var shared = GameInput.shared;
		int num1 = GameInput.IsShiftDown ? 1 : 0;
		bool isControlDown = GameInput.IsControlDown;
		float num2 = num1 != 0 ? 0.05f : 0.01f;
		float reverserDelta = 0.0f;
		float num4 = 0.0f;
		float num5 = 0.0f;
		float num6 = 0.0f;
		
		//  ============ start changed ============

		bool useBigStep = loco.Archetype == CarArchetype.LocomotiveDiesel | isControlDown;
		float stepSize = useBigStep ? 1f : 0.1f;

		var reverserRepeatInterval = Main.MySettings.Notched_RepeatInterval;
		
		if (shared.GetReverserBack(reverserRepeatInterval))
		{
			reverserDelta = -stepSize;
		}
		else if (shared.GetReverserForward(reverserRepeatInterval))
		{
			reverserDelta = stepSize;
		}
		
		int throttleNotches = adapter.ThrottleNotches;
		if (throttleNotches > 0) //notched
		{
			int num8 = 0;
			if (shared.GetThrottleDown(Main.MySettings.Notched_RepeatInterval))
			{
				num8 = -1;
			}
			else if (shared.GetThrottleUp(Main.MySettings.Notched_RepeatInterval))
			{
				num8 = 1;
			}

			if (num8 > 0 && adapter.AbstractThrottle < 1.0)
			{
				__instance.ChangeValue(PropertyChange.Control.Throttle,
					Mathf.Clamp01(adapter.AbstractThrottle + 1f / throttleNotches));
			}

			if (num8 < 0 && adapter.AbstractThrottle > 0.0)
			{
				__instance.ChangeValue(PropertyChange.Control.Throttle,
					Mathf.Clamp01(adapter.AbstractThrottle - 1f / throttleNotches));
			}
		}
		
		else if (shared.GetThrottleDown(Main.MySettings.Smooth_RepeatInterval))
		{
			num4 = -num2;
		}
		else if (shared.GetThrottleUp(Main.MySettings.Smooth_RepeatInterval))
		{
			num4 = num2;
		}

		//  ============ end changed ============
		
		// train brakes are handled in GameInput_Patch.cs
		
		if (shared.TrainBrakeRelease)
			num5 = (float) (-(double) num2 * 2.0);
		else if (shared.TrainBrakeApply)
			num5 = num2;
		if (shared.LocomotiveBrakeRelease)
			num6 = (float) (-(double) num2 * 2.0);
		else if (shared.LocomotiveBrakeApply)
			num6 = num2;
		if ((double) reverserDelta != 0.0)
			__instance.ChangeValue(PropertyChange.Control.Reverser, Mathf.Clamp(adapter.AbstractReverser + reverserDelta, -1f, 1f));
		if ((double) __instance._locomotiveBrakeDelta < 0.0 && (double) adapter.LocomotiveBrakeSetting < 0.0 && (double) num6 == 0.0)
			__instance.ChangeValue(PropertyChange.Control.LocomotiveBrake, 0.0f);
		__instance._locomotiveBrakeDelta = num6;
		__instance._trainBrakeDelta = num5;
		__instance._throttleDelta = num4;
		if (shared.Bell)
			StateManager.ApplyLocal((IGameMessage) new PropertyChange(loco.id, PropertyChange.Control.Bell, !loco.locomotiveControl.Bell));
		if (shared.CylinderCock)
		{
			bool boolValue = loco.KeyValueObject[PropertyChange.KeyForControl(PropertyChange.Control.CylinderCock)].BoolValue;
			StateManager.ApplyLocal((IGameMessage) new PropertyChange(loco.id, PropertyChange.Control.CylinderCock, !boolValue));
		}
		float num9 = shared.InputHorn;
		if (shared.HornExpressionEnabledThisFrame)
			__instance._hornDownMousePosition = shared.HornExpressionValue;
		if (shared.HornExpressionEnabled)
		{
			__instance._hornDownMousePosition += shared.HornExpressionValue;
			num9 = Mathf.Clamp01((float) (-(double) __instance._hornDownMousePosition / 200.0));
		}
		if ((double) Math.Abs(num9 - __instance._hornWas) > 0.0099999997764825821)
		{
			StateManager.ApplyLocal((IGameMessage) new PropertyChange(loco.id, PropertyChange.Control.Horn, num9));
			__instance._hornWas = num9;
		}
		int inputHeadlight = shared.InputHeadlight;
		if (inputHeadlight == 0)
			return;
		Console.Log("Headlight: " + HeadlightToggleLogic.TextForState(HeadlightToggleLogic.SetHeadlightStateOffset(loco.KeyValueObject, inputHeadlight)));
	}
}