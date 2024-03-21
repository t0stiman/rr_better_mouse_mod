using HarmonyLib;
using UI.Common;

namespace better_mouse_mod.Patches;

/// <summary>
/// Disable closing windows with the escape button, so you can pause without closing all windows
/// </summary>
[HarmonyPatch(typeof(WindowManager))]
[HarmonyPatch(nameof(WindowManager.OnEnable))]
public class WindowManager_OnEnable_Patch
{
	private static bool Prefix(ref WindowManager __instance)
	{
		if (!Main.MySettings.DisableEscapeWindowClose)
		{
			return Constants.EXECUTE_ORIGINAL;
		}
		
		__instance.CloseAllWindows();
		
		return Constants.SKIP_ORIGINAL;
	}
}