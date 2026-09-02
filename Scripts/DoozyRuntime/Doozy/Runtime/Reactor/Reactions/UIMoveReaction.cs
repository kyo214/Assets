using System;
using Doozy.Runtime.Reactor.Internal;
using UnityEngine;

namespace Doozy.Runtime.Reactor.Reactions;

[Serializable]
public class UIMoveReaction : Vector3Reaction
{
	internal bool UseCustomLocalScale;

	internal Vector3 CustomFromLocalScale = Vector3.one;

	internal Vector3 CustomToLocalScale = Vector3.one;

	internal bool UseCustomLocalRotation;

	internal Vector3 CustomFromLocalRotation = Vector3.zero;

	internal Vector3 CustomToLocalRotation = Vector3.zero;

	[SerializeField]
	private bool Enabled;

	[SerializeField]
	private UIAnimationType AnimationType = UIAnimationType.Custom;

	[SerializeField]
	private Vector3 StartPosition;

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

	[SerializeField]
	private MoveDirection FromDirection;

	[SerializeField]
	private MoveDirection ToDirection;

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

	public UIAnimationType animationType
	{
		get
		{
			return AnimationType;
		}
		set
		{
			AnimationType = value;
		}
	}

	public Vector3 startPosition
	{
		get
		{
			return StartPosition;
		}
		set
		{
			StartPosition = value;
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

	public MoveDirection fromDirection
	{
		get
		{
			return FromDirection;
		}
		set
		{
			FromDirection = value;
			if (FromDirection != MoveDirection.CustomPosition)
			{
				ToDirection = MoveDirection.CustomPosition;
			}
		}
	}

	public MoveDirection toDirection
	{
		get
		{
			return ToDirection;
		}
		set
		{
			ToDirection = value;
			if (ToDirection != MoveDirection.CustomPosition)
			{
				FromDirection = MoveDirection.CustomPosition;
			}
		}
	}

	public Vector3 currentPosition
	{
		get
		{
			return rectTransform.anchoredPosition3D;
		}
		set
		{
			if (rectTransform == null)
			{
				Stop();
			}
			else
			{
				rectTransform.anchoredPosition3D = value;
			}
		}
	}

	public override void Reset()
	{
		base.Reset();
		rectTransform = null;
		AnimationType = UIAnimationType.Custom;
		FromDirection = MoveDirection.Left;
		FromReferenceValue = ReferenceValue.StartValue;
		FromCustomValue = Vector3.zero;
		FromOffset = Vector3.zero;
		ToDirection = MoveDirection.Left;
		ToReferenceValue = ReferenceValue.StartValue;
		ToCustomValue = Vector3.zero;
		ToOffset = Vector3.zero;
	}

	public UIMoveReaction SetTarget(RectTransform target)
	{
		this.SetTargetObject(target);
		rectTransform = target;
		StartPosition = currentPosition;
		base.getter = () => currentPosition;
		base.setter = (Vector3 value) =>
		{
			currentPosition = value;
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
		switch (animationType)
		{
		case UIAnimationType.Show:
			SetTo(GetValue(ToReferenceValue, ToOffset, ToCustomValue));
			SetFrom((fromDirection == MoveDirection.CustomPosition) ? FromCustomValue : (ReactorUtils.GetMoveInPosition(rectTransform, fromDirection, ToValue, UseCustomLocalScale ? CustomFromLocalScale : rectTransform.localScale, UseCustomLocalRotation ? CustomFromLocalRotation : rectTransform.localEulerAngles) + FromOffset));
			break;
		case UIAnimationType.Hide:
			SetFrom(GetValue(FromReferenceValue, FromOffset, FromCustomValue));
			SetTo((toDirection == MoveDirection.CustomPosition) ? ToCustomValue : (ReactorUtils.GetMoveOutPosition(rectTransform, toDirection, FromValue, UseCustomLocalScale ? CustomToLocalScale : rectTransform.localScale, UseCustomLocalRotation ? CustomToLocalRotation : rectTransform.localEulerAngles) + ToOffset));
			break;
		case UIAnimationType.Loop:
		case UIAnimationType.Button:
		case UIAnimationType.State:
		case UIAnimationType.Reset:
		case UIAnimationType.Custom:
			SetFrom(GetValue(FromReferenceValue, FromOffset, FromCustomValue));
			SetTo(GetValue(ToReferenceValue, ToOffset, ToCustomValue));
			break;
		default:
			throw new ArgumentOutOfRangeException();
		}
	}

	private Vector3 GetValue(ReferenceValue referenceValue, Vector3 offset, Vector3 customValue)
	{
		return referenceValue switch
		{
			ReferenceValue.StartValue => StartPosition + offset, 
			ReferenceValue.CurrentValue => currentPosition + offset, 
			ReferenceValue.CustomValue => customValue, 
			_ => throw new ArgumentOutOfRangeException(), 
		};
	}
}
