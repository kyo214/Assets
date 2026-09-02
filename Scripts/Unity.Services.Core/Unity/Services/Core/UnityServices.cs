using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using UnityEngine;

namespace Unity.Services.Core;

public static class UnityServices
{
	internal static ExternalUserIdProperty ExternalUserIdProperty = new ExternalUserIdProperty();

	public static IUnityServices Instance { get; set; }

	public static IReadOnlyDictionary<string, IUnityServices> Services => s_Services;

	internal static TaskCompletionSource<object> InstantiationCompletion { get; set; }

	private static Dictionary<string, IUnityServices> s_Services { get; } = new Dictionary<string, IUnityServices>();

	public static ServicesInitializationState State
	{
		get
		{
			if (!UnityThreadUtils.IsRunningOnUnityThread)
			{
				throw new ServicesInitializationException("You are attempting to access UnityServices.State from a non-Unity Thread. UnityServices.State can only be accessed from Unity Thread");
			}
			if (Instance != null)
			{
				return Instance.State;
			}
			TaskCompletionSource<object> instantiationCompletion = InstantiationCompletion;
			if (instantiationCompletion != null && instantiationCompletion.Task.Status == TaskStatus.WaitingForActivation)
			{
				return ServicesInitializationState.Initializing;
			}
			return ServicesInitializationState.Uninitialized;
		}
	}

	public static string ExternalUserId
	{
		get
		{
			return ExternalUserIdProperty.UserId;
		}
		set
		{
			ExternalUserIdProperty.UserId = value;
		}
	}

	public static event Action Initialized
	{
		add
		{
			if (Instance != null)
			{
				Instance.Initialized += value;
			}
		}
		remove
		{
			if (Instance != null)
			{
				Instance.Initialized -= value;
			}
		}
	}

	public static event Action<Exception> InitializeFailed
	{
		add
		{
			if (Instance != null)
			{
				Instance.InitializeFailed += value;
			}
		}
		remove
		{
			if (Instance != null)
			{
				Instance.InitializeFailed -= value;
			}
		}
	}

	public static Task InitializeAsync()
	{
		return InitializeAsync(new InitializationOptions());
	}

	[System.Runtime.CompilerServices.PreserveDependency("Register()", "Unity.Services.Core.Registration.CorePackageInitializer", "Unity.Services.Core.Registration")]
	[System.Runtime.CompilerServices.PreserveDependency("CreateStaticInstance()", "Unity.Services.Core.Internal.UnityServicesInitializer", "Unity.Services.Core.Internal")]
	[System.Runtime.CompilerServices.PreserveDependency("EnableServicesInitializationAsync()", "Unity.Services.Core.Internal.UnityServicesInitializer", "Unity.Services.Core.Internal")]
	[System.Runtime.CompilerServices.PreserveDependency("CaptureUnityThreadInfo()", "Unity.Services.Core.UnityThreadUtils", "Unity.Services.Core")]
	public static async Task InitializeAsync(InitializationOptions options)
	{
		if (!UnityThreadUtils.IsRunningOnUnityThread)
		{
			throw new ServicesInitializationException("You are attempting to initialize Unity Services from a non-Unity Thread. Unity Services can only be initialized from Unity Thread");
		}
		if (!Application.isPlaying)
		{
			throw new ServicesInitializationException("You are attempting to initialize Unity Services in Edit Mode. Unity Services can only be initialized in Play Mode");
		}
		if (Instance == null)
		{
			if (InstantiationCompletion == null)
			{
				InstantiationCompletion = new TaskCompletionSource<object>();
			}
			await InstantiationCompletion.Task;
		}
		await Instance.InitializeAsync(options);
	}

	public static IUnityServices CreateServices()
	{
		return CreateServices(Guid.NewGuid().ToString());
	}

	public static IUnityServices CreateServices(string servicesId)
	{
		if (string.IsNullOrEmpty(servicesId))
		{
			throw new ArgumentException("The services identifier cannot be null or empty");
		}
		if (s_Services.ContainsKey(servicesId))
		{
			throw new ServicesCreationException("The services identifier '" + servicesId + "' is already registered.");
		}
		IUnityServices unityServices = UnityServicesBuilder.Create(servicesId);
		s_Services[servicesId] = unityServices;
		return unityServices;
	}

	internal static void ClearServices()
	{
		s_Services.Clear();
	}
}
