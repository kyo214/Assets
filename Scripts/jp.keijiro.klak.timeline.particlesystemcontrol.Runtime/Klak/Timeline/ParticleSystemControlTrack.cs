using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace Klak.Timeline;

[TrackColor(0.4f, 0.7f, 0.6f)]
[TrackClipType(typeof(ParticleSystemControlClip))]
[TrackBindingType(typeof(ParticleSystem))]
public class ParticleSystemControlTrack : TrackAsset
{
	public ParticleSystemControlMixer template = new ParticleSystemControlMixer();

	public void OnEnable()
	{
		if (template.randomSeed == uint.MaxValue)
		{
			template.randomSeed = (uint)Random.Range(0, int.MaxValue);
		}
	}

	public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
	{
		ParticleSystem particleSystem = go.GetComponent<PlayableDirector>().GetGenericBinding(this) as ParticleSystem;
		ScriptPlayable<ParticleSystemControlMixer> scriptPlayable = ScriptPlayable<ParticleSystemControlMixer>.Create(graph, template, inputCount);
		scriptPlayable.GetBehaviour().particleSystem = particleSystem;
		return scriptPlayable;
	}

	public override void GatherProperties(PlayableDirector director, IPropertyCollector driver)
	{
		ParticleSystem particleSystem = director.GetGenericBinding(this) as ParticleSystem;
		if (!(particleSystem == null))
		{
			GameObject gameObject = particleSystem.gameObject;
			driver.AddFromName<Transform>(gameObject, "m_LocalPosition");
			driver.AddFromName<Transform>(gameObject, "m_LocalRotation");
			driver.AddFromName<ParticleSystem>(gameObject, "lengthInSec");
			driver.AddFromName<ParticleSystem>(gameObject, "autoRandomSeed");
			driver.AddFromName<ParticleSystem>(gameObject, "randomSeed");
			driver.AddFromName<ParticleSystem>(gameObject, "EmissionModule.rateOverTime.scalar");
			driver.AddFromName<ParticleSystem>(gameObject, "EmissionModule.rateOverDistance.scalar");
		}
	}
}
