using UI;
using UnityEngine;
using UnityModManagerNet;

namespace better_mouse_mod
{
	public class Settings : UnityModManager.ModSettings
	{
		//Camera
		public bool ToggleModeEnabled = true;
		public bool DisableLeftClickPanning = true;
		// public bool DisableCameraSmoothing => ToggleModeEnabled; //todo separate setting
		
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

		public float Notched_RepeatInterval = 0.2f;
		private string Notched_RepeatInterval_text;
		
		public float Smooth_RepeatInterval = 0.05f; //0.05 is the default in VirtualRepeatingInput constructor
		private string Smooth_RepeatInterval_text;

		public void Setup()
		{
			Notched_RepeatInterval_text = Notched_RepeatInterval.ToString("0.000");
			Smooth_RepeatInterval_text = Smooth_RepeatInterval.ToString("0.000");
		}
		
		public void Draw(UnityModManager.ModEntry modEntry)
		{
			//Camera
			GUILayout.Label("Camera: ");
			
			ToggleModeEnabled = GUILayout.Toggle(ToggleModeEnabled, "Toggle camera movement instead of hold and disable camera smoothing");
			DisableLeftClickPanning = GUILayout.Toggle(DisableLeftClickPanning, "Disable camera panning with left mouse click");
			
			//Pause menu
			GUILayout.Space(20);
			GUILayout.Label("Pause menu: ");
			
			DisableEscapeWindowClose = GUILayout.Toggle(DisableEscapeWindowClose, "Disable closing windows with the escape button, so you can pause without closing all windows");
			PauseWithPauseButton = GUILayout.Toggle(PauseWithPauseButton, "Pause the game with the pause/break button on your keyboard, so you don't have to close all windows to do so");
			
			//Vehicle controls
			GUILayout.Space(20);
			GUILayout.Label("Notched controls: ");
			
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
			
			GUILayout.Label("Repeat interval for notched controls:"); //todo
			DrawFloatInput(ref Notched_RepeatInterval_text, ref Notched_RepeatInterval);
			
			GUILayout.Label("Repeat interval for smooth (non-notched) controls:");
			DrawFloatInput(ref Smooth_RepeatInterval_text, ref Smooth_RepeatInterval);
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