using Character;
using HarmonyLib;
using Model;
using UnityEngine;
using CharacterController = Character.CharacterController;

namespace better_mouse_mod.Patches;

/// <summary>
/// Make the characters rotation match the 1st person camera
/// </summary>
[HarmonyPatch(typeof(CharacterController))]
[HarmonyPatch(nameof(CharacterController.GetMotionSnapshot))]
public static class CharacterController_GetMotionSnapshot_Patch
{
	//todo test ladders
	private static bool Prefix(ref CharacterController __instance, ref MotionSnapshot __result)
	{
		if (!Main.MySettings.DisableCameraSmoothing)
		{
			return Constants.EXECUTE_ORIGINAL;
		}

		//this line is different:
		var cameraRotation = __instance.cameraContainer.rotation;

		var bodyRotation = cameraRotation;
		if (__instance._ladder != null)
		{
			bodyRotation = Quaternion.Euler(0.0f, 180f, 0.0f) * __instance._ladder.transform.rotation;
		}

		if (__instance._seat != null)
		{
			bodyRotation = __instance._seat.transform.rotation;
		}

		__result = new MotionSnapshot(__instance.motor.TransientPosition, bodyRotation, cameraRotation,
			__instance.motor.Velocity);

		return Constants.SKIP_ORIGINAL;
	}
}

/// <summary>
/// IDFK why this patch is necessary but without it leaning doesn't work
/// </summary>
[HarmonyPatch(typeof(CharacterController))]
[HarmonyPatch(nameof(CharacterController.AfterCharacterUpdate))]
public static class CharacterController_AfterCharacterUpdate_Patch
{
	private static Lean previousLean = (Lean)99;
	private static Vector3 localGoal = Vector3.zero;


	private static bool Prefix(ref CharacterController __instance, float deltaTime)
	{
		if (!Main.MySettings.DisableCameraSmoothing)
		{
			return Constants.EXECUTE_ORIGINAL;
		}

		// =============================================

		switch (__instance.CurrentCharacterState)
		{
			case CharacterState.Default:
				if (__instance._jumpRequested &&
				    __instance._timeSinceJumpRequested > (double)__instance.jumpPreGroundingGraceTime)
				{
					__instance._jumpRequested = false;
				}

				if ((__instance.allowJumpingWhenSliding
					    ? (__instance.motor.GroundingStatus.FoundAnyGround ? 1 : 0)
					    : (__instance.motor.GroundingStatus.IsStableOnGround ? 1 : 0)) != 0)
				{
					if (!__instance._jumpedThisFrame)
					{
						__instance._jumpConsumed = false;
					}

					__instance._timeSinceLastAbleToJump = 0.0f;
				}
				else
				{
					__instance._timeSinceLastAbleToJump += deltaTime;
				}

				if (__instance._isCrouching && !__instance._shouldBeCrouching)
				{
					__instance.SetCrouched(false);
					if (__instance.motor.CharacterOverlap(__instance.motor.TransientPosition, __instance.motor.TransientRotation,
						    __instance._probedColliders, __instance.motor.CollidableLayers, QueryTriggerInteraction.Ignore) > 0)
					{
						__instance.SetCrouched(true);
						break;
					}

					__instance._isCrouching = false;
				}

				break;
			case CharacterState.Seated:
				var flag = __instance._anchoringTimer >= 0.15000000596046448;
				switch (__instance._attachState)
				{
					case CharacterController.AttachState.Anchoring:
						if (flag)
						{
							__instance.motor.SetMovementCollisionsSolvingActivation(true);
							__instance._attachState = CharacterController.AttachState.Stable;
							break;
						}

						__instance._anchoringTimer += deltaTime;
						__instance.SetCameraHeightParameter(__instance.AnchoringParameter);
						break;
					case CharacterController.AttachState.Deanchoring:
						if (flag)
						{
							__instance.TransitionToState(CharacterState.Default);
							break;
						}

						__instance._anchoringTimer += deltaTime;
						__instance.SetCameraHeightParameter(1f - __instance.AnchoringParameter);
						break;
				}

				__instance._seatStickyRemaining -= deltaTime;
				break;
			case CharacterState.Ladder:
				__instance._ladderDuration += deltaTime;
				break;
		}

		// =============================================

		var camera = __instance.cameraContainer;

		if (previousLean != __instance.Lean)
		{
			var cameraOffset = 0.0f;
			switch (__instance.Lean)
			{
				case Lean.Left:
					cameraOffset = -0.5f;
					break;
				case Lean.Right:
					cameraOffset = 0.5f;
					break;
			}

			var cameraController = CameraSelector.shared.character.cameraController;
			localGoal = Quaternion.Euler(0f, Stuff.CPPModulo(cameraController._targetYaw, 360), 0f) *
			            new Vector3(cameraOffset, 0, 0);

			previousLean = __instance.Lean;
		}

		var t = deltaTime * 10f;

		camera.localPosition = new Vector3(
			Mathf.Lerp(camera.localPosition.x, localGoal.x, t),
			Mathf.Lerp(__instance.eyeHeightStanding, __instance.eyeHeightSeated, __instance._seatedParameter),
			Mathf.Lerp(camera.localPosition.z, localGoal.z, t)
		);

		return Constants.SKIP_ORIGINAL;
	}
}

