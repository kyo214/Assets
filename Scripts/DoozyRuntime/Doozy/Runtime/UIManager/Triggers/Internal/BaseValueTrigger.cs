using System;
using Doozy.Runtime.Mody;
using UnityEngine;

namespace Doozy.Runtime.UIManager.Triggers.Internal;

public abstract class BaseValueTrigger<Tbehaviour> : MonoBehaviour where Tbehaviour : MonoBehaviour
{
	public enum TriggerWhenValueIs
	{
		EqualTo = 0,
		LessThan = 1,
		LessThanOrEqualTo = 2,
		GreaterThanOrEqualTo = 3,
		GreaterThan = 4
	}

	private const float TOLERANCE = 0.0001f;

	[SerializeField]
	protected Tbehaviour Target;

	[SerializeField]
	protected TriggerWhenValueIs TriggerMode;

	[SerializeField]
	protected float TriggerValue;

	public ModyEvent OnTrigger = new ModyEvent();

	public bool TriggerOnExactValueMatch;

	public bool TriggerOnIncrement = true;

	public bool TriggerOnDecrement = true;

	public Tbehaviour target
	{
		get
		{
			if (Target == null)
			{
				Target = GetComponent<Tbehaviour>();
			}
			return Target;
		}
	}

	public TriggerWhenValueIs triggerMode
	{
		get
		{
			return TriggerMode;
		}
		set
		{
			TriggerMode = value;
			ResetTrigger();
		}
	}

	public float triggerValue
	{
		get
		{
			return TriggerValue;
		}
		set
		{
			TriggerValue = value;
		}
	}

	protected abstract float value { get; }

	private bool triggered { get; set; }

	private float previousValue { get; set; }

	protected virtual void Reset()
	{
		Target = (Target ? Target : GetComponent<Tbehaviour>());
	}

	private void OnValueChanged(float oldValue, float newValue)
	{
		switch (triggerMode)
		{
		case TriggerWhenValueIs.EqualTo:
			if (triggered && Math.Abs(newValue - TriggerValue) > 0.0001f)
			{
				ResetTrigger();
			}
			if (!triggered && !(TriggerOnExactValueMatch & (Math.Abs(newValue - TriggerValue) > 0.0001f)))
			{
				if (((oldValue < TriggerValue) & (newValue >= TriggerValue)) && (TriggerOnIncrement || (!TriggerOnIncrement & !TriggerOnDecrement)))
				{
					Trigger();
				}
				else if (((oldValue > TriggerValue) & (newValue <= TriggerValue)) && (TriggerOnDecrement || (!TriggerOnIncrement & !TriggerOnDecrement)))
				{
					Trigger();
				}
			}
			break;
		case TriggerWhenValueIs.LessThan:
			if (triggered & (newValue > TriggerValue))
			{
				ResetTrigger();
			}
			if (!triggered && ((oldValue >= TriggerValue) & (newValue < TriggerValue) & (Math.Abs(oldValue - newValue) > 0.0001f)))
			{
				Trigger();
			}
			break;
		case TriggerWhenValueIs.LessThanOrEqualTo:
			if (triggered & (newValue > TriggerValue))
			{
				ResetTrigger();
			}
			if (!triggered && ((oldValue > TriggerValue) & (newValue <= TriggerValue)))
			{
				Trigger();
			}
			break;
		case TriggerWhenValueIs.GreaterThanOrEqualTo:
			if (triggered & (newValue < TriggerValue))
			{
				ResetTrigger();
			}
			if (!triggered && ((oldValue < TriggerValue) & (newValue >= TriggerValue)))
			{
				Trigger();
			}
			break;
		case TriggerWhenValueIs.GreaterThan:
			if (triggered & (newValue < TriggerValue))
			{
				ResetTrigger();
			}
			if (!triggered && ((oldValue <= TriggerValue) & (newValue > TriggerValue) & (Math.Abs(oldValue - newValue) > 0.0001f)))
			{
				Trigger();
			}
			break;
		default:
			throw new ArgumentOutOfRangeException();
		}
	}

	private void OnEnable()
	{
		triggered = false;
	}

	public virtual void Trigger()
	{
		triggered = true;
		OnTrigger?.Execute();
	}

	protected virtual void ResetTrigger()
	{
		triggered = false;
	}

	protected void LateUpdate()
	{
		if (!(Math.Abs(previousValue - value) < 0.0001f))
		{
			OnValueChanged(previousValue, value);
			previousValue = value;
		}
	}
}
