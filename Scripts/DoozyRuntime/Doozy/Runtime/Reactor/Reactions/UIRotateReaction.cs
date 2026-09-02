using System;
using Doozy.Runtime.Reactor.Internal;
using UnityEngine;

namespace Doozy.Runtime.Reactor.Reactions;

[Serializable]
public class UIRotateReaction : Vector3Reaction
{
	[SerializeField]
	private bool Enabled;

	[SerializeField]
	private Vector3 StartRotation;

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

	public RectTransform rectTransform { get; private set; }

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

	public Vector3 startRotation
	{
		get
		{
			return StartRotation;
		}
		set
		{
			StartRotation = value;
			if (base.isActive)
			{
				UpdateValues();
			}
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

	public Vector3 currentRotation
	{
		get
		{
			Vector3 localEulerAngles = rectTransform.localEulerAngles;
			float x = localEulerAngles.x;
			float y = localEulerAngles.y;
			float z = localEulerAngles.z;
			x = ((x > 180f) ? (x - 360f) : x);
			y = ((y > 180f) ? (y - 360f) : y);
			z = ((z > 180f) ? (z - 360f) : z);
			return new Vector3(x, y, z);
		}
		set
		{
			if (rectTransform == null)
			{
				Stop();
			}
			else
			{
				rectTransform.localEulerAngles = value;
			}
		}
	}

	public override void Reset()
	{
		base.Reset();
		rectTransform = null;
		FromReferenceValue = ReferenceValue.StartValue;
		FromCustomValue = Vector3.zero;
		FromOffset = Vector3.zero;
		ToReferenceValue = ReferenceValue.StartValue;
		ToCustomValue = Vector3.zero;
		ToOffset = Vector3.zero;
	}

	public UIRotateReaction SetTarget(RectTransform target)
	{
		this.SetTargetObject(target);
		rectTransform = target;
		StartRotation = currentRotation;
		base.getter = () => currentRotation;
		base.setter = (Vector3 value) =>
		{
			currentRotation = value;
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
			ReferenceValue.StartValue => StartRotation + offset, 
			ReferenceValue.CurrentValue => currentRotation + offset, 
			ReferenceValue.CustomValue => customValue, 
			_ => throw new ArgumentOutOfRangeException(), 
		};
	}
}
