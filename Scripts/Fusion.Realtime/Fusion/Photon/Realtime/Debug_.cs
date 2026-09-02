using System;
using System.Diagnostics;
using Fusion.Photon.Realtime.Async;

namespace Fusion.Photon.Realtime;

internal static class Debug_
{
	[Conditional("DEBUG")]
	public static void Log(string msg)
	{
		Fusion.Photon.Realtime.Async.Log.Info(msg);
	}

	[Conditional("DEBUG")]
	public static void LogWarning(string msg)
	{
		Fusion.Photon.Realtime.Async.Log.Warn(msg);
	}

	[Conditional("DEBUG")]
	public static void LogError(string msg)
	{
		Fusion.Photon.Realtime.Async.Log.Error(msg);
	}

	[Conditional("DEBUG")]
	public static void LogException(Exception ex)
	{
		Fusion.Photon.Realtime.Async.Log.Error(ex);
	}
}
