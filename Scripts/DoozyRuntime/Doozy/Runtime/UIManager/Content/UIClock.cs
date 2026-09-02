using System;
using Doozy.Runtime.UIManager.Content.Internal;
using UnityEngine;

namespace Doozy.Runtime.UIManager.Content;

public class UIClock : DateTimeComponent
{
	[SerializeField]
	private string TimeZoneId;

	private TimeZoneInfo m_TimeZoneInfo = TimeZoneInfo.Local;

	public string timeZoneId
	{
		get
		{
			return TimeZoneId;
		}
		set
		{
			if (!timeZoneInfo.Id.Equals(value))
			{
				timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById(value);
				TimeZoneId = value;
				TimeZoneChanged();
			}
		}
	}

	public TimeZoneInfo timeZoneInfo
	{
		get
		{
			return m_TimeZoneInfo;
		}
		set
		{
			m_TimeZoneInfo = value;
			TimeZoneId = value.Id;
			TimeZoneChanged();
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
		StopTimer();
	}

	public override void StartTimer()
	{
		base.StartTimer();
		UpdateLabels();
	}

	public override void StopTimer()
	{
		base.StopTimer();
		UpdateLabels();
	}

	public override void ResetTimer()
	{
		base.ResetTimer();
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

	public override void CancelTimer()
	{
		base.CancelTimer();
		UpdateLabels();
	}

	protected override void SetStartTime()
	{
		base.startTime = TimeZoneInfo.ConvertTimeFromUtc(GetDateTimeUtcNow(), timeZoneInfo);
		UpdateLastTime();
	}

	protected override void SetEndTime()
	{
		base.endTime = base.startTime.AddYears(100);
	}

	public void TimeZoneChanged()
	{
		SetStartTime();
		SetEndTime();
		UpdateCurrentTime();
	}

	protected override void UpdateCurrentTime()
	{
		base.currentTime = TimeZoneInfo.ConvertTimeFromUtc(GetDateTimeUtcNow(), timeZoneInfo);
		Years = base.currentTime.Year;
		Months = base.currentTime.Month;
		Days = base.currentTime.Day;
		Hours = base.currentTime.Hour;
		Minutes = base.currentTime.Minute;
		Seconds = base.currentTime.Second;
		Milliseconds = base.currentTime.Millisecond;
		UpdateLabels();
	}

	public virtual DateTime GetDateTimeUtcNow()
	{
		return DateTime.UtcNow;
	}

	public void SetTimeZone(string zoneId)
	{
		timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById(zoneId);
		TimeZoneId = timeZoneInfo.Id;
		TimeZoneChanged();
	}

	public int GetTimeZoneUtcOffset()
	{
		return timeZoneInfo.BaseUtcOffset.Hours;
	}

	public void SetTimeZone(TimeZoneInfo zoneInfo)
	{
		timeZoneInfo = zoneInfo;
		TimeZoneId = timeZoneInfo.Id;
		TimeZoneChanged();
	}

	public void SetTimeZoneByUtcOffset(int utcOffset)
	{
		string text = "UTC" + ((utcOffset >= 0) ? "+" : "") + utcOffset;
		timeZoneInfo = TimeZoneInfo.CreateCustomTimeZone(text, TimeSpan.FromHours(utcOffset), text, text);
		TimeZoneId = timeZoneInfo.Id;
		TimeZoneChanged();
	}

	public void SetLocalTimeZone()
	{
		SetTimeZone(TimeZoneInfo.Local);
	}

	public void SetUtcTimeZone()
	{
		SetTimeZone(TimeZoneInfo.Utc);
	}
}
