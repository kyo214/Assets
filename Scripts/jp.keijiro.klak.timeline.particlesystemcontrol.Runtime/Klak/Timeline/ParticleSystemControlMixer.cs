using System;
using UnityEngine;
using UnityEngine.Playables;

namespace Klak.Timeline;

[Serializable]
public class ParticleSystemControlMixer : PlayableBehaviour
{
	public ExposedReference<Transform> snapTarget;

	public uint randomSeed = uint.MaxValue;

	private Transform _snapTarget;

	private bool _needRestart;

	public ParticleSystem particleSystem { get; set; }

	private void PrepareParticleSystem(Playable playable)
	{
		if (particleSystem.useAutoRandomSeed)
		{
			particleSystem.useAutoRandomSeed = false;
		}
		if (particleSystem.randomSeed != randomSeed)
		{
			particleSystem.randomSeed = randomSeed;
		}
		float num = (float)playable.GetGraph().GetRootPlayable(0).GetDuration();
		ParticleSystem.MainModule main = particleSystem.main;
		if (main.duration < num)
		{
			main.duration = num;
		}
	}

	private void ResetSimulation(float time)
	{
		if (time < 2f / 3f)
		{
			particleSystem.Simulate(time);
			return;
		}
		particleSystem.Simulate(time - 2f / 3f, withChildren: true, restart: true, fixedTimeStep: false);
		particleSystem.Simulate(2f / 3f, withChildren: true, restart: false, fixedTimeStep: true);
	}

	public override void OnGraphStart(Playable playable)
	{
		if (!(particleSystem == null) && Application.isPlaying)
		{
			particleSystem.Stop();
			PrepareParticleSystem(playable);
		}
	}

	public override void ProcessFrame(Playable playable, FrameData info, object playerData)
	{
		if (particleSystem == null)
		{
			return;
		}
		if (!particleSystem.gameObject.activeInHierarchy)
		{
			_needRestart = true;
			return;
		}
		float num = (float)playable.GetGraph().GetRootPlayable(0).GetTime();
		if (!Application.isPlaying && !particleSystem.isPlaying)
		{
			PrepareParticleSystem(playable);
		}
		if (_snapTarget == null || !Application.isPlaying)
		{
			_snapTarget = snapTarget.Resolve(playable.GetGraph().GetResolver());
			if (_snapTarget == null)
			{
				_snapTarget = particleSystem.transform;
			}
		}
		if (_snapTarget != particleSystem.transform)
		{
			particleSystem.transform.position = _snapTarget.position;
			particleSystem.transform.rotation = _snapTarget.rotation;
		}
		float num2 = 0f;
		float num3 = 0f;
		int inputCount = playable.GetInputCount();
		for (int i = 0; i < inputCount; i++)
		{
			ParticleSystemControlPlayable behaviour = ((ScriptPlayable<ParticleSystemControlPlayable>)playable.GetInput(i)).GetBehaviour();
			float inputWeight = playable.GetInputWeight(i);
			num2 += behaviour.rateOverTime * inputWeight;
			num3 += behaviour.rateOverDistance * inputWeight;
		}
		ParticleSystem.EmissionModule emission = particleSystem.emission;
		emission.rateOverTimeMultiplier = num2;
		emission.rateOverDistanceMultiplier = num3;
		if (Application.isPlaying)
		{
			float num4 = Mathf.Max(1f / 30f, Time.smoothDeltaTime * 2f);
			if (Mathf.Abs(num - particleSystem.time) > num4)
			{
				ResetSimulation(num);
				particleSystem.Play();
			}
			return;
		}
		float num5 = 0.004166667f;
		float num6 = Mathf.Max(0.1f, Time.fixedDeltaTime * 2f);
		float num7 = 0.2f;
		if (_needRestart)
		{
			particleSystem.Play();
			_needRestart = false;
		}
		if (num < particleSystem.time || num > particleSystem.time + num7)
		{
			ResetSimulation(num);
		}
		else if (num > particleSystem.time + num6)
		{
			particleSystem.Simulate(num - particleSystem.time, withChildren: true, restart: false, fixedTimeStep: true);
		}
		else if (num > particleSystem.time + num5)
		{
			particleSystem.Simulate(num - particleSystem.time, withChildren: true, restart: false, fixedTimeStep: false);
		}
	}
}
