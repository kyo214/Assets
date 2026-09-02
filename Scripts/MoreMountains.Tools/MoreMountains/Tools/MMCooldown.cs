using System;
using UnityEngine;

namespace MoreMountains.Tools;

[Serializable]
public class MMCooldown
{
	public enum CooldownStates
	{
		Idle = 0,
		Consuming = 1,
		PauseOnEmpty = 2,
		Refilling = 3
	}

	public bool Unlimited;

	public float ConsumptionDuration = 2f;

	public float PauseOnEmptyDuration = 1f;

	public float RefillDuration = 1f;

	public bool CanInterruptRefill = true;

	[MMReadOnly]
	public CooldownStates CooldownState;

	[MMReadOnly]
	public float CurrentDurationLeft;

	protected WaitForSeconds _pauseOnEmptyWFS;

	protected float _emptyReachedTimestamp;

	public float Progress
	{
		get
		{
			if (Unlimited)
			{
				return 1f;
			}
			if (CooldownState == CooldownStates.Consuming || CooldownState == CooldownStates.PauseOnEmpty)
			{
				return 0f;
			}
			if (CooldownState == CooldownStates.Refilling)
			{
				return CurrentDurationLeft / RefillDuration;
			}
			return 1f;
		}
	}

	public virtual void Initialization()
	{
		_pauseOnEmptyWFS = new WaitForSeconds(PauseOnEmptyDuration);
		CurrentDurationLeft = ConsumptionDuration;
		CooldownState = CooldownStates.Idle;
		_emptyReachedTimestamp = 0f;
	}

	public virtual void Start()
	{
		if (Ready())
		{
			CooldownState = CooldownStates.Consuming;
		}
	}

	public virtual bool Ready()
	{
		if (Unlimited)
		{
			return true;
		}
		if (CooldownState == CooldownStates.Idle)
		{
			return true;
		}
		if (CooldownState == CooldownStates.Refilling && CanInterruptRefill)
		{
			return true;
		}
		return false;
	}

	public virtual void Stop()
	{
		if (CooldownState == CooldownStates.Consuming)
		{
			CooldownState = CooldownStates.PauseOnEmpty;
		}
	}

	public virtual void Update()
	{
		if (Unlimited)
		{
			return;
		}
		switch (CooldownState)
		{
		case CooldownStates.Consuming:
			CurrentDurationLeft -= Time.deltaTime;
			if (CurrentDurationLeft <= 0f)
			{
				CurrentDurationLeft = 0f;
				_emptyReachedTimestamp = Time.time;
				CooldownState = CooldownStates.PauseOnEmpty;
			}
			break;
		case CooldownStates.PauseOnEmpty:
			if (Time.time - _emptyReachedTimestamp >= PauseOnEmptyDuration)
			{
				CooldownState = CooldownStates.Refilling;
			}
			break;
		case CooldownStates.Refilling:
			CurrentDurationLeft += RefillDuration * Time.deltaTime / RefillDuration;
			if (CurrentDurationLeft >= RefillDuration)
			{
				CurrentDurationLeft = ConsumptionDuration;
				CooldownState = CooldownStates.Idle;
			}
			break;
		case CooldownStates.Idle:
			break;
		}
	}
}
