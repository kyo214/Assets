using UnityEngine;

namespace MoreMountains.Tools;

[AddComponentMenu("More Mountains/Tools/Animation/MMStopMotionAnimation")]
public class MMStopMotionAnimation : MonoBehaviour
{
	public enum FramerateModes
	{
		Manual = 0,
		Automatic = 1
	}

	[Header("General Settings")]
	public bool StopMotionEnabled = true;

	public int AnimationLayerID;

	[Header("Framerate")]
	public FramerateModes FramerateMode = FramerateModes.Automatic;

	[MMEnumCondition("FramerateMode", new int[] { 1 })]
	public float FramesPerSecond = 4f;

	[MMEnumCondition("FramerateMode", new int[] { 1 })]
	public float PollFrequency = 1f;

	[MMEnumCondition("FramerateMode", new int[] { 0 })]
	public float ManualTimeBetweenFrames = 0.125f;

	[MMEnumCondition("FramerateMode", new int[] { 0 })]
	public float ManualAnimatorSpeed = 2f;

	public float timet;

	protected float _currentClipFPS;

	protected float _currentClipLength;

	protected float _lastPollAt = -10f;

	protected Animator _animator;

	protected AnimationClip _currentClip;

	protected virtual void Awake()
	{
		_animator = base.gameObject.GetComponent<Animator>();
	}

	protected virtual void Update()
	{
		StopMotion();
		if (Time.time - _lastPollAt > PollFrequency)
		{
			Poll();
		}
	}

	protected virtual void StopMotion()
	{
		if (StopMotionEnabled)
		{
			float num = 0f;
			float speed = 0f;
			switch (FramerateMode)
			{
			case FramerateModes.Manual:
				num = ManualTimeBetweenFrames;
				speed = ManualAnimatorSpeed;
				break;
			case FramerateModes.Automatic:
				num = 1f / FramesPerSecond;
				speed = 1f / (FramesPerSecond - 1f) * 2f * _currentClipFPS;
				break;
			}
			timet += Time.deltaTime;
			if (timet > num)
			{
				timet -= num;
				_animator.speed = speed;
			}
			else
			{
				_animator.speed = 0f;
			}
		}
	}

	protected virtual void Poll()
	{
		_currentClip = _animator.GetCurrentAnimatorClipInfo(AnimationLayerID)[0].clip;
		_currentClipLength = _currentClip.length;
		_currentClipFPS = _currentClip.frameRate;
		_lastPollAt = Time.time;
	}
}
