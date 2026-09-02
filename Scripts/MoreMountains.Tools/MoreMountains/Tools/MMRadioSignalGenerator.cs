using UnityEngine;

namespace MoreMountains.Tools;

public class MMRadioSignalGenerator : MMRadioSignal
{
	public bool AnimatedPreview;

	public bool BackAndForth;

	[MMCondition("BackAndForth", true)]
	public float BackAndForthMirrorPoint = 0.5f;

	public MMRadioSignalGeneratorItemList SignalList;

	[MMVector(new string[] { "Min", "Max" })]
	public Vector2 Clamps = new Vector2(0f, 1f);

	[Range(0f, 1f)]
	public float Bias = 0.5f;

	private void Reset()
	{
		SignalList = new MMRadioSignalGeneratorItemList
		{
			new MMRadioSignalGeneratorItem()
		};
	}

	public virtual float Evaluate(float time)
	{
		float num = 1f;
		if (SignalList.Count <= 0)
		{
			return num;
		}
		time = ApplyBias(time, Bias);
		for (int i = 0; i < SignalList.Count; i++)
		{
			if (SignalList[i].Active)
			{
				float valueNormalized = MMSignal.GetValueNormalized(time, SignalList[i].SignalType, SignalList[i].Phase, SignalList[i].Amplitude, SignalList[i].Frequency, SignalList[i].Offset, SignalList[i].Invert, SignalList[i].Curve, SignalList[i].TweenCurve, clamp: true, Clamps.x, Clamps.y, BackAndForth, BackAndForthMirrorPoint);
				num = ((SignalList[i].Mode == MMRadioSignalGeneratorItem.GeneratorItemModes.Multiply) ? (num * valueNormalized) : (num + valueNormalized));
			}
		}
		CurrentLevel *= GlobalMultiplier;
		CurrentLevel = Mathf.Clamp(CurrentLevel, Clamps.x, Clamps.y);
		return num;
	}

	protected override void Shake()
	{
		base.Shake();
		if (Playing)
		{
			if (SignalMode == SignalModes.OneTime)
			{
				float x = base.TimescaleTime - _shakeStartedTimestamp;
				CurrentLevel = Evaluate(MMMaths.Remap(x, 0f, Duration, 0f, 1f));
			}
			else
			{
				CurrentLevel = Evaluate(DriverTime);
			}
		}
	}

	protected override void ShakeComplete()
	{
		base.ShakeComplete();
		CurrentLevel = Evaluate(1f);
	}

	public override float GraphValue(float time)
	{
		time = ApplyBias(time, Bias);
		return Evaluate(time);
	}
}
