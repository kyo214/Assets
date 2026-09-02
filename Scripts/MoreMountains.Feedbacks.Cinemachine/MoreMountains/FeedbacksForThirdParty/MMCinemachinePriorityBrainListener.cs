using System.Collections;
using Cinemachine;
using MoreMountains.Feedbacks;
using UnityEngine;

namespace MoreMountains.FeedbacksForThirdParty;

[AddComponentMenu("More Mountains/Feedbacks/Shakers/Cinemachine/MMCinemachinePriorityBrainListener")]
[RequireComponent(typeof(CinemachineBrain))]
public class MMCinemachinePriorityBrainListener : MonoBehaviour
{
	[HideInInspector]
	public TimescaleModes TimescaleMode;

	protected CinemachineBrain _brain;

	protected CinemachineBlendDefinition _initialDefinition;

	protected Coroutine _coroutine;

	public virtual float GetTime()
	{
		if (TimescaleMode != TimescaleModes.Scaled)
		{
			return Time.unscaledTime;
		}
		return Time.time;
	}

	public virtual float GetDeltaTime()
	{
		if (TimescaleMode != TimescaleModes.Scaled)
		{
			return Time.unscaledDeltaTime;
		}
		return Time.deltaTime;
	}

	protected virtual void Awake()
	{
		_brain = base.gameObject.GetComponent<CinemachineBrain>();
	}

	public virtual void OnMMCinemachinePriorityEvent(int channel, bool forceMaxPriority, int newPriority, bool forceTransition, CinemachineBlendDefinition blendDefinition, bool resetValuesAfterTransition, TimescaleModes timescaleMode)
	{
		if (forceTransition)
		{
			if (_coroutine != null)
			{
				StopCoroutine(_coroutine);
			}
			else
			{
				_initialDefinition = _brain.m_DefaultBlend;
			}
			_brain.m_DefaultBlend = blendDefinition;
			TimescaleMode = timescaleMode;
			_coroutine = StartCoroutine(ResetBlendDefinition(blendDefinition.m_Time));
		}
	}

	protected virtual IEnumerator ResetBlendDefinition(float delay)
	{
		for (float timer = 0f; timer < delay; timer += GetDeltaTime())
		{
			yield return null;
		}
		_brain.m_DefaultBlend = _initialDefinition;
		_coroutine = null;
	}

	protected virtual void OnEnable()
	{
		_coroutine = null;
		MMCinemachinePriorityEvent.Register(OnMMCinemachinePriorityEvent);
	}

	protected virtual void OnDisable()
	{
		if (_coroutine != null)
		{
			StopCoroutine(_coroutine);
		}
		_coroutine = null;
		MMCinemachinePriorityEvent.Unregister(OnMMCinemachinePriorityEvent);
	}
}
