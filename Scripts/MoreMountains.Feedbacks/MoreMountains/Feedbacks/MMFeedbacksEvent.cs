using System.Runtime.InteropServices;

namespace MoreMountains.Feedbacks;

[StructLayout(LayoutKind.Sequential, Size = 1)]
public struct MMFeedbacksEvent
{
	public enum EventTypes
	{
		Play = 0,
		Pause = 1,
		Resume = 2,
		Revert = 3,
		Complete = 4,
		Skip = 5
	}

	public delegate void Delegate(MMFeedbacks source, EventTypes type);

	private static event Delegate OnEvent;

	public static void Register(Delegate callback)
	{
		OnEvent += callback;
	}

	public static void Unregister(Delegate callback)
	{
		OnEvent -= callback;
	}

	public static void Trigger(MMFeedbacks source, EventTypes type)
	{
		OnEvent?.Invoke(source, type);
	}
}
