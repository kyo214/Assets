using System;
using System.Collections.Generic;
using System.Linq;
using Doozy.Runtime.Common.Attributes;
using Doozy.Runtime.Common.Events;
using Doozy.Runtime.Common.Extensions;
using Doozy.Runtime.Mody;
using Doozy.Runtime.Reactor.Internal;
using Doozy.Runtime.Reactor.Reactions;
using Doozy.Runtime.Reactor.Ticker;
using UnityEngine;

namespace Doozy.Runtime.Reactor;

[AddComponentMenu("Reactor/Progressor")]
public class Progressor : MonoBehaviour
{
	[SerializeField]
	private List<ProgressTarget> ProgressTargets;

	[SerializeField]
	private List<Progressor> ProgressorTargets;

	[SerializeField]
	protected float FromValue;

	[SerializeField]
	protected float ToValue = 1f;

	[SerializeField]
	protected float CurrentValue;

	[SerializeField]
	protected float Progress;

	[SerializeField]
	protected float CustomResetValue;

	[SerializeField]
	protected FloatReaction Reaction;

	public ResetValue ResetValueOnEnable;

	public FloatEvent OnValueChanged;

	public FloatEvent OnProgressChanged;

	public FloatEvent OnValueIncremented = new FloatEvent();

	public FloatEvent OnValueDecremented = new FloatEvent();

	public ModyEvent OnValueReset = new ModyEvent();

	public ModyEvent OnValueReachedFromValue = new ModyEvent();

	public ModyEvent OnValueReachedToValue = new ModyEvent();

	public ProgressorId Id;

	public static HashSet<Progressor> database { get; private set; } = new HashSet<Progressor>();

	public List<ProgressTarget> progressTargets
	{
		get
		{
			Initialize();
			return ProgressTargets;
		}
	}

	public List<Progressor> progressorTargets
	{
		get
		{
			Initialize();
			return ProgressorTargets;
		}
	}

	public float fromValue
	{
		get
		{
			return FromValue;
		}
		set
		{
			FromValue = value;
			if (reaction.isActive)
			{
				reaction.SetFrom(fromValue);
			}
		}
	}

	public float toValue
	{
		get
		{
			return ToValue;
		}
		set
		{
			ToValue = value;
			if (reaction.isActive)
			{
				reaction.SetTo(toValue);
			}
		}
	}

	public float currentValue => CurrentValue;

	public float progress => Progress;

	public float customResetValue
	{
		get
		{
			return CustomResetValue;
		}
		set
		{
			CustomResetValue = Mathf.Clamp(value, FromValue, ToValue);
		}
	}

	public FloatReaction reaction
	{
		get
		{
			Initialize();
			return Reaction;
		}
	}

	public bool initialized { get; set; }

	[ExecuteOnReload]
	private static void OnReload()
	{
		database = new HashSet<Progressor>();
	}

	protected Progressor()
	{
		Id = new ProgressorId();
	}

	public virtual void Initialize()
	{
		if (!initialized)
		{
			if (ProgressTargets == null)
			{
				ProgressTargets = new List<ProgressTarget>();
			}
			if (ProgressorTargets == null)
			{
				ProgressorTargets = new List<Progressor>();
			}
			Reaction = Reaction ?? ReactionPool.Get<FloatReaction>();
			Reaction.SetFrom(fromValue);
			Reaction.SetTo(toValue);
			Reaction.SetValue(fromValue);
			Reaction.OnUpdateCallback = UpdateProgressor;
			initialized = true;
		}
	}

	protected virtual void Awake()
	{
		if (Application.isPlaying)
		{
			database.Add(this);
			Initialize();
		}
	}

	protected virtual void OnEnable()
	{
		if (Application.isPlaying)
		{
			CleanDatabase();
			ValidateTargets();
			ResetCurrentValue(ResetValueOnEnable);
		}
	}

	protected virtual void OnDisable()
	{
		if (Application.isPlaying)
		{
			CleanDatabase();
			ValidateTargets();
			reaction.Stop();
		}
	}

	protected void OnDestroy()
	{
		if (Application.isPlaying)
		{
			database.Remove(this);
			CleanDatabase();
			Reaction?.Recycle();
		}
	}

	private void ValidateTargets()
	{
		ProgressTargets = progressTargets.Where((ProgressTarget t) => t != null).Distinct().ToList();
	}

	protected void ResetCurrentValue(ResetValue resetValue)
	{
		if (resetValue != ResetValue.Disabled)
		{
			reaction.SetFrom(FromValue);
			reaction.SetTo(ToValue);
			switch (resetValue)
			{
			case ResetValue.FromValue:
				SetProgressAtZero();
				OnValueReset?.Execute();
				break;
			case ResetValue.EndValue:
				SetProgressAtOne();
				OnValueReset?.Execute();
				break;
			case ResetValue.CustomValue:
				SetProgressAt(reaction.GetProgressAtValue(CustomResetValue));
				OnValueReset?.Execute();
				break;
			default:
				throw new ArgumentOutOfRangeException("resetValue", resetValue, null);
			}
		}
	}

