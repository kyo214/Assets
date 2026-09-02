using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace MoreMountains.Feedbacks;

[AddComponentMenu("More Mountains/Feedbacks/Sequencing/MMSequencer")]
public class MMSequencer : MonoBehaviour
{
	[Header("Sequence")]
	[Tooltip("the sequence to design on or to play")]
	public MMSequence Sequence;

	[Tooltip("the intended BPM for playback and design")]
	public int BPM = 160;

	[Tooltip("the number of notes in the sequence")]
	public int SequencerLength = 8;

	[Header("Playback")]
	[Tooltip("whether the sequence should loop or not when played back")]
	public bool Loop = true;

	[Tooltip("if this is true the sequence will play in random order")]
	public bool RandomSequence;

	[Tooltip("whether that sequencer should start playing on application start")]
	public bool PlayOnStart;

	[Header("Metronome")]
	[Tooltip("a sound to play every beat")]
	public AudioClip MetronomeSound;

	[Tooltip("the volume of the metronome sound")]
	[Range(0f, 1f)]
	public float MetronomeVolume = 0.2f;

	[Header("Events")]
	[Tooltip("a list of events to play every time an active beat is found on each track (one event per track)")]
	public List<UnityEvent> TrackEvents;

	[Header("Monitor")]
	[Tooltip("true if the sequencer is playing right now")]
	[MMFReadOnly]
	public bool Playing;

	[Tooltip("true if the sequencer has been played once")]
	[HideInInspector]
	public bool PlayedOnce;

	[Tooltip("true if a perfect beat was found this frame")]
	[MMFReadOnly]
	public bool BeatThisFrame;

	[Tooltip("the index of the last played bit (our position in the playing sequence)")]
	[MMFReadOnly]
	public int LastBeatIndex;

	[HideInInspector]
	public int LastBPM = -1;

	[HideInInspector]
	public int LastTracksCount = -1;

	[HideInInspector]
	public int LastSequencerLength = -1;

	[HideInInspector]
	public MMSequence LastSequence;

	[HideInInspector]
	public int CurrentSequenceIndex;

	[HideInInspector]
	public float LastBeatTimestamp;

	protected float _beatInterval;

	protected AudioSource _beatSoundAudiosource;

	protected virtual void Start()
	{
		Initialization();
	}

	protected virtual void Initialization()
	{
		Playing = false;
		if (MetronomeSound != null)
		{
			GameObject gameObject = new GameObject();
			SceneManager.MoveGameObjectToScene(gameObject, base.gameObject.scene);
			gameObject.name = "BeatSoundAudioSource";
			gameObject.transform.SetParent(base.transform);
			_beatSoundAudiosource = gameObject.AddComponent<AudioSource>();
			_beatSoundAudiosource.clip = MetronomeSound;
			_beatSoundAudiosource.loop = false;
			_beatSoundAudiosource.playOnAwake = false;
		}
		if (PlayOnStart)
		{
			PlaySequence();
		}
	}

	public virtual void ToggleSequence()
	{
		if (Playing)
		{
			StopSequence();
		}
		else
		{
			PlaySequence();
		}
	}

	public virtual void PlaySequence()
	{
		CurrentSequenceIndex = 0;
		Playing = true;
		LastBeatTimestamp = 0f;
	}

	public virtual void StopSequence()
	{
		Playing = false;
	}

	public virtual void ClearSequence()
	{
		for (int i = 0; i < Sequence.SequenceTracks.Count; i++)
		{
			for (int j = 0; j < SequencerLength; j++)
			{
				Sequence.QuantizedSequence[i].Line[j].ID = -1;
			}
		}
	}

	protected virtual void Update()
	{
		HandleBeat();
	}

	protected virtual void HandleBeat()
	{
		BeatThisFrame = false;
		if (!Playing)
		{
			return;
		}
		if (CurrentSequenceIndex >= SequencerLength)
		{
			StopSequence();
			return;
		}
		_beatInterval = 60f / (float)BPM;
		if (Time.time - LastBeatTimestamp >= _beatInterval || LastBeatTimestamp == 0f)
		{
			PlayBeat();
		}
	}

	public virtual void PlayBeat()
	{
		BeatThisFrame = true;
		LastBeatIndex = CurrentSequenceIndex;
		LastBeatTimestamp = Time.time;
		PlayedOnce = true;
		PlayMetronomeSound();
		OnBeat();
		for (int i = 0; i < Sequence.SequenceTracks.Count; i++)
		{
			if (Sequence.SequenceTracks[i].Active && Sequence.QuantizedSequence[i].Line[CurrentSequenceIndex].ID != -1 && TrackEvents[i] != null)
			{
				TrackEvents[i].Invoke();
			}
		}
		CurrentSequenceIndex++;
		if (CurrentSequenceIndex >= SequencerLength && Loop)
		{
			CurrentSequenceIndex = 0;
		}
		if (RandomSequence)
		{
			CurrentSequenceIndex = Random.Range(0, SequencerLength);
		}
	}

