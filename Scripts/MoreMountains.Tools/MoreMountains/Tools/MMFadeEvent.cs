using UnityEngine;

namespace MoreMountains.Tools;

public struct MMFadeEvent(float duration, float targetAlpha, MMTweenType tween, int id = 0, bool ignoreTimeScale = true, Vector3 worldPosition = default(Vector3))
{
	public int ID = id;

	public float Duration = duration;

	public float TargetAlpha = targetAlpha;

	public MMTweenType Curve = tween;

	public bool IgnoreTimeScale = ignoreTimeScale;

	public Vector3 WorldPosition = worldPosition;

	private static MMFadeEvent e;

	public static void Trigger(float duration, float targetAlpha)
	{
		Trigger(duration, targetAlpha, new MMTweenType(MMTween.MMTweenCurve.EaseInCubic));
	}

	public static void Trigger(float duration, float targetAlpha, MMTweenType tween, int id = 0, bool ignoreTimeScale = true, Vector3 worldPosition = default(Vector3))
	{
		e.ID = id;
		e.Duration = duration;
		e.TargetAlpha = targetAlpha;
		e.Curve = tween;
		e.IgnoreTimeScale = ignoreTimeScale;
		e.WorldPosition = worldPosition;
		MMEventManager.TriggerEvent(e);
	}
}
