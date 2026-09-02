using Doozy.Runtime.Common.Attributes;
using UnityEngine;

namespace Doozy.Runtime.UIManager;

public static class UISettings
{
	[ClearOnReload(0)]
	private static int s_interactionsDisableLevel;

	[ClearOnReload(false)]
	private static bool initialized { get; set; }

	public static bool interactionsDisabled => s_interactionsDisableLevel > 0;

	private static void Initialize()
	{
		if (!initialized)
		{
			initialized = true;
		}
	}

	public static void EnableUIInteractions(bool byForce = false)
	{
		s_interactionsDisableLevel = ((!byForce) ? Mathf.Max(0, s_interactionsDisableLevel - 1) : 0);
	}

	public static void DisableUIInteractions()
	{
		s_interactionsDisableLevel++;
	}
}
