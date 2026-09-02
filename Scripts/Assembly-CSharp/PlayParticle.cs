using System.Collections.Generic;
using UnityEngine;

public class PlayParticle : MonoBehaviour
{
	public List<ParticleSystem> particles;

	public List<ParticleSystem> otherParticles;

	public void PlayParticles()
	{
		for (int i = 0; i < particles.Count; i++)
		{
			particles[i].gameObject.SetActive(value: true);
			particles[i].Play();
		}
	}

	public void PlayOtherParticles()
	{
		for (int i = 0; i < otherParticles.Count; i++)
		{
			otherParticles[i].gameObject.SetActive(value: true);
			otherParticles[i].Play();
		}
	}

	public void StopParticles()
	{
		for (int i = 0; i < particles.Count; i++)
		{
			particles[i].Stop();
		}
	}

	public void StopOtherParticles()
	{
		for (int i = 0; i < otherParticles.Count; i++)
		{
			otherParticles[i].Stop();
		}
	}
}
