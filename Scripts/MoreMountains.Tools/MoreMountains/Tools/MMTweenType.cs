using System;
using UnityEngine;

namespace MoreMountains.Tools;

[Serializable]
public class MMTweenType
{
	public MMTweenDefinitionTypes MMTweenDefinitionType;

	public MMTween.MMTweenCurve MMTweenCurve = MMTween.MMTweenCurve.EaseInCubic;

	public AnimationCurve Curve = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(1f, 1f));

	public MMTweenType(MMTween.MMTweenCurve newCurve)
	{
		MMTweenCurve = newCurve;
		MMTweenDefinitionType = MMTweenDefinitionTypes.MMTween;
	}

	public MMTweenType(AnimationCurve newCurve)
	{
		Curve = newCurve;
		MMTweenDefinitionType = MMTweenDefinitionTypes.AnimationCurve;
	}
}
