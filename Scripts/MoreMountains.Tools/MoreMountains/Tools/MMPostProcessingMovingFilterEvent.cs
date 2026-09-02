using System.Runtime.InteropServices;

namespace MoreMountains.Tools;

[StructLayout(LayoutKind.Sequential, Size = 1)]
public struct MMPostProcessingMovingFilterEvent
{
	public delegate void Delegate(MMTweenType curve, bool active, bool toggle, float duration, int channel = 0, bool stop = false);

	private static event Delegate OnEvent;

	public static void Register(Delegate callback)
	{
		OnEvent += callback;
	}

	public static void Unregister(Delegate callback)
	{
		OnEvent -= callback;
	}

	public static void Trigger(MMTweenType curve, bool active, bool toggle, float duration, int channel = 0, bool stop = false)
	{
		OnEvent?.Invoke(curve, active, toggle, duration, channel, stop);
	}
}
