using UnityEngine;

namespace MoreMountains.Tools;

[RequireComponent(typeof(ParticleSystem))]
public class MMRuntimeParticleControl : MonoBehaviour
{
	public enum TrackerModes
	{
		Basic = 0,
		ForcedBounds = 1
	}

	[Header("Base Controls")]
	[MMInspectorButton("Play")]
	public bool PlayButton;

	[MMInspectorButton("Pause")]
	public bool PauseButton;

	[MMInspectorButton("Stop")]
	public bool StopButton;

	[Header("Simulate")]
	public float TargetTimestamp = 1f;

	[MMInspectorButton("Simulate")]
	public bool FastForwardToTimeButton;

	[Header("Tracker")]
	public TrackerModes TrackerMode;

	[MMEnumCondition("TrackerMode", new int[] { 1 })]
	public float MinBound;

	[MMEnumCondition("TrackerMode", new int[] { 1 })]
	public float MaxBound;

	[Range(0f, 1f)]
	public float Tracker;

	[MMReadOnly]
	public float Timestamp;

	protected ParticleSystem _particleSystem;

	protected ParticleSystem.MainModule _mainModule;

	protected virtual void Awake()
	{
		_particleSystem = GetComponent<ParticleSystem>();
		_mainModule = _particleSystem.main;
	}

	protected virtual void Play()
	{
		_particleSystem.Play();
	}

	protected virtual void Pause()
	{
		_particleSystem.Pause();
	}

	protected virtual void Stop()
	{
		_particleSystem.Stop();
	}

	protected virtual void Simulate()
	{
		_particleSystem.Simulate(TargetTimestamp, withChildren: true, restart: true);
	}

	protected void OnValidate()
	{
		float c = ((TrackerMode == TrackerModes.Basic) ? 0f : MinBound);
		float d = ((TrackerMode == TrackerModes.Basic) ? _mainModule.duration : MaxBound);
		Timestamp = MMMaths.Remap(Tracker, 0f, 1f, c, d);
		_particleSystem.Simulate(Timestamp, withChildren: true, restart: true);
	}
}
