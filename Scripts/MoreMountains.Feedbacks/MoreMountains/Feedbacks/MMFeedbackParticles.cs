using System.Collections.Generic;
using UnityEngine;

namespace MoreMountains.Feedbacks;

[AddComponentMenu("")]
[FeedbackHelp("This feedback will simply play the specified ParticleSystem (from your scene) when played.")]
[FeedbackPath("Particles/Particles Play")]
public class MMFeedbackParticles : MMFeedback
{
	public enum Modes
	{
		Play = 0,
		Stop = 1,
		Pause = 2
	}

	public static bool FeedbackTypeAuthorized = true;

	[Header("Bound Particles")]
	[Tooltip("whether to Play, Stop or Pause the target particle system when that feedback is played")]
	public Modes Mode;

	[Tooltip("the particle system to play with this feedback")]
	public ParticleSystem BoundParticleSystem;

	[Tooltip("a list of (optional) particle systems")]
	public List<ParticleSystem> RandomParticleSystems;

	[Tooltip("if this is true, the particles will be moved to the position passed in parameters")]
	public bool MoveToPosition;

	[Tooltip("if this is true, the particle system's object will be set active on play")]
	public bool ActivateOnPlay;

	protected override void CustomInitialization(GameObject owner)
	{
		base.CustomInitialization(owner);
		StopParticles();
	}

	protected override void CustomPlayFeedback(Vector3 position, float feedbacksIntensity = 1f)
	{
		if (Active && FeedbackTypeAuthorized)
		{
			PlayParticles(position);
		}
	}

	protected override void CustomStopFeedback(Vector3 position, float feedbacksIntensity = 1f)
	{
		if (Active && FeedbackTypeAuthorized)
		{
			StopParticles();
		}
	}

	protected override void CustomReset()
	{
		base.CustomReset();
		if (!InCooldown)
		{
			StopParticles();
		}
	}

	protected virtual void PlayParticles(Vector3 position)
	{
		if (MoveToPosition)
		{
			BoundParticleSystem.transform.position = position;
			foreach (ParticleSystem randomParticleSystem in RandomParticleSystems)
			{
				randomParticleSystem.transform.position = position;
			}
		}
		if (ActivateOnPlay)
		{
			BoundParticleSystem.gameObject.SetActive(value: true);
			foreach (ParticleSystem randomParticleSystem2 in RandomParticleSystems)
			{
				randomParticleSystem2.gameObject.SetActive(value: true);
			}
		}
		if (RandomParticleSystems.Count > 0)
		{
			int index = Random.Range(0, RandomParticleSystems.Count);
			switch (Mode)
			{
			case Modes.Play:
				RandomParticleSystems[index].Play();
				break;
			case Modes.Stop:
				RandomParticleSystems[index].Stop();
				break;
			case Modes.Pause:
				RandomParticleSystems[index].Pause();
				break;
			}
		}
		else if (BoundParticleSystem != null)
		{
			switch (Mode)
			{
			case Modes.Play:
				BoundParticleSystem?.Play();
				break;
			case Modes.Stop:
				BoundParticleSystem?.Stop();
				break;
			case Modes.Pause:
				BoundParticleSystem?.Pause();
				break;
			}
		}
	}

	protected virtual void StopParticles()
	{
		foreach (ParticleSystem randomParticleSystem in RandomParticleSystems)
		{
			randomParticleSystem?.Stop();
		}
		if (BoundParticleSystem != null)
		{
			BoundParticleSystem.Stop();
		}
	}
}
