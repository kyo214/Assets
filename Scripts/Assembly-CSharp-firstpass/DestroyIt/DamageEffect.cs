using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace DestroyIt;

[Serializable]
public class DamageEffect
{
	public int TriggeredAt;

	public Vector3 Offset;

	public Vector3 Rotation;

	[FormerlySerializedAs("Effect")]
	public GameObject Prefab;

	public GameObject GameObject;

	public bool HasStarted;

	public bool HasTagDependency;

	public Tag TagDependency;

	public bool UnparentOnDestroy = true;

	public bool StopEmittingOnDestroy;

	public ParticleSystem[] ParticleSystems { get; set; }
}
