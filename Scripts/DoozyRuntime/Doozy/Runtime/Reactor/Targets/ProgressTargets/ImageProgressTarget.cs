using System;
using UnityEngine;
using UnityEngine.UI;

namespace Doozy.Runtime.Reactor.Targets.ProgressTargets;

[AddComponentMenu("Reactor/Targets/Image Progress Target")]
public class ImageProgressTarget : MetaProgressTarget<Image>
{
	public override void UpdateTarget(Progressor progressor)
	{
		if (!(base.target == null))
		{
			if (!Enum.IsDefined(typeof(Mode), base.targetMode))
			{
				base.targetMode = Mode.Value;
			}
			switch (base.targetMode)
			{
			case Mode.Progress:
				base.target.fillAmount = Mathf.Clamp01(progressor.progress);
				break;
			case Mode.Value:
				base.target.fillAmount = Mathf.Clamp01(progressor.currentValue);
				break;
			default:
				throw new ArgumentOutOfRangeException();
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
			base.target.fillAmount = Mathf.Clamp01(progressorGroup.progress);
		}
	}
}
