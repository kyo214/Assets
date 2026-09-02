using System;
using Doozy.Runtime.Reactor.Internal;
using Doozy.Runtime.Reactor.Reflection;
using UnityEngine;

namespace Doozy.Runtime.Reactor.Reactions;

[Serializable]
public class ReflectedIntReaction : IntReaction
{
	[SerializeField]
	private bool Enabled;

	[SerializeField]
	private int StartValue;

	[SerializeField]
	private ReferenceValue FromReferenceValue;

	[SerializeField]
	private ReferenceValue ToReferenceValue;

	[SerializeField]
	private int FromCustomValue;

	[SerializeField]
	private int ToCustomValue;

	[SerializeField]
	private int FromOffset;

	[SerializeField]
	private int ToOffset;

	public ReflectedInt valueTarget { get; private set; }

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

	public int startValue
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

	public int currentReflectedValue
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

	public int fromCustomValue
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

	public int toCustomValue
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

	public int fromOffset
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

	public int toOffset
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
		FromCustomValue = 0;
		FromOffset = 0;
		ToReferenceValue = ReferenceValue.StartValue;
		ToCustomValue = 100;
		ToOffset = 0;
	}

	public ReflectedIntReaction SetTarget(ReflectedInt target)
	{
		this.SetTargetObject(target);
		valueTarget = target;
		startValue = target.value;
		base.getter = () => currentReflectedValue;
		base.setter = (int value) =>
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

	private int GetValue(ReferenceValue referenceValue, int offset, int customValue)
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
