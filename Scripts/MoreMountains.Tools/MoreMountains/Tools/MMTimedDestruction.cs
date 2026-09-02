using System.Collections;
using UnityEngine;

namespace MoreMountains.Tools;

[AddComponentMenu("More Mountains/Tools/Activation/MMTimedDestruction")]
public class MMTimedDestruction : MonoBehaviour
{
	public enum TimedDestructionModes
	{
		Destroy = 0,
		Disable = 1
	}

	public TimedDestructionModes TimeDestructionMode;

	public float TimeBeforeDestruction = 2f;

	protected virtual void Start()
	{
		StartCoroutine(Destruction());
	}

	protected virtual IEnumerator Destruction()
	{
		yield return MMCoroutine.WaitFor(TimeBeforeDestruction);
		if (TimeDestructionMode == TimedDestructionModes.Destroy)
		{
			Object.Destroy(base.gameObject);
		}
		else
		{
			base.gameObject.SetActive(value: false);
		}
	}
}
