using System;
using Doozy.Runtime.Reactor.Internal;
using Doozy.Runtime.Reactor.Reflection;
using UnityEngine;

namespace Doozy.Runtime.Reactor.Reactions;

[Serializable]
public class ReflectedFloatReaction : FloatReaction
{
	[SerializeField]
	private bool Enabled;

	[SerializeField]
	private float StartValue;

	[SerializeField]
	private ReferenceValue FromReferenceValue;

	[SerializeField]
	private ReferenceValue ToReferenceValue;

	[SerializeField]
	private float FromCustomValue;

	[SerializeField]
	private float ToCustomValue;

	[SerializeField]
	private float FromOffset;

	[SerializeField]
	private float ToOffset;

	public ReflectedFloat valueTarget { get; private set; }

	public bool enabled
	{
		get
		{
			return Enabled;
		}
		set
		{
			Enabled = value;
		}
	}

	public float startValue
	{
		get
		{
			return StartValue;
		}
		set
		{
			StartValue = value;
			if (base.isActive)
			{
				UpdateValues();
			}
		}
	}

	public float currentReflectedValue
	{
		get
		{
			return valueTarget.value;
		}
		set
		{
			valueTarget.value = value;
		}
	}

	public ReferenceValue fromReferenceValue
	{
		get
		{
			return FromReferenceValue;
		}
		set
		{
			FromReferenceValue = value;
		}
	}

	public ReferenceValue toReferenceValue
	{
		get
		{
			return ToReferenceValue;
		}
		set
		{
			ToReferenceValue = value;
		}
	}

	public float fromCustomValue
	{
		get
		{
			return FromCustomValue;
		}
		set
		{
			FromCustomValue = value;
		}
	}

	public float toCustomValue
	{
		get
		{
			return ToCustomValue;
		}
		set
		{
			ToCustomValue = value;
		}
	}

	public float fromOffset
	{
		get
		{
			return FromOffset;
		}
		set
		{
			FromOffset = value;
		}
	}

	public float toOffset
	{
		get
		{
			return ToOffset;
		}
		set
		{
			ToOffset = value;
		}
	}

	public override void Reset()
	{
		base.Reset();
		valueTarget = null;
		FromReferenceValue = ReferenceValue.StartValue;
		FromCustomValue = 0f;
		FromOffset = 0f;
		ToReferenceValue = ReferenceValue.StartValue;
		ToCustomValue = 1f;
		ToOffset = 0f;
	}

	public ReflectedFloatReaction SetTarget(ReflectedFloat target)
	{
		this.SetTargetObject(target);
		valueTarget = target;
		startValue = target.value;
		base.getter = () => currentReflectedValue;
		base.setter = (float value) =>
		{
			currentReflectedValue = value;
		};
		return this;
	}

	public override void Play(bool inReverse = false)
	{
		if (!base.isActive)
		{
			UpdateValues();
			SetValue(inReverse ? ToValue : FromValue);
		}
		base.Play(inReverse);
	}

	public override void PlayFromProgress(float fromProgress)
	{
		UpdateValues();
		base.PlayFromProgress(fromProgress);
	}

	public override void SetProgressAt(float targetProgress)
	{
		UpdateValues();
		base.SetProgressAt(targetProgress);
	}

	public void UpdateValues()
	{
		SetFrom(GetValue(FromReferenceValue, FromOffset, FromCustomValue));
		SetTo(GetValue(ToReferenceValue, ToOffset, ToCustomValue));
	}

	private float GetValue(ReferenceValue referenceValue, float offset, float customValue)
	{
		return referenceValue switch
		{
			ReferenceValue.StartValue => startValue + offset, 
			ReferenceValue.CurrentValue => currentReflectedValue + offset, 
			ReferenceValue.CustomValue => customValue, 
			_ => throw new ArgumentOutOfRangeException(), 
		};
	}
}
