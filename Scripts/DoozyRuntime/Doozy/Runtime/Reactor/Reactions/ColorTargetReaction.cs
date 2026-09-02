using System;
using Doozy.Runtime.Colors;
using Doozy.Runtime.Colors.Models;
using Doozy.Runtime.Reactor.Internal;
using Doozy.Runtime.Reactor.Targets;
using UnityEngine;

namespace Doozy.Runtime.Reactor.Reactions;

[Serializable]
public class ColorTargetReaction : ColorReaction
{
	[SerializeField]
	private bool Enabled;

	[SerializeField]
	private Color StartColor;

	[SerializeField]
	private ReferenceValue FromReferenceValue;

	[SerializeField]
	private ReferenceValue ToReferenceValue;

	[SerializeField]
	private Color FromCustomValue = Color.white;

	[SerializeField]
	private Color ToCustomValue = Color.white;

	[SerializeField]
	private float FromHueOffset;

	[SerializeField]
	private float ToHueOffset;

	[SerializeField]
	private float FromSaturationOffset;

	[SerializeField]
	private float ToSaturationOffset;

	[SerializeField]
	private float FromLightnessOffset;

	[SerializeField]
	private float ToLightnessOffset;

	[SerializeField]
	private float FromAlphaOffset;

	[SerializeField]
	private float ToAlphaOffset;

	public ReactorColorTarget colorTarget { get; private set; }

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

	public Color startColor
	{
		get
		{
			return StartColor;
		}
		set
		{
			StartColor = value;
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

	public Color fromCustomValue
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

	public Color toCustomValue
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

	public float fromHueOffset
	{
		get
		{
			return FromHueOffset;
		}
		set
		{
			FromHueOffset = Mathf.Clamp(value, -1f, 1f);
		}
	}

	public float toHueOffset
	{
		get
		{
			return ToHueOffset;
		}
		set
		{
			ToHueOffset = Mathf.Clamp(value, -1f, 1f);
		}
	}

	public float fromSaturationOffset
	{
		get
		{
			return FromSaturationOffset;
		}
		set
		{
			FromSaturationOffset = Mathf.Clamp(value, -1f, 1f);
		}
	}

	public float toSaturationOffset
	{
		get
		{
			return ToSaturationOffset;
		}
		set
		{
			ToSaturationOffset = Mathf.Clamp(value, -1f, 1f);
		}
	}

	public float fromLightnessOffset
	{
		get
		{
			return FromLightnessOffset;
		}
		set
		{
			FromLightnessOffset = Mathf.Clamp(value, -1f, 1f);
		}
	}

	public float toLightnessOffset
	{
		get
		{
			return ToLightnessOffset;
		}
		set
		{
			ToLightnessOffset = Mathf.Clamp(value, -1f, 1f);
		}
	}

	public float fromAlphaOffset
	{
		get
		{
			return FromAlphaOffset;
		}
		set
		{
			FromAlphaOffset = Mathf.Clamp(value, -1f, 1f);
		}
	}

	public float toAlphaOffset
	{
		get
		{
			return ToAlphaOffset;
		}
		set
		{
			ToAlphaOffset = Mathf.Clamp(value, -1f, 1f);
		}
	}

	public Color currentColor
	{
		get
		{
			return colorTarget.color;
		}
		set
		{
			colorTarget.color = value;
		}
	}

	public override void Reset()
	{
		base.Reset();
		colorTarget = null;
		FromReferenceValue = ReferenceValue.StartValue;
		ToReferenceValue = ReferenceValue.StartValue;
		FromCustomValue = Color.white;
		ToCustomValue = Color.white;
		fromHueOffset = 0f;
		toHueOffset = 0f;
		fromSaturationOffset = 0f;
		toSaturationOffset = 0f;
		fromLightnessOffset = 0f;
		toLightnessOffset = 0f;
		fromAlphaOffset = 0f;
		toAlphaOffset = 0f;
	}

	public ColorTargetReaction SetTarget(ReactorColorTarget target)
	{
		this.SetTargetObject(target);
		colorTarget = target;
		StartColor = currentColor;
		base.getter = () => currentColor;
		base.setter = (Color value) =>
		{
			currentColor = value;
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
		SetFrom(GetValue(FromReferenceValue, startColor, currentColor, FromCustomValue, FromHueOffset, FromSaturationOffset, FromLightnessOffset, FromAlphaOffset));
		SetTo(GetValue(ToReferenceValue, startColor, currentColor, ToCustomValue, ToHueOffset, ToSaturationOffset, ToLightnessOffset, ToAlphaOffset));
	}

	public Color GetValue(ReferenceValue referenceValue, Color refStartValue, Color refCurrentValue, Color refCustomValue, float hueOffset, float saturationOffset, float lightnessOffset, float alphaOffset)
	{
		Color color;
		switch (referenceValue)
		{
		case ReferenceValue.StartValue:
			color = refStartValue;
			break;
		case ReferenceValue.CurrentValue:
			color = refCurrentValue;
			break;
		case ReferenceValue.CustomValue:
			return refCustomValue;
		default:
			throw new ArgumentOutOfRangeException("referenceValue", referenceValue, null);
		}
		if (((hueOffset == 0f) & (saturationOffset == 0f)) && lightnessOffset == 0f && alphaOffset == 0f)
		{
			return color;
		}
		HSL hSL = color.ToHSL();
		hSL.h += hueOffset;
		hSL.s += saturationOffset;
		hSL.l += lightnessOffset;
		hSL.h = ((hSL.h < 0f) ? (hSL.h + 1f) : ((hSL.h > 1f) ? (hSL.h - 1f) : hSL.h));
		hSL.Validate();
		float alpha = Mathf.Clamp01(color.a + alphaOffset);
		return hSL.ToColor().WithAlpha(alpha);
	}
}
