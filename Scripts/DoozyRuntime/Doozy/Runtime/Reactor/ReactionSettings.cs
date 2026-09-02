using System;
using Doozy.Runtime.Common;
using Doozy.Runtime.Reactor.Easings;
using UnityEngine;

namespace Doozy.Runtime.Reactor;

[Serializable]
public class ReactionSettings
{
	public const int k_InfiniteLoops = -1;

	public const PlayMode k_PlayMode = PlayMode.Normal;

	public const EaseMode k_EaseMode = EaseMode.Ease;

	public const Ease k_Ease = Ease.Easy;

	public const float k_StartDelay = 0f;

	public const float k_Duration = 1f;

	public const int k_Loops = 0;

	public const float k_LoopDelay = 0f;

	public const float k_Strength = 1f;

	public const int k_Vibration = 8;

	public const float k_Elasticity = 1f;

	[SerializeField]
	private PlayMode PlayMode;

	[SerializeField]
	private EaseMode EaseMode;

	[SerializeField]
	private Ease Ease;

	[SerializeField]
	private AnimationCurve Curve;

	[SerializeField]
	private float StartDelay;

	[SerializeField]
	private float Duration;

	[SerializeField]
	private int Loops;

	[SerializeField]
	private float LoopDelay;

	[SerializeField]
	private bool UseRandomStartDelay;

	[SerializeField]
	private bool UseRandomDuration;

	[SerializeField]
	private bool UseRandomLoops;

	[SerializeField]
	private bool UseRandomLoopDelay;

	[SerializeField]
	private RandomFloat RandomStartDelay = new RandomFloat();

	[SerializeField]
	private RandomFloat RandomDuration = new RandomFloat();

	[SerializeField]
	private RandomInt RandomLoops = new RandomInt();

	[SerializeField]
	private RandomFloat RandomLoopDelay = new RandomFloat();

	[SerializeField]
	private float Strength;

	[SerializeField]
	private int Vibration;

	[SerializeField]
	private float Elasticity;

	[SerializeField]
	private bool FadeOutShake;

	public PlayMode playMode
	{
		get
		{
			return PlayMode;
		}
		set
		{
			PlayMode = value;
		}
	}

	public EaseMode easeMode
	{
		get
		{
			return EaseMode;
		}
		set
		{
			EaseMode = value;
		}
	}

	public Ease ease
	{
		get
		{
			return Ease;
		}
		set
		{
			Ease = value;
			EaseMode = EaseMode.Ease;
		}
	}

	private IEasing easing => EaseFactory.GetEase(Ease);

	public AnimationCurve curve
	{
		get
		{
			return Curve;
		}
		set
		{
			Curve = value;
			EaseMode = EaseMode.AnimationCurve;
		}
	}

	public float startDelay
	{
		get
		{
			return StartDelay;
		}
		set
		{
			StartDelay = Mathf.Max(0f, value);
		}
	}

	public float duration
	{
		get
		{
			return Duration;
		}
		set
		{
			Duration = Mathf.Max(0f, value);
		}
	}

	public int loops
	{
		get
		{
			return Loops;
		}
		set
		{
			Loops = Mathf.Max(-1, value);
		}
	}

	public float loopDelay
	{
		get
		{
			return LoopDelay;
		}
		set
		{
			LoopDelay = Mathf.Max(0f, value);
		}
	}

	public bool useRandomStartDelay
	{
		get
		{
			return UseRandomStartDelay;
		}
		set
		{
			UseRandomStartDelay = value;
		}
	}

	public bool useRandomDuration
	{
		get
		{
			return UseRandomDuration;
		}
		set
		{
			UseRandomDuration = value;
		}
	}

	public bool useRandomLoops
	{
		get
		{
			return UseRandomLoops;
		}
		set
		{
			UseRandomLoops = value;
		}
	}

	public bool useRandomLoopDelay
	{
		get
		{
			return UseRandomLoopDelay;
		}
		set
		{
			UseRandomLoopDelay = value;
		}
	}

	public RandomFloat randomStartDelay
	{
		get
		{
			return RandomStartDelay;
		}
		set
		{
			RandomStartDelay = value;
		}
	}

	public RandomFloat randomDuration
	{
		get
		{
			return RandomDuration;
		}
		set
		{
			RandomDuration = value;
		}
	}

	public RandomInt randomLoops
	{
		get
		{
			return RandomLoops;
		}
		set
		{
			RandomLoops = value;
		}
	}

	public RandomFloat randomLoopDelay
	{
		get
		{
			return RandomLoopDelay;
		}
		set
		{
			RandomLoopDelay = value;
		}
	}

	public float strength
	{
		get
		{
			return Strength;
		}
		set
		{
			Strength = value;
		}
	}

	public int vibration
	{
		get
		{
			return Vibration;
		}
		set
		{
			Vibration = Mathf.Max(0, value);
		}
	}

