using Cinemachine;
using MoreMountains.Feedbacks;
using UnityEngine;

namespace MoreMountains.FeedbacksForThirdParty;

[AddComponentMenu("More Mountains/Feedbacks/Shakers/Cinemachine/MMCinemachinePriorityListener")]
[RequireComponent(typeof(CinemachineVirtualCameraBase))]
public class MMCinemachinePriorityListener : MonoBehaviour
{
	[HideInInspector]
	public TimescaleModes TimescaleMode;

	[Header("Priority Listener")]
	[Tooltip("the channel to listen to")]
	public int Channel;

	protected CinemachineVirtualCameraBase _camera;

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
		_camera = base.gameObject.GetComponent<CinemachineVirtualCameraBase>();
	}

	public virtual void OnMMCinemachinePriorityEvent(int channel, bool forceMaxPriority, int newPriority, bool forceTransition, CinemachineBlendDefinition blendDefinition, bool resetValuesAfterTransition, TimescaleModes timescaleMode)
	{
		TimescaleMode = timescaleMode;
		if (channel == Channel)
		{
			_camera.Priority = newPriority;
		}
		else if (forceMaxPriority)
		{
			_camera.Priority = 0;
		}
	}

	protected virtual void OnEnable()
	{
		MMCinemachinePriorityEvent.Register(OnMMCinemachinePriorityEvent);
	}

	protected virtual void OnDisable()
	{
		MMCinemachinePriorityEvent.Unregister(OnMMCinemachinePriorityEvent);
	}
}
