using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace Unity.Services.Core.Internal;

internal class UnityServicesInternal : IUnityServices
{
	internal const string InitSuccessEventInvocationError = "Exception in services initialization success event handler: ";

	internal const string InitFailureEventInvocationError = "Exception in services initialization failure event handler: ";

	internal bool CanInitialize;

	private TaskCompletionSource<object> m_Initialization;

	public ServicesInitializationState State { get; private set; }

	public InitializationOptions Options
	{
		get
		{
			return Registry.Options;
		}
		internal set
		{
			Registry.Options = value;
		}
	}

	[NotNull]
	internal CoreRegistry Registry { get; }

	[NotNull]
	internal CoreMetrics Metrics { get; }

	[NotNull]
	internal CoreDiagnostics Diagnostics { get; }

	public event Action Initialized;

	public event Action<Exception> InitializeFailed;

	public UnityServicesInternal([NotNull] CoreRegistry registry, [NotNull] CoreMetrics coreMetrics, [NotNull] CoreDiagnostics coreDiagnostics)
	{
		Registry = registry;
		Metrics = coreMetrics;
		Diagnostics = coreDiagnostics;
	}

	public async Task InitializeAsync(InitializationOptions options)
	{
		_ = 1;
		try
		{
			if (options == null)
			{
				options = new InitializationOptions();
			}
			if (!HasRequestedInitialization() || HasInitializationFailed())
			{
				Registry.Options = options;
				m_Initialization = new TaskCompletionSource<object>();
			}
			if (CanInitialize && State == ServicesInitializationState.Uninitialized)
			{
				await InitializeServicesAsync();
			}
			else
			{
				await m_Initialization.Task;
			}
			TriggerInitializeSuccess();
		}
		catch (Exception initException)
		{
			TriggerInitializeFailed(initException);
			throw;
		}
		bool HasInitializationFailed()
		{
			if (m_Initialization.Task.IsCompleted)
			{
				return m_Initialization.Task.Status != TaskStatus.RanToCompletion;
			}
			return false;
		}
	}

	public string GetIdentifier()
	{
		return Registry.InstanceId;
	}

	private void TriggerInitializeSuccess()
	{
		try
		{
			Initialized?.Invoke();
		}
		catch (Exception arg)
		{
			CoreLogger.LogError(string.Format("{0} {1}", "Exception in services initialization success event handler: ", arg));
		}
	}

	private void TriggerInitializeFailed(Exception initException)
	{
		try
		{
			InitializeFailed?.Invoke(initException);
		}
		catch (Exception arg)
		{
			CoreLogger.LogError(string.Format("{0} {1}", "Exception in services initialization failure event handler: ", arg));
		}
	}

	public T GetService<T>()
	{
		return Registry.GetService<T>();
	}

	private bool HasRequestedInitialization()
	{
		return m_Initialization != null;
	}

	private async Task InitializeServicesAsync()
	{
		State = ServicesInitializationState.Initializing;
		Stopwatch initStopwatch = new Stopwatch();
		initStopwatch.Start();
		DependencyTree dependencyTree = Registry.PackageRegistry.Tree;
		if (dependencyTree == null)
		{
			NullReferenceException ex = new NullReferenceException("Services require a valid dependency tree to be initialized.");
			FailServicesInitialization(ex);
			throw ex;
		}
		List<int> sortedPackageTypeHashes = new List<int>(dependencyTree.PackageTypeHashToInstance.Count);
		try
		{
			SortPackages();
			await InitializePackagesAsync();
		}
		catch (Exception reason)
		{
			FailServicesInitialization(reason);
			throw;
		}
		SucceedServicesInitialization();
		void FailServicesInitialization(Exception exception)
		{
			State = ServicesInitializationState.Uninitialized;
			initStopwatch.Stop();
			m_Initialization.TrySetException(exception);
		}
		async Task InitializePackagesAsync()
		{
			await new CoreRegistryInitializer(Registry, sortedPackageTypeHashes).InitializeRegistryAsync();
		}
		void SortPackages()
		{
			new DependencyTreeInitializeOrderSorter(dependencyTree, sortedPackageTypeHashes).SortRegisteredPackagesIntoTarget();
		}
		void SucceedServicesInitialization()
		{
			State = ServicesInitializationState.Initialized;
			Registry.LockComponentRegistration();
			initStopwatch.Stop();
			m_Initialization.TrySetResult(null);
		}
	}

	internal void SendInitializationMetrics(List<PackageInitializationInfo> packageInitInfos)
	{
		foreach (PackageInitializationInfo packageInitInfo in packageInitInfos)
		{
			Metrics.SendInitTimeMetricForPackage(packageInitInfo.PackageType, packageInitInfo.InitializationTimeInSeconds);
		}
	}

	internal void EnableInitialization()
	{
		CanInitialize = true;
	}

	internal async Task EnableInitializationAsync()
	{
		CanInitialize = true;
		CorePackageRegistry.Instance.Lock();
		if (HasRequestedInitialization())
		{
			await InitializeServicesAsync();
		}
	}
}
