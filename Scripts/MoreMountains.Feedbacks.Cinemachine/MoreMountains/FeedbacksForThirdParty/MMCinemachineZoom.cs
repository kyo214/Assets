using Cinemachine;
using MoreMountains.Feedbacks;
using UnityEngine;

namespace MoreMountains.FeedbacksForThirdParty;

[AddComponentMenu("More Mountains/Feedbacks/Shakers/Cinemachine/MMCinemachineZoom")]
[RequireComponent(typeof(CinemachineVirtualCamera))]
public class MMCinemachineZoom : MonoBehaviour
{
	public int Channel;

	[Header("Transition Speed")]
	[Tooltip("the animation curve to apply to the zoom transition")]
	public AnimationCurve ZoomCurve = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(1f, 1f));

	[Header("Test Zoom")]
	[Tooltip("the mode to apply the zoom in when using the test button in the inspector")]
	public MMCameraZoomModes TestMode;

	[Tooltip("the target field of view to apply the zoom in when using the test button in the inspector")]
	public float TestFieldOfView = 30f;

	[Tooltip("the transition duration to apply the zoom in when using the test button in the inspector")]
	public float TestTransitionDuration = 0.1f;

	[Tooltip("the duration to apply the zoom in when using the test button in the inspector")]
	public float TestDuration = 0.05f;

	[MMFInspectorButton("TestZoom")]
	public bool TestZoomButton;

	protected CinemachineVirtualCamera _virtualCamera;

	protected float _initialFieldOfView;

	protected MMCameraZoomModes _mode;

	protected bool _zooming;

	protected float _startFieldOfView;

	protected float _transitionDuration;

	protected float _duration;

	protected float _targetFieldOfView;

	protected float _delta;

	protected int _direction = 1;

	protected float _reachedDestinationTimestamp;

	protected bool _destinationReached;

	public TimescaleModes TimescaleMode { get; set; }

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
		_virtualCamera = base.gameObject.GetComponent<CinemachineVirtualCamera>();
		_initialFieldOfView = _virtualCamera.m_Lens.FieldOfView;
	}

	protected virtual void Update()
	{
		if (!_zooming)
		{
			return;
		}
		if (_virtualCamera.m_Lens.FieldOfView != _targetFieldOfView)
		{
			_delta += GetDeltaTime() / _transitionDuration;
			_virtualCamera.m_Lens.FieldOfView = Mathf.LerpUnclamped(_startFieldOfView, _targetFieldOfView, ZoomCurve.Evaluate(_delta));
			return;
		}
		if (!_destinationReached)
		{
			_reachedDestinationTimestamp = GetTime();
			_destinationReached = true;
		}
		if (_mode == MMCameraZoomModes.For && _direction == 1)
		{
			if (GetTime() - _reachedDestinationTimestamp > _duration)
			{
				_direction = -1;
				_startFieldOfView = _targetFieldOfView;
				_targetFieldOfView = _initialFieldOfView;
				_delta = 0f;
			}
		}
		else
		{
			_zooming = false;
		}
	}

	public virtual void Zoom(MMCameraZoomModes mode, float newFieldOfView, float transitionDuration, float duration, bool useUnscaledTime, bool relative = false)
	{
		if (!_zooming)
		{
			_zooming = true;
			_delta = 0f;
			_mode = mode;
			TimescaleMode = (useUnscaledTime ? TimescaleModes.Unscaled : TimescaleModes.Scaled);
			_startFieldOfView = _virtualCamera.m_Lens.FieldOfView;
			_transitionDuration = transitionDuration;
			_duration = duration;
			_transitionDuration = transitionDuration;
			_direction = 1;
			_destinationReached = false;
			switch (mode)
			{
			case MMCameraZoomModes.For:
				_targetFieldOfView = newFieldOfView;
				break;
			case MMCameraZoomModes.Set:
				_targetFieldOfView = newFieldOfView;
				break;
			case MMCameraZoomModes.Reset:
				_targetFieldOfView = _initialFieldOfView;
				break;
			}
			if (relative)
			{
				_targetFieldOfView += _initialFieldOfView;
			}
		}
	}

	protected virtual void TestZoom()
	{
		Zoom(TestMode, TestFieldOfView, TestTransitionDuration, TestDuration, useUnscaledTime: false);
	}

	public virtual void OnCameraZoomEvent(MMCameraZoomModes mode, float newFieldOfView, float transitionDuration, float duration, int channel, bool useUnscaledTime, bool stop = false, bool relative = false)
	{
		if (channel == Channel || channel == -1 || Channel == -1)
		{
			if (stop)
			{
				_zooming = false;
			}
			else
			{
				Zoom(mode, newFieldOfView, transitionDuration, duration, useUnscaledTime, relative);
			}
		}
	}

	protected virtual void OnEnable()
	{
		MMCameraZoomEvent.Register(OnCameraZoomEvent);
	}

	protected virtual void OnDisable()
	{
		MMCameraZoomEvent.Unregister(OnCameraZoomEvent);
	}
}
