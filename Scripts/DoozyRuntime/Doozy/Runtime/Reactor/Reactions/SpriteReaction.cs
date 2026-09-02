using System;
using System.Collections.Generic;
using System.Linq;
using Doozy.Runtime.Reactor.Easings;
using Doozy.Runtime.Reactor.Internal;
using UnityEngine;

namespace Doozy.Runtime.Reactor.Reactions;

[Serializable]
public class SpriteReaction : DynamicReaction<Sprite, int>
{
	public const int DEFAULT_CAPACITY = 100;

	public ReactionCallback<Sprite> OnFrameChangedCallback;

	private List<Sprite> sprites { get; set; }

	public int firstFrame => 0;

	public int lastFrame
	{
		get
		{
			if (sprites != null && sprites.Count != 0)
			{
				return sprites.Count - 1;
			}
			return 0;
		}
	}

	public int currentFrame => CurrentValue;

	public Sprite current
	{
		get
		{
			if (sprites != null)
			{
				if (sprites.Count != 0)
				{
					return sprites?[Mathf.Clamp(currentFrame, 0, lastFrame)];
				}
				return null;
			}
			return null;
		}
	}

	public SpriteReaction()
	{
		this.SetEase(Ease.Linear);
		FromValue = firstFrame;
		ToValue = lastFrame;
	}

	public SpriteReaction(IEnumerable<Sprite> sprites)
		: this()
	{
		SetSprites(sprites.ToList());
		FromValue = firstFrame;
		ToValue = lastFrame;
	}

	public override void Reset()
	{
		base.Reset();
		this.SetEase(Ease.Linear);
		if (sprites == null)
		{
			sprites = new List<Sprite>(100);
		}
		else
		{
			sprites.Clear();
		}
		sprites.Add(null);
		OnFrameChangedCallback = null;
	}

	public override float GetProgressAtValue(int value)
	{
		return Mathf.Clamp01(Mathf.InverseLerp(FromValue, ToValue, value));
	}

	public override void UpdateCurrentValue()
	{
		CurrentValue = (int)Mathf.Lerp(base.cycleFrom, base.cycleTo, base.currentCycleEasedProgress);
		CurrentValue = Mathf.Clamp(CurrentValue, firstFrame, lastFrame);
		base.setter?.Invoke(current);
		OnValueChangedCallback?.Invoke(CurrentValue);
		OnFrameChangedCallback?.Invoke(current);
	}

	public sealed override Reaction SetValue(int value)
	{
		value = Mathf.Clamp(value, firstFrame, lastFrame);
		base.SetValue(value);
		base.setter?.Invoke(current);
		return this;
	}

	public override Reaction SetFrom(int value, bool relative = false)
	{
		FromValue = value;
		if (relative)
		{
			FromValue += CurrentValue;
		}
		FromValue = Mathf.Clamp(FromValue, firstFrame, lastFrame);
		if (base.isActive)
		{
			ComputePlayMode();
		}
		return this;
	}

	public override Reaction SetTo(int value, bool relative = false)
	{
		ToValue = value;
		if (relative)
		{
			ToValue += CurrentValue;
		}
		ToValue = Mathf.Clamp(ToValue, firstFrame, lastFrame);
		if (base.isActive)
		{
			ComputePlayMode();
		}
		return this;
	}

	public override void Play(bool inReverse = false)
	{
		base.Play(inReverse);
	}

	public override void PlayFromToProgress(float fromProgress, float toProgress)
	{
		FromValue = firstFrame;
		ToValue = lastFrame;
		base.PlayFromToProgress(fromProgress, toProgress);
	}

	public override void PlayToProgress(float toProgress)
	{
		FromValue = firstFrame;
		ToValue = lastFrame;
		base.PlayToProgress(toProgress);
	}

	public override void PlayFromProgress(float fromProgress)
	{
		FromValue = firstFrame;
		ToValue = lastFrame;
		base.PlayFromProgress(fromProgress);
	}

	public SpriteReaction SetFrame(int frameNumber)
	{
		return (SpriteReaction)SetValue(frameNumber);
	}

	public SpriteReaction ReverseSpritesOrder()
	{
		int count = sprites.Count;
		for (int i = 0; i < count / 2; i++)
		{
			List<Sprite> list = sprites;
			int index = i;
			List<Sprite> list2 = sprites;
			int index2 = count - i - 1;
			Sprite sprite = sprites[count - i - 1];
			Sprite sprite2 = sprites[i];
			Sprite sprite3 = (list[index] = sprite);
			sprite3 = (list2[index2] = sprite2);
		}
		base.setter?.Invoke(current);
		return this;
	}

	public SpriteReaction SetSprites(List<Sprite> spriteList, bool setFirstFrame = true)
	{
		if (spriteList == null)
		{
			throw new ArgumentNullException("spriteList");
		}
		if (base.isActive)
		{
			Stop(silent: true);
		}
		int count = spriteList.Count;
		if (sprites != null && sprites.Count > 0)
		{
			sprites.Clear();
			if (sprites.Capacity != count)
			{
				sprites.Capacity = count;
			}
		}
		if (sprites == null)
		{
			List<Sprite> list = (sprites = new List<Sprite>(count));
		}
		sprites.AddRange(spriteList);
		FromValue = firstFrame;
		ToValue = lastFrame;
		if (setFirstFrame)
		{
			SetFirstFrame();
		}
		return this;
	}

	public SpriteReaction SetFrameAtProgress(float targetProgress)
	{
		return SetFrame((int)((float)lastFrame * targetProgress));
	}

	public int GetFrameAtProgress(float targetProgress)
	{
		return Mathf.Clamp((int)((float)lastFrame * Mathf.Clamp01(targetProgress)), firstFrame, lastFrame);
	}

	public Sprite GetSpriteAtProgress(float targetProgress)
	{
		if (sprites != null && sprites.Count != 0)
		{
			return sprites[GetFrameAtProgress(targetProgress)];
		}
		return null;
	}

	public SpriteReaction SetFirstFrame()
	{
		return SetFrame(firstFrame);
	}

	public SpriteReaction SetLastFrame()
	{
		return SetFrame(lastFrame);
	}

	public float GetProgressAtFrame(int frameNumber)
	{
		return (float)Mathf.Clamp(frameNumber, firstFrame, lastFrame) / (float)lastFrame;
	}

	public float GetCurrentFrameProgress()
	{
		return GetProgressAtFrame(currentFrame);
	}

	protected override void ComputeSpring()
	{
		base.ComputeSpring();
		float num = base.settings.strength;
		float num2 = num / (float)(base.numberOfCycles - 1);
		for (int i = 0; i < base.numberOfCycles; i++)
		{
			base.cycleValues[i] = (int)((float)FromValue + (float)ToValue * ((i % 2 == 0) ? num : ((0f - num) * base.settings.elasticity)));
			base.cycleValues[i] = Mathf.Clamp(base.cycleValues[i], firstFrame, lastFrame);
			num -= num2;
		}
		base.cycleValues[base.numberOfCycles - 1] = FromValue;
	}

	protected override void ComputeShake()
	{
		base.ComputeShake();
		for (int i = 0; i < base.numberOfCycles; i++)
		{
			if (i % 2 == 0)
			{
				base.cycleValues[i] = FromValue;
				continue;
			}
			float value = UnityEngine.Random.value;
			base.cycleValues[i] = (int)((float)FromValue + (float)ToValue * value * base.settings.strength);
			base.cycleValues[i] = Mathf.Clamp(base.cycleValues[i], firstFrame, lastFrame);
		}
		base.cycleValues[base.numberOfCycles - 1] = FromValue;
	}
}
