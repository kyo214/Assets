using System.Runtime.InteropServices;
using UnityEngine;

namespace MoreMountains.Feedbacks;

[StructLayout(LayoutKind.Sequential, Size = 1)]
public struct MMFeedbacksShakeEvent
{
	public delegate void Delegate(int channel = 0, bool useRange = false, float eventRange = 0f, Vector3 eventOriginPosition = default(Vector3));

	private static event Delegate OnEvent;

	public static void Register(Delegate callback)
	{
		OnEvent += callback;
	}

	public static void Unregister(Delegate callback)
	{
		OnEvent -= callback;
	}

	public static void Trigger(int channel = 0, bool useRange = false, float eventRange = 0f, Vector3 eventOriginPosition = default(Vector3))
	{
		OnEvent?.Invoke(channel, useRange, eventRange, eventOriginPosition);
	}
}
