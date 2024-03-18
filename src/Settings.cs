using UI;
using UnityEngine;
using UnityModManagerNet;

namespace better_mouse_mod
{
	public class Settings : UnityModManager.ModSettings
	{
		//Camera
		public bool ToggleModeEnabled = true;
		public bool DisableCameraSmoothing = true;
		public bool DisableLeftClickPanning = true;
		
		//Pause menu
		public bool DisableEscapeWindowClose = false;
		public bool PauseWithPauseButton = true;
		
		//Vehicle controls
		public bool ChangeThrottleNotchCount_Steam = false;
		public int ThrottleNotchCount_Steam = 8;
		private string ThrottleNotchCount_Steam_text = null;
		
		public bool ChangeThrottleNotchCount_Diesel = false;
		public int ThrottleNotchCount_Diesel = 8;
		private string ThrottleNotchCount_Diesel_text = null;

		public float Notched_RepeatInterval = 0.10f;
		private string Notched_RepeatInterval_text;
		
		public float Smooth_RepeatInterval = 0.01f; //0.05 is the default in VirtualRepeatingInput constructor
		private string Smooth_RepeatInterval_text;
		
		//others
		public bool EnableConsolePatch = false;
		public bool ZoomDependentSwitchRange = true;
		public bool PlaceAvatarInChairOnSelectCar = false;
		
		//logging stuff
		public bool LogToConsole = false;
		public bool EnableDebugLogs = false;

		public void Setup()
		{
			Notched_RepeatInterval_text = Notched_RepeatInterval.ToString("0.000");
			Smooth_RepeatInterval_text = Smooth_RepeatInterval.ToString("0.000");
		}
		
		public void Draw(UnityModManager.ModEntry modEntry)
		{
			//Camera
			GUILayout.Label("Camera: ");
			
			ToggleModeEnabled = GUILayout.Toggle(ToggleModeEnabled, "Toggle camera movement with right click instead of holding");
			DisableCameraSmoothing = GUILayout.Toggle(DisableCameraSmoothing, "Disable camera smoothing, makes the controls much more precise");
			DisableLeftClickPanning = GUILayout.Toggle(DisableLeftClickPanning, "Disable camera panning with left mouse click. This prevents accidental panning when miss-clicking");
			
			//Pause menu
			GUILayout.Space(20);
			GUILayout.Label("Pause menu: ");
			
			DisableEscapeWindowClose = GUILayout.Toggle(DisableEscapeWindowClose, "Disable closing windows with the escape button, so you can pause without closing all windows (you need to restart the game to apply this)");
			PauseWithPauseButton = GUILayout.Toggle(PauseWithPauseButton, "Pause the game with the pause/break button on your keyboard, so you don't have to close all windows to do so");
			
			//Vehicle controls
			GUILayout.Space(20);
			GUILayout.Label("Vehicle controls: ");
			
			ChangeThrottleNotchCount_Steam = GUILayout.Toggle(ChangeThrottleNotchCount_Steam, "Change the amount of throttle notches on steam locomotives");
			if (ChangeThrottleNotchCount_Steam)
			{
				UpdateThrottleNotchCount(ref ThrottleNotchCount_Steam_text, ref ThrottleNotchCount_Steam);
			}
			
			ChangeThrottleNotchCount_Diesel = GUILayout.Toggle(ChangeThrottleNotchCount_Diesel, "Change the amount of throttle notches on diesel locomotives");
			if (ChangeThrottleNotchCount_Diesel)
			{
				UpdateThrottleNotchCount(ref ThrottleNotchCount_Diesel_text, ref ThrottleNotchCount_Diesel);
			}
			
			GUILayout.Space(20);
			GUILayout.Label("How fast vehicle control inputs should be repeated when holding the button (in seconds). Lower value = faster.");
			
			GUILayout.Label("Repeat interval for notched controls (recommended: 0.1):");
			DrawFloatInput(ref Notched_RepeatInterval_text, ref Notched_RepeatInterval);
			
			GUILayout.Label("Repeat interval for smooth (non-notched) controls (recommended: 0.01):");
			DrawFloatInput(ref Smooth_RepeatInterval_text, ref Smooth_RepeatInterval);
			
			// others
			GUILayout.Space(20);
			GUILayout.Label("Other tweaks: ");
			
			EnableConsolePatch = GUILayout.Toggle(EnableConsolePatch, "Enable console patch. There is a bug in Railroader where the console would close when you try to type something. Enable this to get rid of that bug.");
			ZoomDependentSwitchRange = GUILayout.Toggle(ZoomDependentSwitchRange, "Flip switches from further away when you zoom in with your camera.");
			PlaceAvatarInChairOnSelectCar = GUILayout.Toggle(PlaceAvatarInChairOnSelectCar, "When you select a locomotive, place your avatar in an unoccupied seat in the locomotive.");
			
			// logging stuff
			GUILayout.Space(20);
			GUILayout.Label("Logging stuff: ");

			LogToConsole = GUILayout.Toggle(LogToConsole, "Log messages to the in-game console as well as Player.log");
			EnableDebugLogs = GUILayout.Toggle(EnableDebugLogs, "Enable debug messages");
		}

		private void DrawFloatInput(ref string text, ref float number)
		{
			text = GUILayout.TextField(text);
			if (float.TryParse(text, out float parsed))
			{
				number = parsed;
			}
			else
			{
				GUILayout.Label($"not a valid number");
			}
		}

		private static void UpdateThrottleNotchCount(ref string text, ref int notchCount)
		{
			if (text is null)
			{
				text = notchCount.ToString();
			}
				
			text = GUILayout.TextField(text, 3);
			
			if (int.TryParse(text, out int parsed) && parsed is >= 2 and <= 100)
			{
				notchCount = parsed;
			}
			else
			{
				GUILayout.Label($"must be a number from 2 to 100");
			}
		}

		public override void Save(UnityModManager.ModEntry modEntry)
		{
			Save(this, modEntry);
		}
	}
}