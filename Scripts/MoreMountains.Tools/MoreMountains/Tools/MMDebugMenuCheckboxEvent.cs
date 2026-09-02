using System.Runtime.InteropServices;

namespace MoreMountains.Tools;

[StructLayout(LayoutKind.Sequential, Size = 1)]
public struct MMDebugMenuCheckboxEvent
{
	public enum EventModes
	{
		FromCheckbox = 0,
		SetCheckbox = 1
	}

	public delegate void Delegate(string checkboxEventName, bool value, EventModes eventMode = EventModes.FromCheckbox);

	private static event Delegate OnEvent;

	public static void Register(Delegate callback)
	{
		OnEvent += callback;
	}

	public static void Unregister(Delegate callback)
	{
		OnEvent -= callback;
	}

	public static void Trigger(string checkboxEventName, bool value, EventModes eventMode = EventModes.FromCheckbox)
	{
		OnEvent?.Invoke(checkboxEventName, value, eventMode);
	}
}
