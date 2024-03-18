using better_mouse_mod.Extensions;
using HarmonyLib;
using Model.Definition;
using UI.CarInspector;
using UnityEngine.SceneManagement;

namespace better_mouse_mod.Patches;

[HarmonyPatch(typeof(CarInspector))]
[HarmonyPatch(nameof(CarInspector.SelectConsist))]
public class CarInspector_Patch
{
	private static CameraSelector cameraSelector;
	
	private static void Postfix(ref CarInspector __instance)
	{
		if (!Main.MySettings.PlaceAvatarInChairOnSelectCar)
		{
			return;
		}
		
		// selected a locomotive?
		if (__instance._car.Definition.Archetype != CarArchetype.LocomotiveDiesel && 
		    __instance._car.Definition.Archetype != CarArchetype.LocomotiveSteam)
		{
			return;
		}

		if (cameraSelector == null)
		{
			cameraSelector = GetCameraSelector();
		}

		cameraSelector.JumpToSeatWithoutCameraChange();
	}

	private static CameraSelector GetCameraSelector()
	{
#pragma warning disable CS0618
		foreach (var scene in SceneManager.GetAllScenes())
#pragma warning restore CS0618
		{
			foreach (var rootObject in scene.GetRootGameObjects())
			{
				var component = rootObject.GetComponentInChildren<CameraSelector>();
				if (component != null)
				{
					return component;
				}
			}
		}

		return null;
	}
}