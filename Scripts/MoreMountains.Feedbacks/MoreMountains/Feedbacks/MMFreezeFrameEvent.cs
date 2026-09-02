using System.Runtime.InteropServices;

namespace MoreMountains.Feedbacks;

[StructLayout(LayoutKind.Sequential, Size = 1)]
public struct MMFreezeFrameEvent
{
	public delegate void Delegate(float duration);

	private static event Delegate OnEvent;

	public static void Register(Delegate callback)
	{
		OnEvent += callback;
	}

	public static void Unregister(Delegate callback)
	{
		OnEvent -= callback;
	}

	public static void Trigger(float duration)
	{
		OnEvent?.Invoke(duration);
	}
}
