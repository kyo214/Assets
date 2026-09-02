using System;
using UnityEngine;

namespace MoreMountains.Tools;

[Serializable]
public class MMPlaylistSong
{
	public AudioSource TargetAudioSource;

	[MMVector(new string[] { "Min", "Max" })]
	public Vector2 Volume = new Vector2(0f, 1f);

	[MMVector(new string[] { "RMin", "RMax" })]
	public Vector2 InitialDelay = Vector2.zero;

	[MMVector(new string[] { "RMin", "RMax" })]
	public Vector2 CrossFadeDuration = new Vector2(2f, 2f);

	[MMVector(new string[] { "RMin", "RMax" })]
	public Vector2 Pitch = Vector2.one;

	[Range(-1f, 1f)]
	public float StereoPan;

	[Range(0f, 1f)]
	public float SpatialBlend;

	public bool Loop;

	[MMReadOnly]
	public bool Playing;

	[MMReadOnly]
	public bool Fading;

	public virtual void Initialization()
	{
		Volume = new Vector2(0f, 1f);
		InitialDelay = Vector2.zero;
		CrossFadeDuration = new Vector2(2f, 2f);
		Pitch = Vector2.one;
		StereoPan = 0f;
		SpatialBlend = 0f;
		Loop = false;
	}
}
