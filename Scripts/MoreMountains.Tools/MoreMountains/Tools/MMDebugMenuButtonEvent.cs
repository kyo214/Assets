using System.Runtime.InteropServices;

namespace MoreMountains.Tools;

[StructLayout(LayoutKind.Sequential, Size = 1)]
public struct MMDebugMenuButtonEvent
{
	public enum EventModes
	{
		FromButton = 0,
		SetButton = 1
	}

	public delegate void Delegate(string buttonEventName, bool active = true, EventModes eventMode = EventModes.FromButton);

	private static event Delegate OnEvent;

	public static void Register(Delegate callback)
	{
		OnEvent += callback;
	}

	public static void Unregister(Delegate callback)
	{
		OnEvent -= callback;
	}

	public static void Trigger(string buttonEventName, bool active = true, EventModes eventMode = EventModes.FromButton)
	{
		OnEvent?.Invoke(buttonEventName, active, eventMode);
	}
}
