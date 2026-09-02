using System.Collections;
using MoreMountains.Tools;
using TMPro;
using UnityEngine;

namespace MoreMountains.Feedbacks;

[AddComponentMenu("")]
[FeedbackHelp("This feedback will let you update a TMP text value over time, with a value going from A to B over time, on a curve")]
[FeedbackPath("TextMesh Pro/TMP Count To")]
public class MMF_TMPCountTo : MMF_Feedback
{
	public static bool FeedbackTypeAuthorized = true;

	[MMFInspectorGroup("TextMeshPro Target Text", true, 12, true, false)]
	[Tooltip("the target TMP_Text component we want to change the text on")]
	public TMP_Text TargetTMPText;

	[MMFInspectorGroup("Count Settings", true, 13, false, false)]
	[Tooltip("the value from which to count from")]
	public float CountFrom;

	[Tooltip("the value to count towards")]
	public float CountTo = 10f;

	[Tooltip("the curve on which to animate the count")]
	public MMTweenType CountingCurve = new MMTweenType(new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(0.3f, 1f), new Keyframe(1f, 0f)));

	[Tooltip("the duration of the count, in seconds")]
	public float Duration = 5f;

	[Tooltip("the format with which to display the count")]
	public string Format = "00.00";

	[Tooltip("whether or not value should be floored")]
	public bool FloorValues = true;

	[Tooltip("the minimum frequency (in seconds) at which to refresh the text field")]
	public float MinRefreshFrequency;

	protected string _newText;

	protected float _startTime;

	protected float _lastRefreshAt;

	public override float FeedbackDuration
	{
		get
		{
			return ApplyTimeMultiplier(Duration);
		}
		set
		{
			Duration = value;
		}
	}

	protected override void CustomPlayFeedback(Vector3 position, float feedbacksIntensity = 1f)
	{
		if (Active && FeedbackTypeAuthorized && !(TargetTMPText == null))
		{
			Owner.StartCoroutine(CountCo());
		}
	}

	protected virtual IEnumerator CountCo()
	{
		_lastRefreshAt = float.MinValue;
		_ = CountFrom;
		_startTime = FeedbackTime;
		while (FeedbackTime - _startTime <= Duration)
		{
			if (FeedbackTime - _lastRefreshAt >= MinRefreshFrequency)
			{
				float currentValue = ProcessCount();
				UpdateText(currentValue);
				_lastRefreshAt = FeedbackTime;
			}
			yield return null;
		}
		UpdateText(CountTo);
	}

	protected virtual void UpdateText(float currentValue)
	{
		if (FloorValues)
		{
			_newText = Mathf.Floor(currentValue).ToString(Format);
		}
		else
		{
			_newText = currentValue.ToString(Format);
		}
		TargetTMPText.text = _newText;
	}

	protected virtual float ProcessCount()
	{
		return MMTween.Tween(FeedbackTime - _startTime, 0f, Duration, CountFrom, CountTo, CountingCurve);
	}
}
