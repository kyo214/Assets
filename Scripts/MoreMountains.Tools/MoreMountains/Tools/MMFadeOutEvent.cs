using UnityEngine;

namespace MoreMountains.Tools;

public struct MMFadeOutEvent(float duration, MMTweenType tween, int id = 0, bool ignoreTimeScale = true, Vector3 worldPosition = default(Vector3))
{
	public int ID = id;

	public float Duration = duration;

	public MMTweenType Curve = tween;

	public bool IgnoreTimeScale = ignoreTimeScale;

	public Vector3 WorldPosition = worldPosition;

	private static MMFadeOutEvent e;

	public static void Trigger(float duration, MMTweenType tween, int id = 0, bool ignoreTimeScale = true, Vector3 worldPosition = default(Vector3))
	{
		e.ID = id;
		e.Duration = duration;
		e.Curve = tween;
		e.IgnoreTimeScale = ignoreTimeScale;
		e.WorldPosition = worldPosition;
		MMEventManager.TriggerEvent(e);
	}
}
