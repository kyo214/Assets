using System;
using UnityEngine;

namespace MoreMountains.Tools;

[Serializable]
public class PlatformBindings
{
	public enum PlatformActions
	{
		DoNothing = 0,
		Disable = 1
	}

	public RuntimePlatform Platform = RuntimePlatform.WindowsPlayer;

	public PlatformActions PlatformAction;
}
