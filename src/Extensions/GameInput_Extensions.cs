using UI;

namespace better_mouse_mod.Extensions;

public static class GameInput_Extensions
{
	public static bool GetReverserForward(this GameInput instance, float repeatInterval)
	{
		return instance._reverserForwardRepeating.ActiveThisFrame(repeatInterval);
	}
	
	public static bool GetReverserBack(this GameInput instance, float repeatInterval)
	{
		return instance._reverserBackRepeating.ActiveThisFrame(repeatInterval);
	}
	
	public static bool GetThrottleUp(this GameInput instance, float repeatInterval)
	{
		return instance._throttleUpRepeating.ActiveThisFrame(repeatInterval);
	}
	
	public static bool GetThrottleDown(this GameInput instance, float repeatInterval)
	{
		return instance._throttleDownRepeating.ActiveThisFrame(repeatInterval);
	}
}