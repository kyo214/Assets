using UnityEngine;

namespace MoreMountains.Feedbacks;

[AddComponentMenu("More Mountains/Feedbacks/Sequencing/MMInputSequenceRecorder")]
[ExecuteAlways]
public class MMInputSequenceRecorder : MonoBehaviour
{
	[Header("Target")]
	[Tooltip("the target scriptable object to write to")]
	public MMSequence SequenceScriptableObject;

	[Header("Recording")]
	[MMFReadOnly]
	[Tooltip("whether this recorder is recording right now or not")]
	public bool Recording;

	[Tooltip("whether any silence between the start of the recording and the first press should be removed or not")]
	public bool RemoveInitialSilence = true;

	[Tooltip("whether this recording should write on top of existing entries or not")]
	public bool AdditiveRecording;

	[Tooltip("whether this recorder should start recording when entering play mode")]
	public bool StartRecordingOnGameStart;

	[Tooltip("the offset to apply to entries")]
	public float RecordingStartOffset;

	[Header("Recorder Keys")]
	[Tooltip("the key binding for recording start")]
	public KeyCode StartRecordingHotkey = KeyCode.Home;

	[Tooltip("the key binding for recording stop")]
	public KeyCode StopRecordingHotkey = KeyCode.End;

	protected MMSequenceNote _note;

	protected float _recordingStartedAt;

	protected virtual void Awake()
	{
		Initialization();
	}

	public virtual void Initialization()
	{
		Recording = false;
		_note = new MMSequenceNote();
		if (SequenceScriptableObject == null)
		{
			Debug.LogError(base.name + " this input based sequencer needs a bound scriptable object to function, please create one and bind it in the inspector.");
		}
	}

	protected virtual void Start()
	{
		if (StartRecordingOnGameStart)
		{
			StartRecording();
		}
	}

	public virtual void StartRecording()
	{
		Recording = true;
		if (!AdditiveRecording)
		{
			SequenceScriptableObject.OriginalSequence.Line.Clear();
		}
		_recordingStartedAt = Time.realtimeSinceStartup;
	}

	public virtual void StopRecording()
	{
		Recording = false;
		SequenceScriptableObject.QuantizeOriginalSequence();
	}

	protected virtual void Update()
	{
		if (Application.isPlaying)
		{
			DetectStartAndEnd();
			DetectRecording();
		}
	}

	protected virtual void DetectStartAndEnd()
	{
	}

	protected virtual void DetectRecording()
	{
		if (!Recording || !(SequenceScriptableObject != null))
		{
			return;
		}
		foreach (MMSequenceTrack sequenceTrack in SequenceScriptableObject.SequenceTracks)
		{
			if (Input.GetKeyDown(sequenceTrack.Key))
			{
				AddNoteToTrack(sequenceTrack);
			}
		}
	}

	public virtual void AddNoteToTrack(MMSequenceTrack track)
	{
		if (SequenceScriptableObject.OriginalSequence.Line.Count == 0 && RemoveInitialSilence)
		{
			_recordingStartedAt = Time.realtimeSinceStartup;
		}
		_note = new MMSequenceNote();
		_note.ID = track.ID;
		_note.Timestamp = Time.realtimeSinceStartup + RecordingStartOffset - _recordingStartedAt;
		SequenceScriptableObject.OriginalSequence.Line.Add(_note);
	}
}
