using better_mouse_mod.Extensions;
using Character;
using HarmonyLib;
using UI;
using UnityEngine;

namespace better_mouse_mod.Patches;

//todo
// [HarmonyPatch(typeof(PlayerController))]
// [HarmonyPatch(nameof(PlayerController.JumpToCar))]
// public class PlayerController_JumpToCar_Patch
// {
// 	private static bool Prefix(Model.Car car)
// 	{
// 		if (!Main.MySettings.SwitchCarsWithButton)
// 		{
// 			return true;
// 		}
//
// 		TrainController.Shared.SelectedCar = car;
// 		CameraSelector.shared.JumpToSeatWithoutCameraChange();
// 		return Constants.SKIP_ORIGINAL;
// 	}
// }

[HarmonyPatch(typeof(PlayerController))]
[HarmonyPatch(nameof(PlayerController.HandleCharacterInput))]
public class PlayerController_HandleCharacterInput_Patch
{
	private static bool Prefix(ref PlayerController __instance)
	{
		if (!Main.MySettings.DisableCameraSmoothing)
		{
			return Constants.EXECUTE_ORIGINAL;
		}
		
		var inputs = new PlayerCharacterInputs();
		if (__instance._isSelected)
		{
			var shared = GameInput.shared;
			var moveVector = shared.MoveVector;
			var y = moveVector.y;
			var x = moveVector.x;
			var lean = __instance.LeanInput(__instance.character.Lean);
			if (Mathf.Abs(y) > 0.10000000149011612 || Mathf.Abs(x) > 0.10000000149011612)
			{
				lean = Lean.Off;
			}

			inputs.MoveAxisForward = y;
			inputs.MoveAxisRight = x;
			inputs.CameraRotation = __instance.cameraContainer.transform.rotation;
			inputs.RotateAxisY = __instance.character.Lean == Lean.Off ? __instance._mouseLookInput.Yaw : 0.0f;
			inputs.JumpDown = shared.JumpDown;
			inputs.CrouchDown = shared.CrouchDown;
			inputs.CrouchUp = shared.CrouchUp;
			inputs.Lean = lean;
			inputs.Run = shared.ModifierRun;
		}
		
		var num = __instance._characterInputs.Lean == Lean.Off ? 0 : (inputs.Lean == Lean.Off ? 1 : 0);
		__instance._characterInputs = inputs;
		// if (num != 0)
		// {
		// 	__instance.character.motor.SetRotation(Quaternion.LookRotation(Vector3.ProjectOnPlane(__instance.cameraContainer.rotation * Vector3.forward, Vector3.up)));
		// 	__instance.cameraContainer.rotation = Quaternion.LookRotation(Vector3.ProjectOnPlane(__instance.cameraContainer.rotation * Vector3.right, Vector3.forward));
		// }
		__instance.character.SetInputs(ref inputs);

		return Constants.SKIP_ORIGINAL;
	}
}