	public void ResetToStartValues()
	{
		if (reaction.isActive)
		{
			Stop();
		}
		ResetCurrentValue(ResetValueOnEnable);
	}

	public virtual void UpdateProgressor()
	{
		float num = CurrentValue;
		CurrentValue = reaction.currentValue;
		Progress = Mathf.InverseLerp(fromValue, toValue, currentValue);
		if (num < CurrentValue)
		{
			OnValueIncremented?.Invoke(CurrentValue - num);
		}
		else if (num > CurrentValue)
		{
			OnValueDecremented?.Invoke(num - CurrentValue);
		}
		OnValueChanged?.Invoke(CurrentValue);
		OnProgressChanged?.Invoke(Progress);
		if (currentValue.Approximately(fromValue))
		{
			OnValueReachedFromValue?.Execute();
		}
		if (currentValue.Approximately(toValue))
		{
			OnValueReachedToValue?.Execute();
		}
		ProgressTargets.RemoveNulls();
		ProgressTargets.ForEach((ProgressTarget t) =>
		{
			t.UpdateTarget(this);
		});
		ProgressorTargets.RemoveNulls();
		for (int num2 = ProgressorTargets.Count - 1; num2 >= 0; num2--)
		{
			if (progressorTargets[num2] == this)
			{
				ProgressorTargets.RemoveAt(num2);
			}
		}
		ProgressorTargets.ForEach((Progressor p) =>
		{
			p.SetProgressAt(Progress);
		});
	}

	public void SetValueAt(float value)
	{
		SetProgressAt(reaction.GetProgressAtValue(Mathf.Clamp(value, fromValue, toValue)));
	}

	public void SetProgressAt(float targetProgress)
	{
		reaction.SetFrom(FromValue);
		reaction.SetTo(ToValue);
		reaction.SetProgressAt(targetProgress);
		UpdateProgressor();
	}

	public void SetProgressAtOne()
	{
		SetProgressAt(1f);
	}

	public void SetProgressAtZero()
	{
		SetProgressAt(0f);
	}

	public void Play(PlayDirection direction)
	{
		Play(direction == PlayDirection.Reverse);
	}

	public void Play(bool inReverse = false)
	{
		reaction.SetValue(inReverse ? ToValue : FromValue);
		reaction.Play(FromValue, ToValue, inReverse);
	}

	public void PlayToValue(float value)
	{
		value = Mathf.Clamp(value, fromValue, toValue);
		if (Math.Abs(value - fromValue) < 0.001f)
		{
			PlayToProgress(0f);
		}
		else if (Math.Abs(value - toValue) < 0.001f)
		{
			PlayToProgress(1f);
		}
		else
		{
			PlayToProgress(Mathf.InverseLerp(fromValue, toValue, value));
		}
	}

	public void PlayToProgress(float toProgress)
	{
		float num = Mathf.Clamp01(toProgress);
		if (num != 0f)
		{
			if (num == 1f)
			{
				reaction.Play(currentValue, toValue);
			}
			else
			{
				reaction.Play(currentValue, Mathf.Lerp(fromValue, toValue, num));
			}
		}
		else
		{
			reaction.Play(currentValue, fromValue);
		}
	}

	public void Stop()
	{
		reaction.Stop();
	}

	public void Reverse()
	{
		reaction.Reverse();
	}

	public void Rewind()
	{
		reaction.Rewind();
	}

	public float GetStartDelay()
	{
		if (!reaction.isActive)
		{
			return reaction.settings.GetStartDelay();
		}
		return reaction.startDelay;
	}

	public float GetDuration()
	{
		if (!reaction.isActive)
		{
			return reaction.settings.GetDuration();
		}
		return reaction.duration;
	}

	public float GetTotalDuration()
	{
		return GetStartDelay() + GetDuration();
	}

	public List<Heartbeat> SetHeartbeat<T>() where T : Heartbeat, new()
	{
		List<Heartbeat> list = new List<Heartbeat>
		{
			new T()
		};
		reaction.SetHeartbeat(list[0]);
		return list;
	}

	protected static void CleanDatabase()
	{
		database.Remove(null);
	}

	public static IEnumerable<Progressor> GetProgressors(string category, string name)
	{
		return from button in database
			where button.Id.Category.Equals(category)
			where button.Id.Name.Equals(name)
			select button;
	}

	public static IEnumerable<Progressor> GetAllProgressorsInCategory(string name)
	{
		return database.Where((Progressor p) => p.Id.Category.Equals(name));
	}

	public static IEnumerable<Progressor> GetAllProgressorsByName(string name)
	{
		return database.Where((Progressor p) => p.Id.Name.Equals(name));
	}
}
