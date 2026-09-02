using System.Collections.Generic;
using UnityEngine;

namespace MoreMountains.Feedbacks;

[AddComponentMenu("")]
[FeedbackHelp("This feedback will allow you to send to an animator (bound in its inspector) a bool, int, float or trigger parameter, allowing you to trigger an animation, with or without randomness.")]
[FeedbackPath("GameObject/Animation")]
public class MMFeedbackAnimation : MMFeedback
{
	public enum TriggerModes
	{
		SetTrigger = 0,
		ResetTrigger = 1
	}

	public enum ValueModes
	{
		None = 0,
		Constant = 1,
		Random = 2,
		Incremental = 3
	}

	public static bool FeedbackTypeAuthorized = true;

	[Header("Animation")]
	[Tooltip("the animator whose parameters you want to update")]
	public Animator BoundAnimator;

	[Header("Trigger")]
	[Tooltip("if this is true, will update the specified trigger parameter")]
	public bool UpdateTrigger;

	[Tooltip("the selected mode to interact with this trigger")]
	[MMFCondition("UpdateTrigger", true)]
	public TriggerModes TriggerMode;

	[Tooltip("the trigger animator parameter to, well, trigger when the feedback is played")]
	[MMFCondition("UpdateTrigger", true)]
	public string TriggerParameterName;

	[Header("Random Trigger")]
	[Tooltip("if this is true, will update a random trigger parameter, picked from the list below")]
	public bool UpdateRandomTrigger;

	[Tooltip("the selected mode to interact with this trigger")]
	[MMFCondition("UpdateRandomTrigger", true)]
	public TriggerModes RandomTriggerMode;

	[Tooltip("the trigger animator parameters to trigger at random when the feedback is played")]
	public List<string> RandomTriggerParameterNames;

	[Header("Bool")]
	[Tooltip("if this is true, will update the specified bool parameter")]
	public bool UpdateBool;

	[Tooltip("the bool parameter to turn true when the feedback gets played")]
	[MMFCondition("UpdateBool", true)]
	public string BoolParameterName;

	[Tooltip("when in bool mode, whether to set the bool parameter to true or false")]
	[MMFCondition("UpdateBool", true)]
	public bool BoolParameterValue = true;

	[Header("Random Bool")]
	[Tooltip("if this is true, will update a random bool parameter picked from the list below")]
	public bool UpdateRandomBool;

	[Tooltip("when in bool mode, whether to set the bool parameter to true or false")]
	[MMFCondition("UpdateRandomBool", true)]
	public bool RandomBoolParameterValue = true;

	[Tooltip("the bool parameter to turn true when the feedback gets played")]
	public List<string> RandomBoolParameterNames;

	[Header("Int")]
	[Tooltip("the int parameter to turn true when the feedback gets played")]
	public ValueModes IntValueMode;

	[Tooltip("the int parameter to turn true when the feedback gets played")]
	[MMFEnumCondition("IntValueMode", new int[] { 1, 2, 3 })]
	public string IntParameterName;

	[Tooltip("the value to set to that int parameter")]
	[MMFEnumCondition("IntValueMode", new int[] { 1 })]
	public int IntValue;

	[Tooltip("the min value (inclusive) to set at random to that int parameter")]
	[MMFEnumCondition("IntValueMode", new int[] { 2 })]
	public int IntValueMin;

	[Tooltip("the max value (exclusive) to set at random to that int parameter")]
	[MMFEnumCondition("IntValueMode", new int[] { 2 })]
	public int IntValueMax = 5;

	[Tooltip("the value to increment that int parameter by")]
	[MMFEnumCondition("IntValueMode", new int[] { 3 })]
	public int IntIncrement = 1;

	[Header("Float")]
	[Tooltip("the Float parameter to turn true when the feedback gets played")]
	public ValueModes FloatValueMode;

	[Tooltip("the float parameter to turn true when the feedback gets played")]
	[MMFEnumCondition("FloatValueMode", new int[] { 1, 2, 3 })]
	public string FloatParameterName;

	[Tooltip("the value to set to that float parameter")]
	[MMFEnumCondition("FloatValueMode", new int[] { 1 })]
	public float FloatValue;

	[Tooltip("the min value (inclusive) to set at random to that float parameter")]
	[MMFEnumCondition("FloatValueMode", new int[] { 2 })]
	public float FloatValueMin;

