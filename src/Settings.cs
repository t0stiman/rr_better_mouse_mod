using UnityEngine;
using UnityModManagerNet;

namespace better_mouse_mod
{
	public class Settings : UnityModManager.ModSettings
	{
		public bool ToggleModeEnabled = true;
		public bool DisableLeftClickPanning = true;
		// public bool DisableCameraSmoothing => ToggleModeEnabled; //todo separate setting
		
		public void Draw(UnityModManager.ModEntry modEntry)
		{
			ToggleModeEnabled = GUILayout.Toggle(ToggleModeEnabled, "Toggle camera movement instead of hold and disable camera smoothing");
			DisableLeftClickPanning = GUILayout.Toggle(DisableLeftClickPanning, "Disable camera panning with left mouse click");
		}

		public override void Save(UnityModManager.ModEntry modEntry)
		{
			Save(this, modEntry);
		}
	}
}