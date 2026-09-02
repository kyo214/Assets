using System;
using System.Collections.Generic;
using UnityEngine;

namespace Chronos;

[DisallowMultipleComponent]
[HelpURL("http://ludiq.io/chronos/documentation#Timekeeper")]
public class Timekeeper : Singleton<Timekeeper>
{
	public const int DefaultMaxParticleLoops = 10;

	[SerializeField]
	private bool _debug;

	[SerializeField]
	private int _maxParticleLoops = 10;

	protected Dictionary<string, GlobalClock> _clocks;

	public bool debug
	{
		get
		{
			return _debug;
		}
		set
		{
			_debug = value;
		}
	}

	public int maxParticleLoops
	{
		get
		{
			return _maxParticleLoops;
		}
		set
		{
			_maxParticleLoops = value;
		}
	}

	public IEnumerable<GlobalClock> clocks => _clocks.Values;

	internal static float unscaledDeltaTime
	{
		get
		{
			if (Time.frameCount <= 2)
			{
				return 0.02f;
			}
			return Mathf.Min(Time.unscaledDeltaTime, Time.maximumDeltaTime);
		}
	}

	public Timekeeper()
		: base(false, false)
	{
		_clocks = new Dictionary<string, GlobalClock>();
	}

	protected virtual void Awake()
	{
		GlobalClock[] components = GetComponents<GlobalClock>();
		foreach (GlobalClock globalClock in components)
		{
			_clocks.Add(globalClock.key, globalClock);
		}
	}

	public virtual bool HasClock(string key)
	{
		if (key == null)
		{
			throw new ArgumentNullException("key");
		}
		return _clocks.ContainsKey(key);
	}

	public virtual GlobalClock Clock(string key)
	{
		if (key == null)
		{
			throw new ArgumentNullException("key");
		}
		if (!HasClock(key))
		{
			throw new ChronosException($"Unknown global clock '{key}'.");
		}
		return _clocks[key];
	}

	public virtual GlobalClock AddClock(string key)
	{
		if (key == null)
		{
			throw new ArgumentNullException("key");
		}
		if (HasClock(key))
		{
			throw new ChronosException($"Global clock '{key}' already exists.");
		}
		GlobalClock globalClock = base.gameObject.AddComponent<GlobalClock>();
		globalClock.key = key;
		_clocks.Add(key, globalClock);
		return globalClock;
	}

	public virtual void RemoveClock(string key)
	{
		if (key == null)
		{
			throw new ArgumentNullException("key");
		}
		if (!HasClock(key))
		{
			throw new ChronosException($"Unknown global clock '{key}'.");
		}
		_clocks.Remove(key);
	}

	internal static TimeState GetTimeState(float timeScale)
	{
		if (timeScale < 0f)
		{
			return TimeState.Reversed;
		}
		if (timeScale == 0f)
		{
			return TimeState.Paused;
		}
		if (timeScale < 1f)
		{
			return TimeState.Slowed;
		}
		if (timeScale == 1f)
		{
			return TimeState.Normal;
		}
		return TimeState.Accelerated;
	}
}
