using System.Runtime.InteropServices;
using UnityEngine;

namespace MoreMountains.Tools;

[StructLayout(LayoutKind.Sequential, Size = 1)]
public struct MMFloatingTextSpawnEvent
{
	public delegate void Delegate(int channel, Vector3 spawnPosition, string value, Vector3 direction, float intensity, bool forceLifetime = false, float lifetime = 1f, bool forceColor = false, Gradient animateColorGradient = null, bool useUnscaledTime = false);

	private static event Delegate OnEvent;

	public static void Register(Delegate callback)
	{
		OnEvent += callback;
	}

	public static void Unregister(Delegate callback)
	{
		OnEvent -= callback;
	}

	public static void Trigger(int channel, Vector3 spawnPosition, string value, Vector3 direction, float intensity, bool forceLifetime = false, float lifetime = 1f, bool forceColor = false, Gradient animateColorGradient = null, bool useUnscaledTime = false)
	{
		OnEvent?.Invoke(channel, spawnPosition, value, direction, intensity, forceLifetime, lifetime, forceColor, animateColorGradient, useUnscaledTime);
	}
}
