using UnityEngine;

namespace MoreMountains.Tools;

[AddComponentMenu("More Mountains/Tools/Activation/MMConditionalActivation")]
public class MMConditionalActivation : MonoBehaviour
{
	public MonoBehaviour[] EnableThese;

	public MonoBehaviour[] AfterTheseAreAllDisabled;

	protected bool _enabled;

	protected virtual void Update()
	{
		if (_enabled)
		{
			return;
		}
		bool flag = true;
		MonoBehaviour[] afterTheseAreAllDisabled = AfterTheseAreAllDisabled;
		for (int i = 0; i < afterTheseAreAllDisabled.Length; i++)
		{
			if (afterTheseAreAllDisabled[i].isActiveAndEnabled)
			{
				flag = false;
			}
		}
		if (flag)
		{
			afterTheseAreAllDisabled = EnableThese;
			for (int i = 0; i < afterTheseAreAllDisabled.Length; i++)
			{
				afterTheseAreAllDisabled[i].enabled = true;
			}
			_enabled = true;
		}
	}
}
