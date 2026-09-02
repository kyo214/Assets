using System.Runtime.InteropServices;

namespace MoreMountains.Feedbacks;

[StructLayout(LayoutKind.Sequential, Size = 1)]
public struct MMCameraZoomEvent
{
	public delegate void Delegate(MMCameraZoomModes mode, float newFieldOfView, float transitionDuration, float duration, int channel, bool useUnscaledTime = false, bool stop = false, bool relative = false);

	private static event Delegate OnEvent;

	public static void Register(Delegate callback)
	{
		OnEvent += callback;
	}

	public static void Unregister(Delegate callback)
	{
		OnEvent -= callback;
	}

	public static void Trigger(MMCameraZoomModes mode, float newFieldOfView, float transitionDuration, float duration, int channel, bool useUnscaledTime = false, bool stop = false, bool relative = false)
	{
		OnEvent?.Invoke(mode, newFieldOfView, transitionDuration, duration, channel, useUnscaledTime, stop, relative);
	}
}
