using System.Collections.Generic;
using UnityEngine;

namespace MoreMountains.Tools;

[AddComponentMenu("More Mountains/Tools/Activation/MMApplicationPlatformActivation")]
public class MMApplicationPlatformActivation : MonoBehaviour
{
	public enum ExecutionTimes
	{
		Awake = 0,
		Start = 1,
		OnEnable = 2
	}

	[Header("Settings")]
	public ExecutionTimes ExecutionTime;

	public bool DebugToTheConsole;

	[Header("Platforms")]
	public List<PlatformBindings> Platforms;

	protected virtual void OnEnable()
	{
		if (ExecutionTime == ExecutionTimes.OnEnable)
		{
			Process();
		}
	}

	protected virtual void Awake()
	{
		if (ExecutionTime == ExecutionTimes.Awake)
		{
			Process();
		}
	}

	protected virtual void Start()
	{
		if (ExecutionTime == ExecutionTimes.Start)
		{
			Process();
		}
	}

	protected virtual void Process()
	{
		foreach (PlatformBindings platform in Platforms)
		{
			if (platform.Platform == Application.platform)
			{
				DisableIfNeeded(platform.PlatformAction, platform.Platform.ToString());
			}
		}
		_ = Application.platform;
		_ = 11;
	}

	protected virtual void DisableIfNeeded(PlatformBindings.PlatformActions platform, string platformName)
	{
		if (base.gameObject.activeInHierarchy && platform == PlatformBindings.PlatformActions.Disable)
		{
			base.gameObject.SetActive(value: false);
			if (DebugToTheConsole)
			{
				Debug.LogFormat(base.gameObject.name + " got disabled via MMPlatformActivation, platform : " + platformName + ".");
			}
		}
	}
}
