using System;
using Doozy.Runtime.Reactor.Internal;
using UnityEngine;

namespace Doozy.Runtime.Reactor.Reactions;

[Serializable]
public class UIFadeReaction : FloatReaction
{
	[SerializeField]
	private bool Enabled;

	[SerializeField]
	private float StartAlpha;

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

	public RectTransform rectTransform { get; private set; }

	public CanvasGroup canvasGroup { get; private set; }

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

	public float startAlpha
	{
		get
		{
			return StartAlpha;
		}
		set
		{
			StartAlpha = value;
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

	public float currentAlpha
	{
		get
		{
			return canvasGroup.alpha;
		}
		set
		{
			if (canvasGroup == null)
			{
				Stop();
			}
			else
			{
				canvasGroup.alpha = Mathf.Clamp01(value);
			}
		}
	}

	public override void Reset()
	{
		base.Reset();
		rectTransform = null;
		canvasGroup = null;
		FromReferenceValue = ReferenceValue.StartValue;
		FromCustomValue = 1f;
		FromOffset = 0f;
		ToReferenceValue = ReferenceValue.StartValue;
		ToCustomValue = 1f;
		ToOffset = 0f;
	}

	public UIFadeReaction SetTarget(RectTransform targetRectTransform, CanvasGroup targetCanvasGroup)
	{
		this.SetTargetObject(targetRectTransform);
		rectTransform = targetRectTransform;
		canvasGroup = targetCanvasGroup;
		StartAlpha = currentAlpha;
		base.getter = () => currentAlpha;
		base.setter = (float value) =>
		{
			currentAlpha = value;
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
		return Mathf.Clamp01(referenceValue switch
		{
			ReferenceValue.StartValue => StartAlpha + offset, 
			ReferenceValue.CurrentValue => currentAlpha + offset, 
			ReferenceValue.CustomValue => customValue, 
			_ => throw new ArgumentOutOfRangeException(), 
		});
	}
}