	public float elasticity
	{
		get
		{
			return Elasticity;
		}
		set
		{
			Elasticity = Mathf.Clamp01(value);
		}
	}

	public bool fadeOutShake
	{
		get
		{
			return FadeOutShake;
		}
		set
		{
			FadeOutShake = value;
		}
	}

	public bool hasLoops
	{
		get
		{
			if (!useRandomLoops)
			{
				return loops != 0;
			}
			return true;
		}
	}

	public float GetStartDelay()
	{
		if (!useRandomStartDelay)
		{
			return startDelay;
		}
		return randomStartDelay.randomValue;
	}

	public void SetRandomStartDelay(float min, float max, bool useRandomValue = true)
	{
		useRandomStartDelay = useRandomValue;
		min = Mathf.Max(0f, min);
		max = Mathf.Max(0f, max);
		randomStartDelay = new RandomFloat(min, max);
		if (Mathf.Approximately(min, max))
		{
			useRandomStartDelay = false;
			startDelay = min;
		}
	}

	public float GetDuration()
	{
		if (!useRandomDuration)
		{
			return duration;
		}
		return randomDuration.randomValue;
	}

	public void SetRandomDuration(float min, float max, bool useRandomValue = true)
	{
		useRandomDuration = useRandomValue;
		min = Mathf.Max(0f, min);
		max = Mathf.Max(0f, max);
		randomDuration = new RandomFloat(min, max);
		if (Mathf.Approximately(min, max))
		{
			useRandomDuration = false;
			duration = min;
		}
	}

	public int GetLoops()
	{
		if (!useRandomLoops)
		{
			return loops;
		}
		return randomLoops.randomValue;
	}

	public void SetRandomLoops(int min, int max, bool useRandomValue = true)
	{
		useRandomLoops = useRandomValue;
		min = Mathf.Max(0, min);
		max = Mathf.Max(1, max);
		randomLoops = new RandomInt(min, max);
		if (min == max)
		{
			useRandomLoops = false;
			loops = min;
		}
	}

	public float GetLoopDelay()
	{
		if (!useRandomLoopDelay)
		{
			return loopDelay;
		}
		return randomLoopDelay.randomValue;
	}

	public void SetRandomLoopDelay(float min, float max, bool useRandomValue = true)
	{
		useRandomLoopDelay = useRandomValue;
		min = Mathf.Max(0f, min);
		max = Mathf.Max(0f, max);
		randomLoopDelay = new RandomFloat(min, max);
		if (Mathf.Approximately(min, max))
		{
			useRandomLoopDelay = false;
			loopDelay = min;
		}
	}

	public ReactionSettings()
	{
		Reset();
	}

	public ReactionSettings(ReactionSettings other)
	{
		Reset();
		playMode = other.playMode;
		curve = other.curve;
		ease = other.ease;
		easeMode = other.easeMode;
		startDelay = other.startDelay;
		duration = other.duration;
		loops = other.loops;
		loopDelay = other.loopDelay;
		strength = other.strength;
		vibration = other.vibration;
		elasticity = other.elasticity;
		UseRandomStartDelay = other.useRandomStartDelay;
		UseRandomDuration = other.useRandomDuration;
		UseRandomLoops = other.useRandomLoops;
		UseRandomLoopDelay = other.UseRandomLoopDelay;
		RandomStartDelay = new RandomFloat(other.randomStartDelay);
		RandomDuration = new RandomFloat(other.randomDuration);
		RandomLoops = new RandomInt(other.randomLoops);
		RandomLoopDelay = new RandomFloat(other.randomLoopDelay);
	}

	public void Reset()
	{
		playMode = PlayMode.Normal;
		curve = AnimationCurve.Linear(0f, 0f, 1f, 1f);
		ease = Ease.Easy;
		easeMode = EaseMode.Ease;
		startDelay = 0f;
		duration = 1f;
		loops = 0;
		loopDelay = 0f;
		strength = 1f;
		vibration = 8;
		elasticity = 1f;
		UseRandomStartDelay = false;
		UseRandomDuration = false;
		UseRandomLoops = false;
		UseRandomLoopDelay = false;
		RandomStartDelay.Reset();
		RandomDuration.Reset();
		RandomLoops.Reset();
		RandomLoopDelay.Reset();
	}

	public void Validate()
	{
		StartDelay = startDelay;
		Duration = duration;
		Loops = loops;
		LoopDelay = loopDelay;
		Strength = strength;
		Vibration = vibration;
		Elasticity = elasticity;
	}

	public float CalculateEasedProgress(float progress)
	{
		return easeMode switch
		{
			EaseMode.Ease => easing.Evaluate(progress), 
			EaseMode.AnimationCurve => curve.Evaluate(progress), 
			_ => throw new ArgumentOutOfRangeException(), 
		};
	}
}
