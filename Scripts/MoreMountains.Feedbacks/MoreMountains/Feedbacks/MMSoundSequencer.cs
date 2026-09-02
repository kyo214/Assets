using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MoreMountains.Feedbacks;

[AddComponentMenu("More Mountains/Feedbacks/Sequencing/MMSoundSequencer")]
public class MMSoundSequencer : MMSequencer
{
	[Tooltip("the list of audio clips to play (one per track)")]
	public List<AudioClip> Sounds;

	protected List<AudioSource> _audioSources;

	protected override void Initialization()
	{
		base.Initialization();
		_audioSources = new List<AudioSource>();
		foreach (AudioClip sound in Sounds)
		{
			GameObject obj = new GameObject();
			SceneManager.MoveGameObjectToScene(obj, base.gameObject.scene);
			obj.name = "AudioSource - " + sound.name;
			obj.transform.SetParent(base.transform);
			AudioSource audioSource = obj.AddComponent<AudioSource>();
			audioSource.loop = false;
			audioSource.playOnAwake = false;
			audioSource.clip = sound;
			audioSource.volume = 1f;
			audioSource.pitch = 1f;
			_audioSources.Add(audioSource);
		}
	}

	protected override void OnBeat()
	{
		base.OnBeat();
		for (int i = 0; i < Sequence.SequenceTracks.Count; i++)
		{
			if (Sequence.SequenceTracks[i].Active && Sequence.QuantizedSequence[i].Line[CurrentSequenceIndex].ID != -1 && _audioSources.Count > i && _audioSources[i] != null)
			{
				_audioSources[i].Play();
			}
		}
	}

	public override void PlayTrackEvent(int index)
	{
		if (Application.isPlaying)
		{
			base.PlayTrackEvent(index);
			_audioSources[index].Play();
		}
	}

	public override void EditorMaintenance()
	{
		base.EditorMaintenance();
		SetupSounds();
	}

	public virtual void SetupSounds()
	{
		if (Sequence == null)
		{
			return;
		}
		if (Sounds.Count < Sequence.SequenceTracks.Count)
		{
			for (int i = 0; i < Sequence.SequenceTracks.Count; i++)
			{
				if (i >= Sounds.Count)
				{
					Sounds.Add(null);
				}
			}
		}
		if (Sounds.Count > Sequence.SequenceTracks.Count)
		{
			Sounds.Clear();
			for (int j = 0; j < Sequence.SequenceTracks.Count; j++)
			{
				Sounds.Add(null);
			}
		}
	}
}
