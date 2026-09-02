using System;
using System.Collections.Generic;
using Doozy.Runtime.Mody;
using Doozy.Runtime.UIManager.Content.Internal;
using UnityEngine.Events;

namespace Doozy.Runtime.UIManager.Content;

public class UIStopwatch : DateTimeComponent
{
	[Serializable]
	public struct Lap
	{
		public int lapNumber { get; private set; }

		public TimeSpan lapTime { get; private set; }

		public TimeSpan lapDuration { get; private set; }

		public Lap(int lapNumber, TimeSpan lapTime, TimeSpan lapDuration)
		{
			this.lapNumber = lapNumber;
			this.lapTime = lapTime;
			this.lapDuration = lapDuration;
		}
	}

	public ModyEvent OnLap = new ModyEvent();

	public int currentLapIndex { get; protected set; }

	public int currentLapNumber => currentLapIndex + 1;

	public List<Lap> laps { get; protected set; }

	public UnityEvent onLapEvent => OnLap.Event;

	private TimeSpan previousLapTime { get; set; }

	protected override void Awake()
	{
		base.Awake();
		if (laps == null)
		{
			List<Lap> list = (laps = new List<Lap>());
		}
	}

	protected override void OnEnable()
	{
		base.OnEnable();
		StartTimer();
	}

	protected override void OnDisable()
	{
		base.OnDisable();
		CancelTimer();
	}

	protected override void UpdateCurrentTime()
	{
		base.UpdateCurrentTime();
		Years = base.elapsedTime.Days / 365;
		Months = base.elapsedTime.Days / 30;
		Days = base.elapsedTime.Days;
		Hours = base.elapsedTime.Hours;
		Minutes = base.elapsedTime.Minutes;
		Seconds = base.elapsedTime.Seconds;
		Milliseconds = base.elapsedTime.Milliseconds;
		UpdateLabels();
	}

	public override void UpdateLabels()
	{
		for (int i = 0; i < base.labels.Count; i++)
		{
			if (!(base.labels[i].Label == null))
			{
				base.labels[i].SetText(base.elapsedTime);
			}
		}
	}

	public override void ResetTimer()
	{
		base.ResetTimer();
		ClearLaps();
		UpdateLabels();
	}

	public override void StartTimer()
	{
		base.StartTimer();
		ClearLaps();
		UpdateLabels();
	}

	public override void StopTimer()
	{
		base.StopTimer();
		UpdateLabels();
	}

	public override void PauseTimer()
	{
		base.PauseTimer();
		UpdateLabels();
	}

	public override void ResumeTimer()
	{
		base.ResumeTimer();
		UpdateLabels();
	}

	public override void FinishTimer()
	{
		StartUpdateCoroutine();
		UpdateCurrentTime();
		StopTimer();
		OnFinish?.Execute();
		UpdateLabels();
	}

	public override void CancelTimer()
	{
		base.CancelTimer();
		UpdateLabels();
	}

	public void AddLap()
	{
		laps.Add(new Lap(currentLapNumber, base.elapsedTime, base.elapsedTime - previousLapTime));
		OnLap?.Execute();
		UpdateLabels();
		currentLapIndex++;
		previousLapTime = base.elapsedTime;
	}

	public Lap GetLap(int lapNumber)
	{
		if (lapNumber < 1 || lapNumber > currentLapNumber)
		{
			return default;
		}
		return laps[lapNumber - 1];
	}

	public void ClearLaps()
	{
		laps.Clear();
		currentLapIndex = 0;
		previousLapTime = TimeSpan.Zero;
	}

	protected override void SetEndTime()
	{
		base.endTime = base.startTime.AddYears(100);
	}
}
