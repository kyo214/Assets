using System;

namespace UnityEngine.XR.Management;

internal static class XRManagementAnalytics
{
	[Serializable]
	private struct BuildEvent
	{
		public string buildGuid;

		public string buildTarget;

		public string buildTargetGroup;

		public string[] assigned_loaders;
	}

	private const int kMaxEventsPerHour = 1000;

	private const int kMaxNumberOfElements = 1000;

	private const string kVendorKey = "unity.xrmanagement";

	private const string kEventBuild = "xrmanagment_build";

	private static bool s_Initialized;

	private static bool Initialize()
	{
		return s_Initialized;
	}
}
