using System;
using System.Text;
using TMPro;
using UnityEngine;

namespace Doozy.Runtime.Reactor.Targets.ProgressTargets;

[AddComponentMenu("Reactor/Targets/TextMeshPro Progress Target")]
public class TextMeshProProgressTarget : MetaProgressTarget<TMP_Text>
{
	[SerializeField]
	private bool WholeNumbers = true;

	[SerializeField]
	private bool UseMultiplier = true;

	[SerializeField]
	private float Multiplier = 100f;

	[SerializeField]
	private string Prefix;

	[SerializeField]
	private string Suffix = "%";

	private bool m_Initialized;

	private float m_TargetValue;

	private StringBuilder m_StringBuilder = new StringBuilder();

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

	public string prefix
	{
		get
		{
			return Prefix;
		}
		set
		{
			Prefix = value;
		}
	}

	public string suffix
	{
		get
		{
			return Suffix;
		}
		set
		{
			Suffix = value;
		}
	}

	public override void UpdateTarget(Progressor progressor)
	{
		if (!m_Initialized)
		{
			Init();
		}
		if (!(base.target == null))
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
			base.target.text = m_StringBuilder.Remove(0, m_StringBuilder.Length).Append(Prefix).Append(m_TargetValue)
				.Append(Suffix)
				.ToString();
		}
	}

	public override void UpdateTarget(ProgressorGroup progressorGroup)
	{
		if (!m_Initialized)
		{
			Init();
		}
		if (!(base.target == null))
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
			base.target.text = m_StringBuilder.Remove(0, m_StringBuilder.Length).Append(Prefix).Append(m_TargetValue)
				.Append(Suffix)
				.ToString();
		}
	}

	private void Init()
	{
		if (m_StringBuilder == null)
		{
			m_StringBuilder = new StringBuilder();
		}
		m_Initialized = true;
	}
}
