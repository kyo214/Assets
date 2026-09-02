using Doozy.Runtime.Common;
using Doozy.Runtime.Common.Attributes;
using Doozy.Runtime.Reactor.ScriptableObjects;
using UnityEngine;

namespace Doozy.Runtime.Reactor.Ticker;

public class RuntimeTicker : SingletonBehaviour<RuntimeTicker>
{
	[ClearOnReload]
	private static TickService s_service;

	private static double s_elapsedTime;

	private static double s_lastTickTime;

	public static TickService service
	{
		get
		{
			if (SingletonBehaviour<RuntimeTicker>.applicationIsQuitting)
			{
				return null;
			}
			if (SingletonBehaviour<RuntimeTicker>.instance == null)
			{
				return null;
			}
			if (!SingletonBehaviour<RuntimeTicker>.instance.initialized)
			{
				SingletonBehaviour<RuntimeTicker>.instance.Initialize();
			}
			return s_service ?? (s_service = new TickService(ReactorSettings.runtimeFPS));
		}
	}

	public static float timeSinceStartup => Time.realtimeSinceStartup;

	private static float tickInterval => service.tickInterval;

	private bool initialized { get; set; }

	[ExecuteOnReload]
	private static void OnReload()
	{
		ResetTime();
	}

	private void Initialize()
	{
		if (!initialized)
		{
			initialized = true;
			ResetTime();
		}
	}

	private static void ResetTime()
	{
		s_elapsedTime = 0.0;
		s_lastTickTime = timeSinceStartup;
	}

	private void Update()
	{
		if (!service.hasRegisteredTargets)
		{
			ResetTime();
			return;
		}
		s_elapsedTime += (double)timeSinceStartup - s_lastTickTime;
		s_lastTickTime = timeSinceStartup;
		if (!(tickInterval < (float)TickService.maxFPS) || !(s_elapsedTime < (double)tickInterval))
		{
			s_elapsedTime = 0.0;
			service.Tick();
		}
	}
}
