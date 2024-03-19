using UnityEngine.SceneManagement;

namespace better_mouse_mod;

/// <summary>
/// finds the CameraSelector component
/// </summary>
public static class CameraSelectorFinder
{
	private static CameraSelector cameraSelector;
	
	public static CameraSelector GetCameraSelector()
	{
		if (cameraSelector != null)
		{
			return cameraSelector;
		}
		
#pragma warning disable CS0618
		foreach (var scene in SceneManager.GetAllScenes())
#pragma warning restore CS0618
		{
			foreach (var rootObject in scene.GetRootGameObjects())
			{
				var component = rootObject.GetComponentInChildren<CameraSelector>();
				if (component != null)
				{
					cameraSelector = component;
					return component;
				}
			}
		}

		Main.Error("Couldn't find CameraSelector!");
		return null;
	}
}