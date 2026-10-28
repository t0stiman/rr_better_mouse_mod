using System;
using Cameras;
using Game;
using HarmonyLib;
using Helpers;
using UI;
using UnityEngine;

namespace better_mouse_mod.Patches;

/// <summary>
/// this patch is for the 3rd person / flying camera
/// </summary>
[HarmonyPatch(typeof(StrategyCameraController))]
[HarmonyPatch(nameof(StrategyCameraController.UpdateInput))]
public class StrategyCameraController_UpdateInput_Patch
{
	// this should probably be a transpile but i haven't learned how to make those yet
	private static bool Prefix(ref StrategyCameraController __instance)
	{
		// ======= this is unchanged: ======

		__instance._distanceInput = 0.0f;
		__instance._angleXInput = 0.0f;
		__instance._angleYInput = 0.0f;

		// WASD and QE
		var movement = GameInput.Shared.GetMovement(__instance.normalSpeed, __instance.fastSpeed, __instance.fasterSpeed);
		__instance._movementInput = new Vector3(movement.x, 0.0f, movement.z);

		// ======= this is changed: ======

		// __instance is moving the camera with Q and E
		if (ModEntryPoint.MySettings!.DisableCameraSmoothing)
		{
			__instance._angleYInput += movement.y / 60f;
		}
		else
		{
			__instance._angleYInput += movement.y / 5f;
		}

		// ======= this is unchanged: ======

		var doPanning = false;
		var doLookEnable = false;
		if (!GameInput.IsMouseOverUI(out _, out _))
		{
			doPanning = GameInput.Shared.PanCameraPressed; //TODO why unused
			doLookEnable = Input.GetMouseButtonDown(Constants.RIGHT_MOUSE_BUTTON);
		}

		if (GameInput.IsMouseOverGameWindow() && Math.Abs(Input.mouseScrollDelta.y) > 1.0 / 1000.0)
		{
			__instance._distanceInput = -Input.mouseScrollDelta.y;
		}

		// ======= this is changed: ======

		if (doLookEnable)
		{
			if (ModEntryPoint.MySettings!.ThirdPersonToggleMode)
			{
				__instance._rotateStarted = !__instance._rotateStarted;

				if (__instance._rotateStarted)
				{
					Cursor.lockState = CursorLockMode.Locked;
				}
				else
				{
					Cursor.lockState = CursorLockMode.None;
				}
			}
			else
			{
				__instance._rotateStartPosition = Input.mousePosition;
				__instance._rotateStarted = true;
			}
		}

		if (__instance._rotateStarted && !ModEntryPoint.MySettings!.ThirdPersonToggleMode &&
		    !Input.GetMouseButton(Constants.RIGHT_MOUSE_BUTTON))
		{
			__instance._rotateStarted = false;
		}

		if (__instance._rotateStarted)
		{
			if (ModEntryPoint.MySettings!.DisableCameraSmoothing || ModEntryPoint.MySettings!.ThirdPersonToggleMode)
			{
				var lookInput = GameInput.Shared.LookDelta;
				//y and x intentionally reversed
				__instance._angleYInput = lookInput.x;
				__instance._angleXInput = -lookInput.y;
			}
			else
			{
				__instance._rotateCurrentPosition = Input.mousePosition;
				var vector3 = __instance._rotateStartPosition - __instance._rotateCurrentPosition;

				__instance._rotateStartPosition = __instance._rotateCurrentPosition;
				__instance._angleYInput = (float)(-(double)vector3.x * 0.85000002384185791);
				__instance._angleXInput = vector3.y * 0.5f;
			}
		}

		return Constants.SKIP_ORIGINAL;
	}

