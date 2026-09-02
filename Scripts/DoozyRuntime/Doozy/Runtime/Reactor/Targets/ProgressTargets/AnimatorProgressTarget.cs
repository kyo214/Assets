using System;
using UnityEngine;

namespace Doozy.Runtime.Reactor.Targets.ProgressTargets;

[AddComponentMenu("Reactor/Targets/Animator Progress Target")]
public class AnimatorProgressTarget : MetaProgressTarget<Animator>
{
	[SerializeField]
	private string ParameterName = "Progress";

	public string parameterName
	{
		get
		{
			return ParameterName;
		}
		set
		{
			ParameterName = value;
		}
	}

	public override void UpdateTarget(Progressor progressor)
	{
		if (!(base.target == null) && base.target.gameObject.activeSelf && base.target.isActiveAndEnabled)
		{
			if (!Enum.IsDefined(typeof(Mode), base.targetMode))
			{
				base.targetMode = Mode.Progress;
			}
			switch (base.targetMode)
			{
			case Mode.Progress:
				base.target.SetFloat(ParameterName, progressor.progress);
				break;
			case Mode.Value:
				base.target.SetFloat(ParameterName, progressor.currentValue);
				break;
			default:
				throw new ArgumentOutOfRangeException();
			}
		}
	}

	public override void UpdateTarget(ProgressorGroup progressorGroup)
	{
		if (!(base.target == null) && base.target.gameObject.activeSelf && base.target.isActiveAndEnabled)
		{
			if (!Enum.IsDefined(typeof(Mode), base.targetMode))
			{
				base.targetMode = Mode.Progress;
			}
			base.targetMode = Mode.Progress;
			base.target.SetFloat(ParameterName, progressorGroup.progress);
		}
	}
}
