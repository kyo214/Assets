using UnityEngine;

namespace MoreMountains.Tools;

public class MMParentingOnStart : MonoBehaviour
{
	public enum Modes
	{
		Awake = 0,
		Start = 1,
		Script = 2
	}

	public Modes Mode;

	public Transform TargetParent;

	protected virtual void Awake()
	{
		if (Mode == Modes.Awake)
		{
			Parent();
		}
	}

	protected virtual void Start()
	{
		if (Mode == Modes.Start)
		{
			Parent();
		}
	}

	public virtual void Parent()
	{
		base.transform.SetParent(TargetParent);
	}
}
