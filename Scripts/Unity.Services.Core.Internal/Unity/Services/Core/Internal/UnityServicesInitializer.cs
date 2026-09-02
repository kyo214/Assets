using UnityEngine;

namespace Unity.Services.Core.Internal;

internal static class UnityServicesInitializer
{
	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
	private static void CreateStaticInstance()
	{
		UnityServices.ClearServices();
		UnityServicesBuilder.InstanceCreationDelegate = CreateInstance;
		CorePackageRegistry corePackageRegistry = new CorePackageRegistry();
		CoreRegistry coreRegistry = new CoreRegistry(corePackageRegistry.Registry);
		CorePackageRegistry.Instance = corePackageRegistry;
		CoreRegistry.Instance = coreRegistry;
		CoreMetrics coreMetrics = new CoreMetrics();
		CoreDiagnostics coreDiagnostics = new CoreDiagnostics();
		UnityServices.Instance = new UnityServicesInternal(coreRegistry, coreMetrics, coreDiagnostics);
		UnityServices.InstantiationCompletion?.TrySetResult(null);
		CoreMetrics.Instance = coreMetrics;
		CoreDiagnostics.Instance = coreDiagnostics;
	}

	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
	private static async void EnableServicesInitializationAsync()
	{
		await ((UnityServicesInternal)UnityServices.Instance).EnableInitializationAsync();
	}

	internal static IUnityServices CreateInstance(string servicesId)
	{
		UnityServicesInternal unityServicesInternal = new UnityServicesInternal(new CoreRegistry(CorePackageRegistry.Instance.Registry, ServicesType.Instance, servicesId), CoreMetrics.Instance, CoreDiagnostics.Instance);
		unityServicesInternal.EnableInitialization();
		return unityServicesInternal;
	}
}
