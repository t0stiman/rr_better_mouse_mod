using UnityEngine;

namespace better_mouse_mod;

public static class Stuff
{
	/// <summary>
	/// Same as the modulo operator except it works for negative numbers, like in C++.
	/// </summary>
	public static int CPPModulo(float a, float b)
	{
		return Mathf.RoundToInt(a - b * Mathf.Floor(a / b));
	}
}