#define ENABLE_MONO
#define NET_4_6
#define UNITY_2019_4_OR_NEWER
using UnityEngine;

namespace Fusion;

internal static class FusionRuntimeCheck
{
	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
	private static void RuntimeCheck()
	{
		RuntimeUnityFlagsSetup.Check_ENABLE_MONO();
		RuntimeUnityFlagsSetup.Check_NET_4_6();
		RuntimeUnityFlagsSetup.Check_UNITY_2019_4_OR_NEWER();
	}
}