	[Tooltip("the max value (exclusive) to set at random to that float parameter")]
	[MMFEnumCondition("FloatValueMode", new int[] { 2 })]
	public float FloatValueMax = 5f;

	[Tooltip("the value to increment that float parameter by")]
	[MMFEnumCondition("FloatValueMode", new int[] { 3 })]
	public float FloatIncrement = 1f;

	protected int _triggerParameter;

	protected int _boolParameter;

	protected int _intParameter;

	protected int _floatParameter;

	protected List<int> _randomTriggerParameters;

	protected List<int> _randomBoolParameters;

	protected override void CustomInitialization(GameObject owner)
	{
		base.CustomInitialization(owner);
		_triggerParameter = Animator.StringToHash(TriggerParameterName);
		_boolParameter = Animator.StringToHash(BoolParameterName);
		_intParameter = Animator.StringToHash(IntParameterName);
		_floatParameter = Animator.StringToHash(FloatParameterName);
		_randomTriggerParameters = new List<int>();
		foreach (string randomTriggerParameterName in RandomTriggerParameterNames)
		{
			_randomTriggerParameters.Add(Animator.StringToHash(randomTriggerParameterName));
		}
		_randomBoolParameters = new List<int>();
		foreach (string randomBoolParameterName in RandomBoolParameterNames)
		{
			_randomBoolParameters.Add(Animator.StringToHash(randomBoolParameterName));
		}
	}

	protected override void CustomPlayFeedback(Vector3 position, float feedbacksIntensity = 1f)
	{
		if (!Active || !FeedbackTypeAuthorized)
		{
			return;
		}
		if (BoundAnimator == null)
		{
			Debug.LogWarning("No animator was set for " + base.name);
			return;
		}
		float num = (Timing.ConstantIntensity ? 1f : feedbacksIntensity);
		if (UpdateTrigger)
		{
			if (TriggerMode == TriggerModes.SetTrigger)
			{
				BoundAnimator.SetTrigger(_triggerParameter);
			}
			if (TriggerMode == TriggerModes.ResetTrigger)
			{
				BoundAnimator.ResetTrigger(_triggerParameter);
			}
		}
		if (UpdateRandomTrigger)
		{
			int num2 = _randomTriggerParameters[Random.Range(0, _randomTriggerParameters.Count)];
			if (RandomTriggerMode == TriggerModes.SetTrigger)
			{
				BoundAnimator.SetTrigger(num2);
			}
			if (RandomTriggerMode == TriggerModes.ResetTrigger)
			{
				BoundAnimator.ResetTrigger(num2);
			}
		}
		if (UpdateBool)
		{
			BoundAnimator.SetBool(_boolParameter, BoolParameterValue);
		}
		if (UpdateRandomBool)
		{
			int id = _randomBoolParameters[Random.Range(0, _randomBoolParameters.Count)];
			BoundAnimator.SetBool(id, RandomBoolParameterValue);
		}
		switch (IntValueMode)
		{
		case ValueModes.Constant:
			BoundAnimator.SetInteger(_intParameter, IntValue);
			break;
		case ValueModes.Incremental:
		{
			int value2 = BoundAnimator.GetInteger(_intParameter) + IntIncrement;
			BoundAnimator.SetInteger(_intParameter, value2);
			break;
		}
		case ValueModes.Random:
		{
			int value = Random.Range(IntValueMin, IntValueMax);
			BoundAnimator.SetInteger(_intParameter, value);
			break;
		}
		}
		switch (FloatValueMode)
		{
		case ValueModes.Constant:
			BoundAnimator.SetFloat(_floatParameter, FloatValue * num);
			break;
		case ValueModes.Incremental:
		{
			float value4 = BoundAnimator.GetFloat(_floatParameter) + FloatIncrement * num;
			BoundAnimator.SetFloat(_floatParameter, value4);
			break;
		}
		case ValueModes.Random:
		{
			float value3 = Random.Range(FloatValueMin, FloatValueMax) * num;
			BoundAnimator.SetFloat(_floatParameter, value3);
			break;
		}
		}
	}

	protected override void CustomStopFeedback(Vector3 position, float feedbacksIntensity = 1f)
	{
		if (Active && UpdateBool && FeedbackTypeAuthorized)
		{
			BoundAnimator.SetBool(_boolParameter, value: false);
		}
	}
}
