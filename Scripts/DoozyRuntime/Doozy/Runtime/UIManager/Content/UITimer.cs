using System;
using System.Collections;
using Doozy.Runtime.Common;
using Doozy.Runtime.Common.Events;
using Doozy.Runtime.Common.Extensions;
using Doozy.Runtime.Mody;
using Doozy.Runtime.Reactor;
using Doozy.Runtime.UIManager.Content.Internal;
using UnityEngine;
using UnityEngine.Events;

namespace Doozy.Runtime.UIManager.Content;

[AddComponentMenu("UI/DateTime/UI Timer")]
public class UITimer : DateTimeComponent
{
	public ModyEvent OnLoop = new ModyEvent();

	public FloatEvent OnProgressChanged = new FloatEvent();

	[SerializeField]
	private Progressor TargetProgressor;

	public bool InstantProgressorUpdate = true;

	[SerializeField]
	private int Loops;

	[SerializeField]
	private float LoopDelay;

	public UnityEvent onLoopEvent => OnLoop.Event;

	public Progressor targetProgressor
	{
		get
		{
			return TargetProgressor;
		}
		set
		{
			TargetProgressor = value;
		}
	}

	public int loops
	{
		get
		{
			return Loops;
		}
		set
		{
			Loops = Mathf.Max(-1, value);
		}
	}

	public float loopDelay
	{
		get
		{
			return LoopDelay;
		}
		set
		{
			LoopDelay = Mathf.Max(0f, value);
		}
	}

	public bool inLoopDelay { get; protected set; }

	public DateTime loopDelayEndTime { get; protected set; }

	public float progress
	{
		get
		{
			if (!base.isFinished)
			{
				return ((float)base.elapsedTime.TotalMilliseconds / (float)base.endTime.Subtract(base.startTime).TotalMilliseconds).Round(4);
			}
			return 1f;
		}
	}

	protected override void Awake()
	{
		base.Awake();
		inLoopDelay = false;
	}

	protected override void UpdateCurrentTime()
	{
		base.UpdateCurrentTime();
		if (!inLoopDelay)
		{
			UpdateProgress(progress);
			UpdateLabels();
		}
	}

	public override void UpdateLabels()
	{
		for (int i = 0; i < base.labels.Count; i++)
		{
			if (!(base.labels[i].Label == null))
			{
				base.labels[i].SetText(base.remainingTime);
			}
		}
	}

	public override void ResetTimer()
	{
		base.ResetTimer();
		inLoopDelay = false;
		UpdateProgress(0f);
		UpdateLabels();
	}

	public override void StartTimer()
	{
		inLoopDelay = false;
		base.StartTimer();
		UpdateProgress(0f);
		UpdateLabels();
	}

	public override void StopTimer()
	{
		inLoopDelay = false;
		base.StopTimer();
		UpdateProgress(progress);
		UpdateLabels();
	}

	public override void PauseTimer()
	{
		base.PauseTimer();
		UpdateProgress(progress);
		UpdateLabels();
	}

	public override void ResumeTimer()
	{
		base.ResumeTimer();
		UpdateProgress(progress);
		UpdateLabels();
	}

	public override void FinishTimer()
	{
		base.FinishTimer();
		UpdateProgress(1f);
		UpdateLabels();
	}

	public override void CancelTimer()
	{
		inLoopDelay = false;
		base.CancelTimer();
		UpdateLabels();
	}

	private void UpdateProgress(float newProgress)
	{
		OnProgressChanged?.Invoke(newProgress);
		if (!(targetProgressor == null))
		{
			if (InstantProgressorUpdate)
			{
				targetProgressor.SetProgressAt(newProgress);
			}
			else
			{
				targetProgressor.PlayToProgress(newProgress);
			}
		}
	}

	protected override IEnumerator TimeUpdateCoroutine()
	{
		if (base.waitRealtime == null)
		{
			base.waitRealtime = new WaitForSecondsRealtime(UpdateInterval);
		}
		if (base.wait == null)
		{
			base.wait = new WaitForSeconds(UpdateInterval);
		}
		base.previousUpdateInterval = UpdateInterval;
		int loopCount = loops;
		while (base.isRunning)
		{
			if (base.isPaused)
			{
				yield return null;
				base.lastTime = Time.timeAsDouble;
				base.lastUnscaledTime = (float)Time.realtimeSinceStartupAsDouble;
				continue;
			}
			if (Math.Abs(base.previousUpdateInterval - UpdateInterval) > 0.001f)
			{
				base.waitRealtime = new WaitForSecondsRealtime(UpdateInterval);
				base.wait = new WaitForSeconds(UpdateInterval);
				base.previousUpdateInterval = UpdateInterval;
			}
			switch (TimescaleMode)
			{
			case Timescale.Independent:
				yield return base.waitRealtime;
				break;
			case Timescale.Dependent:
				yield return base.wait;
				break;
			default:
				throw new ArgumentOutOfRangeException();
			}
			UpdateCurrentTime();
			if (!inLoopDelay)
			{
				OnUpdate.Execute();
			}
			if (base.currentTime < base.endTime)
			{
				continue;
			}
			if ((loops < 0) | (loops > 0 && loopCount > 0))
			{
				if (loopDelay > 0f)
				{
					if (!inLoopDelay)
					{
						loopDelayEndTime = base.endTime.AddSeconds(loopDelay);
					}
					if (base.currentTime < loopDelayEndTime)
					{
						inLoopDelay = true;
						continue;
					}
					inLoopDelay = false;
				}
				loopCount--;
				OnLoop?.Execute();
				SetStartTime();
				SetEndTime();
				base.currentTime = base.startTime;
				base.elapsedTime = TimeSpan.Zero;
				base.remainingTime = base.endTime - base.startTime;
				UpdateCurrentTime();
			}
			else
			{
				base.isRunning = false;
				OnFinish?.Execute();
			}
		}
	}
}
