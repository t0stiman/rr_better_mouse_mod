using HarmonyLib;
using UI.Console;

namespace better_mouse_mod.Patches;

[HarmonyPatch(typeof(ExpandedConsole))]
[HarmonyPatch(nameof(ExpandedConsole.DismissDebounced))]
public class ExpandedConsole_DismissDebounced_Patch
{
	private static bool Prefix()
	{
		if (Main.MySettings.EnableConsolePatch)
		{
			return false; //skip original function
		}

		return true;
	}
}