using System;
using better_mouse_mod.Extensions;
using Game.Messages;
using Game.State;
using HarmonyLib;
using Model.Definition;
using RollingStock.Controls;
using UI;
using UI.Common;
using UnityEngine;

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

		return Constants.SKIP_ORIGINAL;
	}

	//i should really learn to transpile
	private static void Yes(ref TrainInput __instance)
	{
		if (!__instance.TryGetLocomotiveControlAdapter(out var loco, out var adapter))
		{
			return;
		}

		var gameInput = GameInput.shared;
		var isControlDown = GameInput.IsControlDown;

		//todo verander de grootte van een stap ipv verkleinen van de herhaaltijd?

		var reverserDelta = 0.0f;
		var throttleDelta = 0.0f;
		var num5 = 0.0f;
		var num6 = 0.0f;
		
		var reverserStepSize = loco.Archetype == CarArchetype.LocomotiveDiesel | isControlDown ? 1f : 0.1f;
		var reverserRepeatInterval = Main.MySettings.Notched_RepeatInterval;
		
		if (gameInput.GetReverserBack(reverserRepeatInterval))
		{
			reverserDelta = -reverserStepSize;
		}
		else if (gameInput.GetReverserForward(reverserRepeatInterval))
		{
			reverserDelta = reverserStepSize;
		}

		var throttleStepSize = GameInput.IsShiftDown ? 0.05f : 0.01f;
		var throttleNotches = adapter.ThrottleNotches;
		if (throttleNotches > 0) //notched
		{
			var throttleChangeo = 0;
			if (gameInput.GetThrottleDown(Main.MySettings.Notched_RepeatInterval))
			{
				throttleChangeo = -1;
			}
			else if (gameInput.GetThrottleUp(Main.MySettings.Notched_RepeatInterval))
			{
				throttleChangeo = 1;
			}

			if (throttleChangeo > 0 && adapter.AbstractThrottle < 1.0)
			{
				__instance.ChangeValue(PropertyChange.Control.Throttle,
					Mathf.Clamp01(adapter.AbstractThrottle + 1f / throttleNotches));
			}

			if (throttleChangeo < 0 && adapter.AbstractThrottle > 0.0)
			{
				__instance.ChangeValue(PropertyChange.Control.Throttle,
					Mathf.Clamp01(adapter.AbstractThrottle - 1f / throttleNotches));
			}
		}

		else if (gameInput.GetThrottleDown(Main.MySettings.Smooth_RepeatInterval))
		{
			throttleDelta = -throttleStepSize;
		}
		else if (gameInput.GetThrottleUp(Main.MySettings.Smooth_RepeatInterval))
		{
			throttleDelta = throttleStepSize;
		}

		//  ============ end changed ============

		// train brakes are handled in GameInput_Patch.cs

		if (gameInput.TrainBrakeRelease)
		{
			num5 = (float)(-(double)throttleStepSize * 2.0);
		}
		else if (gameInput.TrainBrakeApply)
		{
			num5 = throttleStepSize;
		}

		if (gameInput.LocomotiveBrakeRelease)
		{
			num6 = (float)(-(double)throttleStepSize * 2.0);
		}
		else if (gameInput.LocomotiveBrakeApply)
		{
			num6 = throttleStepSize;
		}

		if (reverserDelta != 0.0)
		{
			__instance.ChangeValue(PropertyChange.Control.Reverser,
				Mathf.Clamp(adapter.AbstractReverser + reverserDelta, -1f, 1f));
		}

		if (__instance._locomotiveBrakeDelta < 0.0 && adapter.LocomotiveBrakeSetting < 0.0 && num6 == 0.0)
		{
			__instance.ChangeValue(PropertyChange.Control.LocomotiveBrake, 0.0f);
		}

		__instance._locomotiveBrakeDelta = num6;
		__instance._trainBrakeDelta = num5;
		__instance._throttleDelta = throttleDelta;

		if (gameInput.Bell)
			StateManager.ApplyLocal(new PropertyChange(loco.id, PropertyChange.Control.Bell, !loco.locomotiveControl.Bell));
		if (gameInput.CylinderCock)
			loco.ControlProperties[PropertyChange.Control.CylinderCock] =
				!loco.ControlProperties[PropertyChange.Control.CylinderCock];
		var num9 = gameInput.InputHorn;
		if (gameInput.HornExpressionEnabledThisFrame)
			__instance._hornDownMousePosition = gameInput.HornExpressionValue;
		if (gameInput.HornExpressionEnabled)
		{
			__instance._hornDownMousePosition += gameInput.HornExpressionValue;
			num9 = Mathf.Clamp01((float)(-(double)__instance._hornDownMousePosition / 200.0));
		}

		if (Math.Abs(num9 - __instance._hornWas) > 0.0099999997764825821)
		{
			StateManager.ApplyLocal(new PropertyChange(loco.id, PropertyChange.Control.Horn, num9));
			__instance._hornWas = num9;
		}

		var inputHeadlight = gameInput.InputHeadlight;
		if (inputHeadlight == 0)
			return;
		Toast.Present(
			"Headlight: " +
			HeadlightToggleLogic.TextForState(
				HeadlightToggleLogic.SetHeadlightStateOffset(loco.KeyValueObject, inputHeadlight)), ToastPosition.Bottom);
	}
}