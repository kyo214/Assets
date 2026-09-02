using UnityEngine;

namespace Doozy.Runtime.Reactor;

public abstract class ProgressTarget : MonoBehaviour
{
	public enum Mode
	{
		Progress = 0,
		Value = 1
	}

	[SerializeField]
	private Mode TargetMode = Mode.Value;

	public Mode targetMode
	{
		get
		{
			return TargetMode;
		}
		set
		{
			TargetMode = value;
		}
	}

	public abstract void UpdateTarget(Progressor progressor);

	public abstract void UpdateTarget(ProgressorGroup progressorGroup);
}
