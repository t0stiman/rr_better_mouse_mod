using UnityEngine;
using UnityModManagerNet;

namespace better_mouse_mod
{
	public class Settings : UnityModManager.ModSettings
	{
		public bool ToggleModeEnabled = true;
		public bool DisableLeftClickPanning = true;
		// public bool DisableCameraSmoothing => ToggleModeEnabled; //todo separate setting
		public bool DisableEscapeWindowClose = false;
		public bool PauseWithPauseButton = true; 
		
		public void Draw(UnityModManager.ModEntry modEntry)
		{
			ToggleModeEnabled = GUILayout.Toggle(ToggleModeEnabled, "Toggle camera movement instead of hold and disable camera smoothing");
			DisableLeftClickPanning = GUILayout.Toggle(DisableLeftClickPanning, "Disable camera panning with left mouse click");
			DisableEscapeWindowClose = GUILayout.Toggle(DisableEscapeWindowClose, "Disable closing windows with the escape button, so you can pause without closing all windows");
			PauseWithPauseButton = GUILayout.Toggle(PauseWithPauseButton, "Pause the game with the pause/break button on your keyboard, so you don't have to close all windows to do so");
		}

		public override void Save(UnityModManager.ModEntry modEntry)
		{
			Save(this, modEntry);
		}
	}
}