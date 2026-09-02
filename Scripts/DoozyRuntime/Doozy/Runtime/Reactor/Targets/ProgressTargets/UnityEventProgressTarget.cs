using System;
using Doozy.Runtime.Common.Events;
using UnityEngine;

namespace Doozy.Runtime.Reactor.Targets.ProgressTargets;

[AddComponentMenu("Reactor/Targets/UnityEvent Progress Target")]
public class UnityEventProgressTarget : MetaProgressTarget<FloatEvent>
{
	[SerializeField]
	private bool WholeNumbers = true;

	[SerializeField]
	private bool UseMultiplier = true;

	[SerializeField]
	private float Multiplier = 100f;

	private float m_TargetValue;

	public bool wholeNumbers
	{
		get
		{
			return WholeNumbers;
		}
		set
		{
			WholeNumbers = value;
		}
	}

	public bool useMultiplier
	{
		get
		{
			return UseMultiplier;
		}
		set
		{
			UseMultiplier = value;
		}
	}

	public float multiplier
	{
		get
		{
			return Multiplier;
		}
		set
		{
			Multiplier = value;
		}
	}

	public override void UpdateTarget(Progressor progressor)
	{
		if (base.target != null)
		{
			if (!Enum.IsDefined(typeof(Mode), base.targetMode))
			{
				base.targetMode = Mode.Value;
			}
			m_TargetValue = 0f;
			switch (base.targetMode)
			{
			case Mode.Progress:
				m_TargetValue = progressor.progress;
				break;
			case Mode.Value:
				m_TargetValue = progressor.currentValue;
				break;
			default:
				throw new ArgumentOutOfRangeException();
			}
			if (UseMultiplier)
			{
				m_TargetValue *= Multiplier;
			}
			if (WholeNumbers)
			{
				m_TargetValue = Mathf.Round(m_TargetValue);
			}
			base.target.Invoke(m_TargetValue);
		}
	}

	public override void UpdateTarget(ProgressorGroup progressorGroup)
	{
		if (base.target != null)
		{
			if (!Enum.IsDefined(typeof(Mode), base.targetMode))
			{
				base.targetMode = Mode.Progress;
			}
			base.targetMode = Mode.Progress;
			m_TargetValue = progressorGroup.progress;
			if (UseMultiplier)
			{
				m_TargetValue *= Multiplier;
			}
			if (WholeNumbers)
			{
				m_TargetValue = Mathf.Round(m_TargetValue);
			}
			base.target.Invoke(m_TargetValue);
		}
	}
}
