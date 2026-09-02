using System;
using UnityEngine;
using UnityEngine.Audio;

namespace Doozy.Runtime.Reactor.Targets.ProgressTargets;

[AddComponentMenu("Reactor/Targets/AudioMixer Progress Target")]
public class AudioMixerProgressTarget : MetaProgressTarget<AudioMixer>
{
	private const float MIN_VALUE = 0.0001f;

	private const float MAX_VALUE = 1f;

	[SerializeField]
	private string ExposedParameterName;

	[SerializeField]
	private bool UseLogarithmicConversion = true;

	public string exposedParameterName
	{
		get
		{
			return ExposedParameterName;
		}
		set
		{
			ExposedParameterName = value;
		}
	}

	public bool useLogarithmicConversion
	{
		get
		{
			return UseLogarithmicConversion;
		}
		set
		{
			UseLogarithmicConversion = value;
		}
	}

	private void Awake()
	{
		base.targetMode = Mode.Value;
	}

	public override void UpdateTarget(Progressor progressor)
	{
		if (!(base.target == null))
		{
			if (!Enum.IsDefined(typeof(Mode), base.targetMode))
			{
				base.targetMode = Mode.Value;
			}
			if (UseLogarithmicConversion)
			{
				base.target.SetFloat(ExposedParameterName, GetLogarithmicValue(progressor.progress));
			}
			else
			{
				base.target.SetFloat(ExposedParameterName, progressor.currentValue);
			}
		}
	}

	public override void UpdateTarget(ProgressorGroup progressorGroup)
	{
		if (!(base.target == null))
		{
			if (!Enum.IsDefined(typeof(Mode), base.targetMode))
			{
				base.targetMode = Mode.Progress;
			}
			base.targetMode = Mode.Progress;
			if (UseLogarithmicConversion)
			{
				base.target.SetFloat(ExposedParameterName, GetLogarithmicValue(progressorGroup.progress));
			}
			else
			{
				base.target.SetFloat(ExposedParameterName, progressorGroup.progress);
			}
		}
	}

	private static float GetLogarithmicValue(float value)
	{
		return Mathf.Log10(Mathf.Clamp(value, 0.0001f, 1f)) * 20f;
	}
}
