using System;
using UnityEngine;

namespace MoreMountains.Tools;

public class MMPositionRecorder : MonoBehaviour
{
	public enum Modes
	{
		Framecount = 0,
		Time = 1
	}

	[Header("Recording Settings")]
	public int NumberOfPositionsToRecord = 100;

	public Modes Mode;

	[MMEnumCondition("Mode", new int[] { 0 })]
	public int FrameInterval;

	[MMEnumCondition("Mode", new int[] { 1 })]
	public float TimeInterval = 0.02f;

	public bool RecordOnTimescaleZero;

	[Header("Debug")]
	public Vector3[] Positions;

	[MMReadOnly]
	public int FrameCounter;

	protected int _frameCountLastRecord;

	protected float _timeLastRecord;

	protected virtual void Awake()
	{
		Positions = new Vector3[NumberOfPositionsToRecord];
		for (int i = 0; i < Positions.Length; i++)
		{
			Positions[i] = base.transform.position;
		}
	}

	protected virtual void Update()
	{
		if (RecordOnTimescaleZero || Time.timeScale != 0f)
		{
			StorePositions();
		}
	}

	protected virtual void StorePositions()
	{
		FrameCounter = Time.frameCount;
		if (Mode == Modes.Framecount)
		{
			if (FrameCounter - _frameCountLastRecord < FrameInterval)
			{
				return;
			}
			_frameCountLastRecord = FrameCounter;
		}
		else
		{
			if (Time.time - _timeLastRecord < TimeInterval)
			{
				return;
			}
			_timeLastRecord = Time.time;
		}
		Positions[0] = base.transform.position;
		Array.Copy(Positions, 0, Positions, 1, Positions.Length - 1);
	}
}
