using System;
using System.Collections.Generic;
using Doozy.Runtime.Reactor.Easings;
using Doozy.Runtime.Reactor.Internal;
using UnityEngine;

namespace Doozy.Runtime.Reactor.Reactions;

[Serializable]
public class Texture2DReaction : DynamicReaction<Texture2D, int>
{
	public const int DEFAULT_CAPACITY = 100;

	public ReactionCallback<Texture2D> OnFrameChangedCallback;

	public List<Texture2D> textures { get; private set; }

	public int numberOfFrames => textures?.Count ?? 0;

	public int firstFrame => 0;

	public int lastFrame
	{
		get
		{
			if (textures != null)
			{
				return numberOfFrames - 1;
			}
			return 0;
		}
	}

	public int currentFrame => CurrentValue;

	public Texture2D current
	{
		get
		{
			if (textures != null)
			{
				if (textures.Count != 0)
				{
					return textures[Mathf.Clamp(currentFrame, 0, lastFrame)];
				}
				return null;
			}
			return null;
		}
	}

	public Texture2DReaction()
	{
		this.SetEase(Ease.Linear);
		FromValue = firstFrame;
		ToValue = lastFrame;
	}

	public Texture2DReaction(IEnumerable<Texture2D> textures)
		: this()
	{
		SetTextures(textures);
		FromValue = firstFrame;
		ToValue = lastFrame;
	}

	public override void Reset()
	{
		base.Reset();
		this.SetEase(Ease.Linear);
		if (textures == null)
		{
			textures = new List<Texture2D>(100);
		}
		else
		{
			textures.Clear();
		}
		textures.Add(null);
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
		FromValue = firstFrame;
		ToValue = lastFrame;
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

	public Texture2DReaction SetFrame(int frameNumber)
	{
		return (Texture2DReaction)SetValue(frameNumber);
	}

	public Texture2DReaction ReverseTexturesOrder()
	{
		int count = textures.Count;
		for (int i = 0; i < count / 2; i++)
		{
			List<Texture2D> list = textures;
			int index = i;
			List<Texture2D> list2 = textures;
			int index2 = count - i - 1;
			Texture2D texture2D = textures[count - i - 1];
			Texture2D texture2D2 = textures[i];
			Texture2D texture2D3 = (list[index] = texture2D);
			texture2D3 = (list2[index2] = texture2D2);
		}
		base.setter?.Invoke(current);
		return this;
	}

	public Texture2DReaction SetTextures(IEnumerable<Texture2D> textures2D)
	{
		if (textures2D == null)
		{
			throw new ArgumentNullException("textures2D");
		}
		if (base.isActive)
		{
			Stop(silent: true);
		}
		if (textures == null)
		{
			List<Texture2D> list = (textures = new List<Texture2D>(100));
		}
		int count = textures.Count;
		int num = 0;
		foreach (Texture2D item in textures2D)
		{
			if (num < count)
			{
				textures[num] = item;
			}
			else
			{
				textures.Add(item);
			}
			num++;
		}
		if (num < count)
		{
			textures.RemoveRange(num, count - num);
		}
		else if (num > textures.Capacity)
		{
			textures.Capacity = num;
		}
		ToValue = lastFrame;
		SetFirstFrame();
		return this;
	}

	public Texture2DReaction SetFrameAtProgress(float targetProgress)
	{
		return SetFrame((int)((float)lastFrame * targetProgress));
	}

	public Texture2DReaction SetFirstFrame()
	{
		return SetFrame(firstFrame);
	}

	public Texture2DReaction SetLastFrame()
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
