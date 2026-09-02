using System;
using UnityEngine;

namespace MoreMountains.Tools;

[Serializable]
public class MMRadioSignalGeneratorItem
{
	public enum GeneratorItemModes
	{
		Multiply = 0,
		Additive = 1
	}

	public bool Active = true;

	public MMSignal.SignalType SignalType;

	[MMEnumCondition("SignalType", new int[] { 9 })]
	public AnimationCurve Curve = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(1f, 1f));

	[MMEnumCondition("SignalType", new int[] { 10 })]
	public MMTween.MMTweenCurve TweenCurve = MMTween.MMTweenCurve.EaseInOutQuartic;

	public GeneratorItemModes Mode;

	[Range(-1f, 1f)]
	public float Phase;

	[Range(0f, 10f)]
	public float Frequency = 5f;

	[Range(0f, 1f)]
	public float Amplitude = 1f;

	[Range(-1f, 1f)]
	public float Offset;

	public bool Invert;
}
