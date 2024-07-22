using HarmonyLib;

namespace better_mouse_mod.Patches;

[HarmonyPatch(typeof(PauseMenu))]
[HarmonyPatch(nameof(PauseMenu.OnEnable))]
public static class PauseMenu_OnEnable_Patch
{
	private static PauseMenu thePauseMenu;

	private static bool Prefix(PauseMenu __instance)
	{
		thePauseMenu = __instance;
		return Constants.SKIP_ORIGINAL;
	}

	public static void TogglePause()
	{
		thePauseMenu.SetPaused(!PauseMenu._paused);
	}
}