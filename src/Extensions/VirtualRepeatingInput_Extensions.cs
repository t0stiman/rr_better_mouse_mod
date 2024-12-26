using System;
using UI;
using UnityEngine;

namespace better_mouse_mod.Extensions;

public static class VirtualRepeatingInput_Extensions
{
	/// <summary>
	/// The same as VirtualRepeatingInput.ActiveThisFrame except the repeat interval is an argument instead of a field. 
	/// </summary>
	public static bool ActiveThisFrame(this VirtualRepeatingInput instance, float repeatInterval)
	{
		if (!instance._pressed) { return false; }

		float unscaledTime = Time.unscaledTime;
		if (Math.Abs(instance._downFrameTime - unscaledTime) < 1.0 / 1000.0)
		{
			return true;
		}

		if (instance._downFrameTime + (double)repeatInterval > unscaledTime)
		{
			return false;
		}

		while (instance._downFrameTime + repeatInterval <= (double)unscaledTime)
		{
			instance._downFrameTime += repeatInterval;
		}

		return true;
	}
}