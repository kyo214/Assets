using System.Runtime.InteropServices;
using UnityEngine;

namespace MoreMountains.Feedbacks;

[StructLayout(LayoutKind.Sequential, Size = 1)]
public struct MMFlashEvent
{
	public delegate void Delegate(Color flashColor, float duration, float alpha, int flashID, int channel, TimescaleModes timescaleMode, bool stop = false);

	private static event Delegate OnEvent;

	public static void Register(Delegate callback)
	{
		OnEvent += callback;
	}

	public static void Unregister(Delegate callback)
	{
		OnEvent -= callback;
	}

	public static void Trigger(Color flashColor, float duration, float alpha, int flashID, int channel, TimescaleModes timescaleMode, bool stop = false)
	{
		OnEvent?.Invoke(flashColor, duration, alpha, flashID, channel, timescaleMode, stop);
	}
}
