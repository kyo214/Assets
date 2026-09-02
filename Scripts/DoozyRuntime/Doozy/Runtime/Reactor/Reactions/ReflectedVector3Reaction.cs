using System;
using Doozy.Runtime.Reactor.Internal;
using Doozy.Runtime.Reactor.Reflection;
using UnityEngine;

namespace Doozy.Runtime.Reactor.Reactions;

[Serializable]
public class ReflectedVector3Reaction : Vector3Reaction
{
	[SerializeField]
	private bool Enabled;

	[SerializeField]
	private Vector3 StartValue;

	[SerializeField]
	private ReferenceValue FromReferenceValue;

	[SerializeField]
	private ReferenceValue ToReferenceValue;

	[SerializeField]
	private Vector3 FromCustomValue;

	[SerializeField]
	private Vector3 ToCustomValue;

	[SerializeField]
	private Vector3 FromOffset;

	[SerializeField]
	private Vector3 ToOffset;

	public ReflectedVector3 valueTarget { get; private set; }

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

	public Vector3 startValue
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

	public Vector3 currentReflectedValue
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

	public Vector3 fromCustomValue
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

	public Vector3 toCustomValue
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

	public Vector3 fromOffset
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

	public Vector3 toOffset
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
		FromCustomValue = Vector3.zero;
		FromOffset = Vector3.zero;
		ToReferenceValue = ReferenceValue.StartValue;
		ToCustomValue = Vector3.one;
		ToOffset = Vector3.zero;
	}

	public ReflectedVector3Reaction SetTarget(ReflectedVector3 target)
	{
		this.SetTargetObject(target);
		valueTarget = target;
		startValue = target.value;
		base.getter = () => currentReflectedValue;
		base.setter = (Vector3 value) =>
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

	private Vector3 GetValue(ReferenceValue referenceValue, Vector3 offset, Vector3 customValue)
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
