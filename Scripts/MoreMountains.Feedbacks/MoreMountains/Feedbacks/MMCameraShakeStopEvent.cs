using System.Runtime.InteropServices;

namespace MoreMountains.Feedbacks;

[StructLayout(LayoutKind.Sequential, Size = 1)]
public struct MMCameraShakeStopEvent
{
	public delegate void Delegate(int channel);

	private static event Delegate OnEvent;

	public static void Register(Delegate callback)
	{
		OnEvent += callback;
	}

	public static void Unregister(Delegate callback)
	{
		OnEvent -= callback;
	}

	public static void Trigger(int channel)
	{
		OnEvent?.Invoke(channel);
	}
}
