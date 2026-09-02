using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace MoreMountains.Tools;

[AddComponentMenu("More Mountains/Tools/Activation/MMTimedActivation")]
public class MMTimedActivation : MonoBehaviour
{
	public enum TimedStatusChange
	{
		Enable = 0,
		Disable = 1,
		Destroy = 2
	}

	public enum ActivationModes
	{
		Awake = 0,
		Start = 1,
		OnEnable = 2,
		OnTriggerEnter = 3,
		OnTriggerExit = 4,
		OnTriggerEnter2D = 5,
		OnTriggerExit2D = 6,
		Script = 7
	}

	public enum TriggerModes
	{
		None = 0,
		Tag = 1,
		Layer = 2
	}

	public enum DelayModes
	{
		Time = 0,
		Frames = 1
	}

	[Header("Trigger Mode")]
	public ActivationModes ActivationMode = ActivationModes.Start;

	[MMEnumCondition("ActivationMode", new int[] { 3, 4 })]
	public TriggerModes TriggerMode;

	[MMEnumCondition("TriggerMode", new int[] { 2 })]
	public LayerMask TargetTriggerLayer;

	[MMEnumCondition("TriggerMode", new int[] { 1 })]
	public string TargetTriggerTag;

	[Header("Delay")]
	public DelayModes DelayMode;

	[MMEnumCondition("DelayMode", new int[] { 0 })]
	public float TimeBeforeStateChange = 2f;

	[MMEnumCondition("DelayMode", new int[] { 1 })]
	public int FrameCount = 1;

	[Header("Timed Activation")]
	public List<GameObject> TargetGameObjects;

	public List<MonoBehaviour> TargetBehaviours;

	public TimedStatusChange TimeDestructionMode = TimedStatusChange.Disable;

	[Header("Actions")]
	public UnityEvent TimedActions;

	protected virtual void Awake()
	{
		if (ActivationMode == ActivationModes.Awake)
		{
			StartChangeState();
		}
	}

	public virtual void TriggerSequence()
	{
		StartChangeState();
	}

	protected virtual void Start()
	{
		if (ActivationMode == ActivationModes.Start)
		{
			StartChangeState();
		}
	}

	protected virtual void OnEnable()
	{
		if (ActivationMode == ActivationModes.OnEnable)
		{
			StartChangeState();
		}
	}

	protected virtual void OnTriggerEnter(Collider collider)
	{
		if (ActivationMode == ActivationModes.OnTriggerEnter && CorrectTagOrLayer(collider.gameObject))
		{
			StartChangeState();
		}
	}

	protected virtual void OnTriggerExit(Collider collider)
	{
		if (ActivationMode == ActivationModes.OnTriggerEnter && CorrectTagOrLayer(collider.gameObject))
		{
			StartChangeState();
		}
	}

	protected virtual void OnTriggerEnter2d(Collider2D collider)
	{
		if (ActivationMode == ActivationModes.OnTriggerEnter && CorrectTagOrLayer(collider.gameObject))
		{
			StartChangeState();
		}
	}

	protected virtual void OnTriggerExit2d(Collider2D collider)
	{
		if (ActivationMode == ActivationModes.OnTriggerEnter && CorrectTagOrLayer(collider.gameObject))
		{
			StartChangeState();
		}
	}

	protected virtual bool CorrectTagOrLayer(GameObject target)
	{
		switch (TriggerMode)
		{
		case TriggerModes.None:
			return true;
		case TriggerModes.Layer:
			if (((1 << target.layer) & (int)TargetTriggerLayer) != 0)
			{
				return true;
			}
			return false;
		case TriggerModes.Tag:
			return target.CompareTag(TargetTriggerTag);
		default:
			return false;
		}
	}

	protected virtual void StartChangeState()
	{
		StartCoroutine(TimedActivationSequence());
	}

	protected virtual IEnumerator TimedActivationSequence()
	{
		if (DelayMode == DelayModes.Time)
		{
			yield return MMCoroutine.WaitFor(TimeBeforeStateChange);
		}
		else
		{
			yield return StartCoroutine(MMCoroutine.WaitForFrames(FrameCount));
		}
		StateChange();
		Activate();
	}

	protected virtual void Activate()
	{
		if (TimedActions != null)
		{
			TimedActions.Invoke();
		}
	}

	protected virtual void StateChange()
	{
		foreach (GameObject targetGameObject in TargetGameObjects)
		{
			switch (TimeDestructionMode)
			{
			case TimedStatusChange.Destroy:
				Object.Destroy(targetGameObject);
				break;
			case TimedStatusChange.Disable:
				targetGameObject.SetActive(value: false);
				break;
			case TimedStatusChange.Enable:
				targetGameObject.SetActive(value: true);
				break;
			}
		}
		foreach (MonoBehaviour targetBehaviour in TargetBehaviours)
		{
			switch (TimeDestructionMode)
			{
			case TimedStatusChange.Destroy:
				Object.Destroy(targetBehaviour);
				break;
			case TimedStatusChange.Disable:
				targetBehaviour.enabled = false;
				break;
			case TimedStatusChange.Enable:
				targetBehaviour.enabled = true;
				break;
			}
		}
	}
}