	protected virtual void OnBeat()
	{
	}

	public virtual void PlayTrackEvent(int index)
	{
		TrackEvents[index].Invoke();
	}

	public virtual void ToggleActive(int trackIndex)
	{
		Sequence.SequenceTracks[trackIndex].Active = !Sequence.SequenceTracks[trackIndex].Active;
	}

	public virtual void ToggleStep(int stepIndex)
	{
		bool flag = Sequence.QuantizedSequence[0].Line[stepIndex].ID != -1;
		for (int i = 0; i < Sequence.SequenceTracks.Count; i++)
		{
			if (flag)
			{
				Sequence.QuantizedSequence[i].Line[stepIndex].ID = -1;
			}
			else
			{
				Sequence.QuantizedSequence[i].Line[stepIndex].ID = Sequence.SequenceTracks[i].ID;
			}
		}
	}

	protected virtual void PlayMetronomeSound()
	{
		if (MetronomeSound != null)
		{
			_beatSoundAudiosource.volume = MetronomeVolume;
			_beatSoundAudiosource.Play();
		}
	}

	public virtual void IncrementLength()
	{
		if (!(Sequence == null))
		{
			float num = 60f / (float)BPM;
			SequencerLength++;
			Sequence.Length = (float)SequencerLength * num;
			LastSequencerLength = SequencerLength;
			for (int i = 0; i < Sequence.SequenceTracks.Count; i++)
			{
				MMSequenceNote mMSequenceNote = new MMSequenceNote();
				mMSequenceNote.ID = -1;
				mMSequenceNote.Timestamp = (float)Sequence.QuantizedSequence[i].Line.Count * num;
				Sequence.QuantizedSequence[i].Line.Add(mMSequenceNote);
			}
		}
	}

	public virtual void DecrementLength()
	{
		if (!(Sequence == null))
		{
			float num = 60f / (float)BPM;
			SequencerLength--;
			Sequence.Length = (float)SequencerLength * num;
			LastSequencerLength = SequencerLength;
			for (int i = 0; i < Sequence.SequenceTracks.Count; i++)
			{
				int index = Sequence.QuantizedSequence[i].Line.Count - 1;
				Sequence.QuantizedSequence[i].Line.RemoveAt(index);
			}
		}
	}

	public virtual void UpdateTimestampsToMatchNewBPM()
	{
		if (Sequence == null)
		{
			return;
		}
		float num = 60f / (float)BPM;
		Sequence.TargetBPM = BPM;
		Sequence.Length = (float)SequencerLength * num;
		Sequence.EndSilenceDuration = num;
		Sequence.Quantized = true;
		for (int i = 0; i < Sequence.SequenceTracks.Count; i++)
		{
			for (int j = 0; j < SequencerLength; j++)
			{
				Sequence.QuantizedSequence[i].Line[j].Timestamp = (float)j * num;
			}
		}
		LastBPM = BPM;
	}

	public virtual void ApplySequencerLengthToSequence()
	{
		if (Sequence == null)
		{
			return;
		}
		float num = 60f / (float)BPM;
		Sequence.TargetBPM = BPM;
		Sequence.Length = (float)SequencerLength * num;
		Sequence.EndSilenceDuration = num;
		Sequence.Quantized = true;
		if (LastSequencerLength != SequencerLength || LastTracksCount != Sequence.SequenceTracks.Count)
		{
			Sequence.QuantizedSequence = new List<MMSequenceList>();
			for (int i = 0; i < Sequence.SequenceTracks.Count; i++)
			{
				Sequence.QuantizedSequence.Add(new MMSequenceList());
				Sequence.QuantizedSequence[i].Line = new List<MMSequenceNote>();
				for (int j = 0; j < SequencerLength; j++)
				{
					MMSequenceNote mMSequenceNote = new MMSequenceNote();
					mMSequenceNote.ID = -1;
					mMSequenceNote.Timestamp = (float)j * num;
					Sequence.QuantizedSequence[i].Line.Add(mMSequenceNote);
				}
			}
		}
		LastTracksCount = Sequence.SequenceTracks.Count;
		LastSequencerLength = SequencerLength;
	}

	public virtual void EditorMaintenance()
	{
		SetupTrackEvents();
	}

	public virtual void SetupTrackEvents()
	{
		if (Sequence == null)
		{
			return;
		}
		if (TrackEvents.Count < Sequence.SequenceTracks.Count)
		{
			for (int i = 0; i < Sequence.SequenceTracks.Count; i++)
			{
				if (i >= TrackEvents.Count)
				{
					TrackEvents.Add(new UnityEvent());
				}
			}
		}
		if (TrackEvents.Count > Sequence.SequenceTracks.Count)
		{
			TrackEvents.Clear();
			for (int j = 0; j < Sequence.SequenceTracks.Count; j++)
			{
				TrackEvents.Add(new UnityEvent());
			}
		}
	}
}
