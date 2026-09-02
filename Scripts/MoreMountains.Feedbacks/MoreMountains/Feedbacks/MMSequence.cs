using System.Collections.Generic;
using UnityEngine;

namespace MoreMountains.Feedbacks;

[CreateAssetMenu(menuName = "MoreMountains/Sequencer/MMSequence")]
public class MMSequence : ScriptableObject
{
	[Header("Sequence")]
	[Tooltip("the length (in seconds) of the sequence")]
	[MMFReadOnly]
	public float Length;

	[Tooltip("the original sequence (as outputted by the input sequence recorder)")]
	public MMSequenceList OriginalSequence;

	[Tooltip("the duration in seconds to apply after the last input")]
	public float EndSilenceDuration;

	[Header("Sequence Contents")]
	[Tooltip("the list of tracks for this sequence")]
	public List<MMSequenceTrack> SequenceTracks;

	[Header("Quantizing")]
	[Tooltip("whether this sequence should be used in quantized form or not")]
	public bool Quantized;

	[Tooltip("the target BPM for this sequence")]
	public int TargetBPM = 120;

	[Tooltip("the contents of the quantized sequence")]
	public List<MMSequenceList> QuantizedSequence;

	[Space]
	[Header("Controls")]
	[MMFInspectorButton("RandomizeTrackColors")]
	public bool RandomizeTrackColorsButton;

	protected float[] _quantizedBeats;

	protected List<MMSequenceNote> _deleteList;

	private static int SortByTimestamp(MMSequenceNote p1, MMSequenceNote p2)
	{
		return p1.Timestamp.CompareTo(p2.Timestamp);
	}

	public virtual void SortOriginalSequence()
	{
		OriginalSequence.Line.Sort(SortByTimestamp);
	}

	public virtual void QuantizeOriginalSequence()
	{
		ComputeLength();
		QuantizeSequenceToBPM(OriginalSequence.Line);
	}

	public virtual void ComputeLength()
	{
		Length = OriginalSequence.Line[OriginalSequence.Line.Count - 1].Timestamp + EndSilenceDuration;
	}

	public virtual void QuantizeSequenceToBPM(List<MMSequenceNote> baseSequence)
	{
		float length = Length;
		float num = 60f / (float)TargetBPM;
		int num2 = (int)(length / num);
		QuantizedSequence = new List<MMSequenceList>();
		_deleteList = new List<MMSequenceNote>();
		_deleteList.Clear();
		_quantizedBeats = new float[num2];
		for (int i = 0; i < num2; i++)
		{
			_quantizedBeats[i] = (float)i * num;
		}
		for (int j = 0; j < SequenceTracks.Count; j++)
		{
			QuantizedSequence.Add(new MMSequenceList());
			QuantizedSequence[j].Line = new List<MMSequenceNote>();
			for (int k = 0; k < num2; k++)
			{
				MMSequenceNote mMSequenceNote = new MMSequenceNote();
				mMSequenceNote.ID = -1;
				mMSequenceNote.Timestamp = _quantizedBeats[k];
				QuantizedSequence[j].Line.Add(mMSequenceNote);
				foreach (MMSequenceNote item in baseSequence)
				{
					if (RoundFloatToArray(item.Timestamp, _quantizedBeats) == _quantizedBeats[k] && item.ID == SequenceTracks[j].ID)
					{
						QuantizedSequence[j].Line[k].ID = item.ID;
					}
				}
			}
		}
	}

	protected virtual void OnValidate()
	{
		for (int i = 0; i < SequenceTracks.Count; i++)
		{
			SequenceTracks[i].SetDefaults(i);
		}
	}

	protected virtual void RandomizeTrackColors()
	{
		foreach (MMSequenceTrack sequenceTrack in SequenceTracks)
		{
			sequenceTrack.TrackColor = RandomSequenceColor();
		}
	}

	public static Color RandomSequenceColor()
	{
		return Random.Range(0, 32) switch
		{
			0 => new Color32(240, 248, byte.MaxValue, byte.MaxValue), 
			1 => new Color32(127, byte.MaxValue, 212, byte.MaxValue), 
			2 => new Color32(245, 245, 220, byte.MaxValue), 
			3 => new Color32(95, 158, 160, byte.MaxValue), 
			4 => new Color32(byte.MaxValue, 127, 80, byte.MaxValue), 
			5 => new Color32(0, byte.MaxValue, byte.MaxValue, byte.MaxValue), 
			6 => new Color32(byte.MaxValue, 215, 0, byte.MaxValue), 
			7 => new Color32(byte.MaxValue, 0, byte.MaxValue, byte.MaxValue), 
			8 => new Color32(50, 128, 120, byte.MaxValue), 
			9 => new Color32(173, byte.MaxValue, 47, byte.MaxValue), 
			10 => new Color32(byte.MaxValue, 105, 180, byte.MaxValue), 
			11 => new Color32(75, 0, 130, byte.MaxValue), 
			12 => new Color32(byte.MaxValue, byte.MaxValue, 240, byte.MaxValue), 
			13 => new Color32(124, 252, 0, byte.MaxValue), 
			14 => new Color32(byte.MaxValue, 160, 122, byte.MaxValue), 
			15 => new Color32(0, byte.MaxValue, 0, byte.MaxValue), 
			16 => new Color32(245, byte.MaxValue, 250, byte.MaxValue), 
			17 => new Color32(byte.MaxValue, 228, 225, byte.MaxValue), 
			18 => new Color32(218, 112, 214, byte.MaxValue), 
			19 => new Color32(byte.MaxValue, 192, 203, byte.MaxValue), 
			20 => new Color32(byte.MaxValue, 0, 0, byte.MaxValue), 
			21 => new Color32(196, 112, byte.MaxValue, byte.MaxValue), 
			22 => new Color32(250, 128, 114, byte.MaxValue), 
			23 => new Color32(46, 139, 87, byte.MaxValue), 
			24 => new Color32(192, 192, 192, byte.MaxValue), 
			25 => new Color32(135, 206, 235, byte.MaxValue), 
			26 => new Color32(0, byte.MaxValue, 127, byte.MaxValue), 
			27 => new Color32(210, 180, 140, byte.MaxValue), 
			28 => new Color32(0, 128, 128, byte.MaxValue), 
			29 => new Color32(byte.MaxValue, 99, 71, byte.MaxValue), 
			30 => new Color32(64, 224, 208, byte.MaxValue), 
			31 => new Color32(byte.MaxValue, byte.MaxValue, 0, byte.MaxValue), 
			32 => new Color32(154, 205, 50, byte.MaxValue), 
			_ => new Color32(240, 248, byte.MaxValue, byte.MaxValue), 
		};
	}

	public static float RoundFloatToArray(float value, float[] array)
	{
		int num = 0;
		if (array[num] >= value)
		{
			return array[num];
		}
		int num2 = array.Length - 1;
		if (array[num2] <= value)
		{
			return array[num2];
		}
		while (num2 - num > 1)
		{
			int num3 = (num2 + num) / 2;
			if (array[num3] == value)
			{
				return array[num3];
			}
			if (array[num3] < value)
			{
				num = num3;
			}
			else
			{
				num2 = num3;
			}
		}
		if (array[num2] - value <= value - array[num])
		{
			return array[num2];
		}
		return array[num];
	}
}
