using better_mouse_mod.Extensions;
using HarmonyLib;
using Model;

namespace better_mouse_mod.Patches;

[HarmonyPatch(typeof(TrainController))]
[HarmonyPatch(nameof(TrainController.SelectedCar), MethodType.Setter)]
public class TrainController_Patch
{
	private static void Postfix(Car value)
	{
		if (!Main.MySettings.PlaceAvatarInChairOnSelectCar)
		{
			return;
		}
		
		if (!value.IsLocomotive)
		{
			return;
		}
		
		CameraSelector.shared.JumpToSeatWithoutCameraChange();
	}
}