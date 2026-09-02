using System;
using Doozy.Runtime.Reactor.Internal;
using Doozy.Runtime.Reactor.Targets;
using UnityEngine;

namespace Doozy.Runtime.Reactor.Reactions;

[Serializable]
public class SpriteTargetReaction : SpriteReaction
{
	[SerializeField]
	private bool Enabled;

	[SerializeField]
	private int StartFrame;

	[SerializeField]
	private FrameReferenceValue FromReferenceValue;

	[SerializeField]
	private FrameReferenceValue ToReferenceValue = FrameReferenceValue.LastFrame;

	[SerializeField]
	private int FromCustomValue;

	[SerializeField]
	private int ToCustomValue;

	[SerializeField]
	private int FromFrameOffset;

	[SerializeField]
	private int ToFrameOffset;

	[SerializeField]
	private float FromCustomProgress;

	[SerializeField]
	private float ToCustomProgress;

	public ReactorSpriteTarget spriteTarget { get; private set; }

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

	public int startFrame
	{
		get
		{
			return StartFrame;
		}
		set
		{
			StartFrame = value;
		}
	}

	public FrameReferenceValue fromReferenceValue
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

	public FrameReferenceValue toReferenceValue
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
			FromCustomValue = Mathf.Clamp(value, base.firstFrame, base.lastFrame);
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
			ToCustomValue = Mathf.Clamp(value, base.firstFrame, base.lastFrame);
		}
	}

	public int fromFrameOffset
	{
		get
		{
			return FromFrameOffset;
		}
		set
		{
			FromFrameOffset = Mathf.Clamp(value, base.firstFrame, base.lastFrame);
		}
	}

	public int toFrameOffset
	{
		get
		{
			return ToFrameOffset;
		}
		set
		{
			ToFrameOffset = Mathf.Clamp(value, base.firstFrame, base.lastFrame);
		}
	}

	public float fromCustomProgress
	{
		get
		{
			return FromCustomProgress;
		}
		set
		{
			FromCustomProgress = Mathf.Clamp01(value);
		}
	}

	public float toCustomProgress
	{
		get
		{
			return ToCustomProgress;
		}
		set
		{
			ToCustomProgress = Mathf.Clamp01(value);
		}
	}

	public Sprite currentSprite
	{
		get
		{
			return spriteTarget.sprite;
		}
		set
		{
			spriteTarget.sprite = value;
		}
	}

	public override void Reset()
	{
		base.Reset();
		spriteTarget = null;
		FromReferenceValue = FrameReferenceValue.FirstFrame;
		ToReferenceValue = FrameReferenceValue.LastFrame;
		FromCustomValue = 0;
		ToCustomValue = 0;
		FromFrameOffset = 0;
		ToFrameOffset = 0;
		FromCustomProgress = 0f;
		ToCustomProgress = 1f;
	}

	public SpriteTargetReaction SetTarget(ReactorSpriteTarget target)
	{
		this.SetTargetObject(target);
		spriteTarget = target;
		startFrame = base.currentFrame;
		base.getter = () => currentSprite;
		base.setter = (Sprite value) =>
		{
			currentSprite = value;
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
		SetFrom(GetValue(FromReferenceValue, base.firstFrame, base.lastFrame, base.currentFrame, FromCustomValue, FromFrameOffset, FromCustomProgress));
		SetTo(GetValue(ToReferenceValue, base.firstFrame, base.lastFrame, base.currentFrame, ToCustomValue, ToFrameOffset, ToCustomProgress));
	}

	public int GetValue(FrameReferenceValue referenceValue, int refFirstFrame, int refLastFrame, int refCurrentFrame, int refCustomFrame, int refFrameOffset, float refCustomProgress)
	{
		return Mathf.Clamp(referenceValue switch
		{
			FrameReferenceValue.FirstFrame => refFirstFrame + refFrameOffset, 
			FrameReferenceValue.LastFrame => refLastFrame + refFrameOffset, 
			FrameReferenceValue.CurrentFrame => refCurrentFrame + refFrameOffset, 
			FrameReferenceValue.CustomFrame => refCustomFrame, 
			FrameReferenceValue.CustomProgress => GetFrameAtProgress(refCustomProgress), 
			_ => throw new ArgumentOutOfRangeException("referenceValue", referenceValue, null), 
		}, base.firstFrame, base.lastFrame);
	}
}
