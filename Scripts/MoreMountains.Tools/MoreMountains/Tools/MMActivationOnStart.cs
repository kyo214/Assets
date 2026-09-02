using System.Collections.Generic;
using UnityEngine;

namespace MoreMountains.Tools;

[AddComponentMenu("More Mountains/Tools/Activation/MMActivationOnStart")]
public class MMActivationOnStart : MonoBehaviour
{
	public enum Modes
	{
		Awake = 0,
		Start = 1
	}

	public Modes Mode = Modes.Start;

	public bool StateOnStart = true;

	public List<GameObject> TargetObjects;

	protected virtual void Awake()
	{
		if (Mode == Modes.Awake)
		{
			SetState();
		}
	}

	protected virtual void Start()
	{
		if (Mode == Modes.Start)
		{
			SetState();
		}
	}

	protected virtual void SetState()
	{
		foreach (GameObject targetObject in TargetObjects)
		{
			targetObject.SetActive(StateOnStart);
		}
	}
}
