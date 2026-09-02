using System;
using Doozy.Runtime.Reactor.Internal;
using Doozy.Runtime.Reactor.Reflection;
using UnityEngine;

namespace Doozy.Runtime.Reactor.Reactions;

[Serializable]
public class ReflectedVector2Reaction : Vector2Reaction
{
	[SerializeField]
	private bool Enabled;

	[SerializeField]
	private Vector2 StartValue;

	[SerializeField]
	private ReferenceValue FromReferenceValue;

	[SerializeField]
	private ReferenceValue ToReferenceValue;

	[SerializeField]
	private Vector2 FromCustomValue;

	[SerializeField]
	private Vector2 ToCustomValue;

	[SerializeField]
	private Vector2 FromOffset;

	[SerializeField]
	private Vector2 ToOffset;

	public ReflectedVector2 valueTarget { get; private set; }

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

	public Vector2 startValue
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

	public Vector2 currentReflectedValue
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

	public Vector2 fromCustomValue
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

	public Vector2 toCustomValue
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

	public Vector2 fromOffset
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

	public Vector2 toOffset
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
		FromCustomValue = Vector2.zero;
		FromOffset = Vector2.zero;
		ToReferenceValue = ReferenceValue.StartValue;
		ToCustomValue = Vector2.one;
		ToOffset = Vector2.zero;
	}

	public ReflectedVector2Reaction SetTarget(ReflectedVector2 target)
	{
		this.SetTargetObject(target);
		valueTarget = target;
		startValue = target.value;
		base.getter = () => currentReflectedValue;
		base.setter = (Vector2 value) =>
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

	private Vector2 GetValue(ReferenceValue referenceValue, Vector2 offset, Vector2 customValue)
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
