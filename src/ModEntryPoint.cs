using System;
using HarmonyLib;
using Railroader.Modding;

namespace better_mouse_mod;

public sealed class ModEntryPoint : IModEntryPoint
{
	private Harmony? harmony;
	private static IModContext? context; 
	public static Settings? MySettings { get; private set; }

	//================================================================

	public void OnModLoad(IModContext context_)
	{
		context = context_;
		
		MySettings = new Settings();
		MySettings.Setup();
		
		try
		{
			harmony = new Harmony(context_.ModIdentifier);
			harmony.PatchAll();
		}
		catch (Exception ex)
		{
			context_.Log.Error($"Failed to load {context_.ModIdentifier}: {ex}");
			harmony?.UnpatchSelf();
			throw;
		}

		context.Log.Info($"Loaded from {context.ModDirectory}"); 
	}

	public void OnModUnload()
	{
		harmony?.UnpatchSelf();
		context?.Log.Info("Unloaded mod");
	}

	// Logger Commands
	public static void Debug(string message)
	{
		if (!MySettings!.EnableDebugLogs)
		{
			return;
		}

		if (MySettings.LogToConsole)
		{
			Console.Log($"{context!.ModIdentifier}[DEBUG]{message}");
		}

		context!.Log.Debug(message);
	}

	public static void Info(string message)
	{
		if (MySettings!.LogToConsole)
		{
			Console.Log($"{context!.ModIdentifier}[INFO]{message}");
		}

		context!.Log.Info(message);
	}

	public static void Warning(string message)
	{
		if (MySettings!.LogToConsole)
		{
			Console.Log($"{context!.ModIdentifier}[WARNING]{message}");
		}

		context!.Log.Warning(message);
	}

	public static void Error(string message)
	{
		if (MySettings!.LogToConsole)
		{
			Console.Log($"{context!.ModIdentifier}[ERROR]{message}");
		}

		context!.Log.Error(message);
	}
}