	[HarmonyPatch(typeof(StrategyCameraController))]
	[HarmonyPatch(nameof(StrategyCameraController.UpdateCameraPosition))]
	public class StrategyCameraController_UpdateCameraPosition_Patch
	{
		private static bool Prefix(ref StrategyCameraController __instance, bool immediate)
		{
			if (!ModEntryPoint.MySettings!.DisableCameraSmoothing)
			{
				return Constants.EXECUTE_ORIGINAL;
			}

			// ============== unchanged: ==============
			
			float fixedDeltaTime = Time.fixedDeltaTime;
	    __instance._targetHeight = __instance.targetHeightFree;
	    Vector3 vector3_1 = __instance.distanceToSpeed.Evaluate(__instance._distance) * fixedDeltaTime * __instance._movementInput;
	    float t = fixedDeltaTime * 5f;
	    vector3_1.y = 0.0f;
	    __instance._movementVelocity = Vector3.Lerp(__instance._movementVelocity, __instance.transform.rotation.OnlyEulerY() * vector3_1, t);
	    __instance._targetPosition += 50f * fixedDeltaTime * __instance._movementVelocity;
	    if (__instance._movementVelocity.magnitude > 1.0 / 1000.0)
	      __instance._moveToTarget = new Vector3?();
	    if (__instance._moveToTarget.HasValue)
	    {
	      __instance._moveTimer += fixedDeltaTime;
	      __instance._targetPosition = Vector3.Lerp(__instance._targetPosition, __instance._moveToTarget.Value, fixedDeltaTime * 10f);
	      if (__instance._moveTimer > 5.0)
	      {
	        __instance._targetPosition = __instance._moveToTarget.Value;
	        __instance._moveToTarget = new Vector3?();
	      }
	    }
	    Vector3 vector3_2 = __instance.ConsumePanOffset();
	    bool flag = vector3_2 != Vector3.zero;
	    if (flag)
	    {
	      __instance._targetPosition += vector3_2;
	      __instance._moveToTarget = new Vector3?();
	      if (__instance._panDragPixels > (double) __instance.panClickSlopPixels)
	        __instance.FollowCar = null;
	    }
	    float num = 0.0f;
	    if (__instance._movementVelocity.magnitude > 1.0 / 1000.0)
	    {
	      __instance._targetPosition = __instance.SnapToGround(__instance._targetPosition);
	      __instance.FollowCar = null;
	    }
	    else if (__instance.FollowCar != null)
	    {
	      (Vector3 position, Quaternion rotation) = __instance.FollowCar.GetMoverTargetPositionRotation();
	      Vector3 vector3_3 = __instance._followInertia.Update(rotation * Vector3.forward * __instance.FollowCar.velocity, fixedDeltaTime, Preferences.CameraInertia);
	      Vector3 vector3_4 = __instance._targetHeight * Vector3.up;
	      __instance._targetPosition = position + vector3_4 + vector3_3;
	      float y = (rotation * Quaternion.Inverse(__instance._followCarInitialRotation)).eulerAngles.y;
	      num = Mathf.Abs(Mathf.DeltaAngle(__instance._extraRotationY, y)) < 10.0 ? Mathf.LerpAngle(__instance._extraRotationY, y, t) : y;
	    }
	    else if (flag)
	      __instance._targetPosition = __instance.SnapToGround(__instance._targetPosition);
	    __instance._extraRotationY = num;

			// ============== changed: ==============

			__instance._distanceVelocity = Mathf.Lerp(__instance._distanceVelocity, __instance._distanceInput, t);
			// __instance._angleXVelocity = Mathf.Lerp(__instance._angleXVelocity, __instance._angleXInput, t);
			// __instance._angleYVelocity = Mathf.Lerp(__instance._angleYVelocity, __instance._angleYInput, t);
			__instance._distance += __instance._distanceVelocity * __instance.ZoomDelta * fixedDeltaTime;

			__instance._angleY += __instance._angleYInput * Preferences.MouseLookSpeed;
			__instance._angleX += __instance._angleXInput * Preferences.MouseLookSpeed;

			// ============== end of changed code ==============

			__instance._distance = Mathf.Clamp(__instance._distance, 1f, 500f);
			__instance._angleX = Mathf.Clamp(__instance._angleX, -30f, 90f);
			__instance.CalculatePositionRotation(out var position1, out var rotation1);
			__instance.MoveAboveGround(ref position1, ref rotation1);
			__instance.SetPositionRotation(position1, rotation1, immediate);

			return Constants.SKIP_ORIGINAL;
		}
	}
}

/// <summary>
/// DisableCameraSmoothing
/// </summary>
[HarmonyPatch(typeof(StrategyCameraController))]
[HarmonyPatch(nameof(StrategyCameraController.UpdatePosition))]
public class StrategyCameraController_UpdatePosition_Patch
{
	private static bool Prefix(ref StrategyCameraController __instance)
	{
		if (!ModEntryPoint.MySettings!.DisableCameraSmoothing)
		{
			return Constants.EXECUTE_ORIGINAL;
		}
		
		//TODO deze nog nodig? immediate is altijd true

		var rotation = Quaternion.Euler(__instance._angleX, __instance._angleY + __instance._extraRotationY, 0.0f);
		var position = __instance._targetPosition + rotation * (Vector3.back * __instance._distance);

		var transform = __instance.transform;
		transform.position = position;
		transform.rotation = rotation;

		return false;
	}
}