/// <summary>
/// Make sure ladder exit bump is in the right direction
/// </summary>
[HarmonyPatch(typeof(CharacterController))]
[HarmonyPatch(nameof(CharacterController.SetInputs))]
public static class CharacterController_SetInputs_Patch
{
	private static bool Prefix(ref CharacterController __instance, ref PlayerCharacterInputs inputs)
	{
		if (!Main.MySettings.DisableCameraSmoothing)
		{
			return Constants.EXECUTE_ORIGINAL;
		}

		// ========================== unchanged code: ==========================

		var vector3_1 = Vector3.ClampMagnitude(new Vector3(inputs.MoveAxisRight, 0.0f, inputs.MoveAxisForward), 1f);
		var cameraRotation = inputs.CameraRotation;
		var normalized = Vector3.ProjectOnPlane(cameraRotation * Vector3.forward, __instance.motor.CharacterUp).normalized;
		if (normalized.sqrMagnitude == 0.0)
		{
			normalized = Vector3.ProjectOnPlane(cameraRotation * Vector3.up, __instance.motor.CharacterUp).normalized;
		}

		__instance._moveInputVector = Quaternion.LookRotation(normalized, __instance.motor.CharacterUp) * vector3_1;
		__instance._lookInputRotation = Quaternion.Euler(0.0f, inputs.RotateAxisY, 0.0f);
		if (inputs.JumpDown)
		{
			__instance._jumpedFromLadder = __instance.CurrentCharacterState == CharacterState.Ladder;
			__instance.TransitionToState(CharacterState.Default);
			__instance._timeSinceJumpRequested = 0.0f;
			__instance._jumpRequested = true;
			if (__instance._jumpedFromLadder)
			{
				__instance._jumpConsumed = false;
				__instance._timeSinceLastAbleToJump = 0.0f;
			}
		}

		__instance.Lean = inputs.Lean;
		__instance._inputRun = inputs.Run;
		switch (__instance.CurrentCharacterState)
		{
			case CharacterState.Default:
				if (__instance.CheckForLadderOrSeat())
				{
					break;
				}

				if (inputs.CrouchDown) //TODO can we get a working crouch feature?
				{
					__instance._shouldBeCrouching = true;
					if (__instance._isCrouching)
					{
						break;
					}

					__instance._isCrouching = true;
					__instance.SetCrouched(true);
					break;
				}

				if (!inputs.CrouchUp)
				{
					break;
				}

				__instance._shouldBeCrouching = false;
				break;
			case CharacterState.Seated:
				if (__instance._attachState != CharacterController.AttachState.Stable ||
				    vector3_1.sqrMagnitude <= 1.0 / 1000.0 || __instance._seatStickyRemaining > 0.0)
				{
					break;
				}

				__instance._attachState = CharacterController.AttachState.Deanchoring;
				__instance._anchoringTimer = 0.0f;
				break;
			case CharacterState.Ladder:
				if (__instance._moveInputVector.sqrMagnitude < 0.10000000149011612)
				{
					break;
				}

				if (__instance.IsMovingInto(__instance._ladder.CapsuleCollider, __instance._moveInputVector, true))
				{
					var x = (double)__instance.cameraContainer.transform.localRotation.eulerAngles.x;
					var vector3_2 = __instance._ladder.transform.up * (Time.fixedDeltaTime *
					                                                   (Mathf.Lerp(0.0f,
						                                                    __instance._inputRun
							                                                    ? __instance.ladderSpeedFast
							                                                    : __instance.ladderSpeedNormal,
						                                                    Mathf.InverseLerp(0.0f, 1f,
							                                                    __instance._ladderDuration)) *
					                                                    __instance._ladder.SpeedMultiplierForPosition(__instance
						                                                    ._ladderLocalPosition)));
					if (Mathf.Abs(Mathf.DeltaAngle((float)x, 270f)) < 140.0)
					{
						__instance._ladderLocalPosition += vector3_2;
						if (__instance._ladder.CheckPositionValid(__instance._ladderLocalPosition, true))
						{
							break;
						}

						__instance.TransitionToState(CharacterState.Default);

						// ========================== changed code: ==========================

						__instance.AddVelocity(__instance.cameraContainer.rotation * Config.Shared.ladderExitBump);

						// ========================== unchanged code: ==========================
						break;
					}

					__instance._ladderLocalPosition -= vector3_2;
					if (__instance._ladder.CheckPositionValid(__instance._ladderLocalPosition, false))
					{
						break;
					}

					__instance.TransitionToState(CharacterState.Default);
					break;
				}

				if (__instance.LadderStickyRemaining > 0.0)
				{
					break;
				}

				__instance.TransitionToState(CharacterState.Default);
				break;
		}

		return Constants.SKIP_ORIGINAL;
	}
}