using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Newtonsoft.Json.Linq;
using Unity.Services.Core.Configuration;
using Unity.Services.Core.Configuration.Internal;
using Unity.Services.Core.Device;
using Unity.Services.Core.Device.Internal;
using Unity.Services.Core.Environments.Internal;
using Unity.Services.Core.Internal;
using Unity.Services.Core.Internal.Serialization;
using Unity.Services.Core.Scheduler.Internal;
using Unity.Services.Core.Telemetry.Internal;
using Unity.Services.Core.Threading.Internal;
using UnityEngine;

namespace Unity.Services.Core.Registration;

internal class CorePackageInitializer : IInitializablePackageV2, IInitializablePackage, IDiagnosticsComponentProvider
{
	internal const string CorePackageName = "com.unity.services.core";

	internal const string ProjectUnlinkMessage = "To use Unity's dashboard services, you need to link your Unity project to a project ID. To do this, go to Project Settings to select your organization, select your project and then link a project ID. You also need to make sure your organization has access to the required products. Visit https://dashboard.unity3d.com to sign up.";

	private CoreRegistry m_Registry;

	private readonly IJsonSerializer m_Serializer;

	private InitializationOptions m_CurrentInitializationOptions;

	internal ActionScheduler ActionScheduler { get; private set; }

	internal InstallationId InstallationId { get; private set; }

	internal ProjectConfiguration ProjectConfig { get; private set; }

	internal Unity.Services.Core.Environments.Internal.Environments Environments { get; private set; }

	internal ExternalUserId ExternalUserId { get; private set; }

	internal ICloudProjectId CloudProjectId { get; private set; }

	internal IDiagnosticsFactory DiagnosticsFactory { get; private set; }

	internal IMetricsFactory MetricsFactory { get; private set; }

	internal UnityThreadUtilsInternal UnityThreadUtils { get; private set; }

	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
	private static void InitializeOnLoad()
	{
		new CorePackageInitializer(new NewtonsoftSerializer()).Register(CorePackageRegistry.Instance);
	}

	public void Register(CorePackageRegistry registry)
	{
		CoreDiagnostics.Instance.DiagnosticsComponentProvider = this;
		registry.Register(this).ProvidesComponent<IInstallationId>().ProvidesComponent<ICloudProjectId>()
			.ProvidesComponent<IActionScheduler>()
			.ProvidesComponent<IEnvironments>()
			.ProvidesComponent<IProjectConfiguration>()
			.ProvidesComponent<IMetricsFactory>()
			.ProvidesComponent<IDiagnosticsFactory>()
			.ProvidesComponent<IUnityThreadUtils>()
			.ProvidesComponent<IExternalUserId>();
	}

	public CorePackageInitializer()
	{
		m_Serializer = new NewtonsoftSerializer();
	}

	public CorePackageInitializer(IJsonSerializer serializer)
	{
		m_Serializer = serializer;
	}

	public Task Initialize(CoreRegistry registry)
	{
		m_Registry = registry;
		return InitializeComponents();
	}

	public Task InitializeInstanceAsync(CoreRegistry registry)
	{
		m_Registry = registry;
		return InitializeComponents();
	}

	private async Task InitializeComponents()
	{
		try
		{
			if (HaveInitOptionsChanged())
			{
				FreeOptionsDependantComponents();
			}
			InitializeInstallationId();
			InitializeActionScheduler();
			await InitializeProjectConfigAsync(m_Registry.Options);
			InitializeExternalUserId(ProjectConfig);
			InitializeEnvironments(ProjectConfig);
			InitializeCloudProjectId();
			if (string.IsNullOrEmpty(CloudProjectId.GetCloudProjectId()))
			{
				throw new UnityProjectNotLinkedException("To use Unity's dashboard services, you need to link your Unity project to a project ID. To do this, go to Project Settings to select your organization, select your project and then link a project ID. You also need to make sure your organization has access to the required products. Visit https://dashboard.unity3d.com to sign up.");
			}
			InitializeMetrics();
			InitializeDiagnostics();
			InitializeUnityThreadUtils();
			RegisterProvidedComponents();
		}
		catch (Exception reason) when (SendFailedInitDiagnostic(reason))
		{
		}
		void RegisterProvidedComponents()
		{
			m_Registry.RegisterServiceComponent((IInstallationId)InstallationId);
			m_Registry.RegisterServiceComponent((IActionScheduler)ActionScheduler);
			m_Registry.RegisterServiceComponent((IProjectConfiguration)ProjectConfig);
			m_Registry.RegisterServiceComponent((IEnvironments)Environments);
			m_Registry.RegisterServiceComponent(MetricsFactory);
			m_Registry.RegisterServiceComponent(DiagnosticsFactory);
			m_Registry.RegisterServiceComponent(CloudProjectId);
			m_Registry.RegisterServiceComponent((IUnityThreadUtils)UnityThreadUtils);
			m_Registry.RegisterServiceComponent((IExternalUserId)ExternalUserId);
		}
		static bool SendFailedInitDiagnostic(Exception ex)
		{
			return false;
		}
	}

	private bool HaveInitOptionsChanged()
	{
		if (m_CurrentInitializationOptions != null)
		{
			return !m_CurrentInitializationOptions.Values.ValueEquals(m_Registry.Options.Values);
		}
		return false;
	}

	private void FreeOptionsDependantComponents()
	{
		ProjectConfig = null;
		Environments = null;
		DiagnosticsFactory = null;
		MetricsFactory = null;
	}

	internal void InitializeInstallationId()
	{
		if (InstallationId == null)
		{
			InstallationId installationId = new InstallationId();
			installationId.CreateIdentifier();
			InstallationId = installationId;
		}
	}

	internal void InitializeActionScheduler()
	{
		if (ActionScheduler == null)
		{
			ActionScheduler actionScheduler = new ActionScheduler();
			actionScheduler.JoinPlayerLoopSystem();
			ActionScheduler = actionScheduler;
		}
	}

	internal async Task InitializeProjectConfigAsync([NotNull] InitializationOptions options)
	{
		if (ProjectConfig == null)
		{
			ProjectConfig = await GenerateProjectConfigurationAsync(options);
			m_CurrentInitializationOptions = new InitializationOptions(options);
		}
	}

	internal async Task<ProjectConfiguration> GenerateProjectConfigurationAsync([NotNull] InitializationOptions options)
	{
		SerializableProjectConfiguration config = await GetSerializedConfigOrEmptyAsync();
		if (config.Keys == null || config.Values == null)
		{
			config = SerializableProjectConfiguration.Empty;
		}
		Dictionary<string, ConfigurationEntry> dictionary = new Dictionary<string, ConfigurationEntry>(config.Keys.Length);
		dictionary.FillWith(config);
		dictionary.FillWith(options);
		return new ProjectConfiguration(dictionary, m_Serializer);
	}

	internal static async Task<SerializableProjectConfiguration> GetSerializedConfigOrEmptyAsync()
	{
		try
		{
			return await ConfigurationUtils.ConfigurationLoader.GetConfigAsync();
		}
		catch (Exception ex)
		{
			CoreLogger.LogError("An error occured while trying to get the project configuration for services.\n" + ex.Message + "\n" + ex.StackTrace);
			return SerializableProjectConfiguration.Empty;
		}
	}

	internal void InitializeExternalUserId(IProjectConfiguration projectConfiguration)
	{
		if (UnityServices.ExternalUserId == null)
		{
			string text = projectConfiguration.GetString("com.unity.services.core.analytics-user-id");
			if (!string.IsNullOrEmpty(text))
			{
				UnityServices.ExternalUserId = text;
			}
		}
		if (ExternalUserId == null)
		{
			ExternalUserId = new ExternalUserId();
		}
	}

	internal void InitializeEnvironments(IProjectConfiguration projectConfiguration)
	{
		if (Environments == null)
		{
			string current = projectConfiguration.GetString("com.unity.services.core.environment-name", "production");
			Environments = new Unity.Services.Core.Environments.Internal.Environments
			{
				Current = current
			};
		}
	}

	internal void InitializeMetrics()
	{
		if (MetricsFactory == null)
		{
			MetricsFactory = new MetricsFactory();
		}
	}

	internal void InitializeDiagnostics()
	{
		if (DiagnosticsFactory == null)
		{
			DiagnosticsFactory = new DiagnosticsFactory();
		}
	}

	internal void InitializeCloudProjectId(ICloudProjectId cloudProjectId = null)
	{
		if (CloudProjectId == null)
		{
			CloudProjectId = cloudProjectId ?? new CloudProjectId();
		}
	}

	internal void InitializeUnityThreadUtils()
	{
		if (UnityThreadUtils == null)
		{
			UnityThreadUtils = new UnityThreadUtilsInternal();
		}
	}

	public async Task<IDiagnosticsFactory> CreateDiagnosticsComponents()
	{
		if (HaveInitOptionsChanged())
		{
			FreeOptionsDependantComponents();
		}
		InitializeActionScheduler();
		await InitializeProjectConfigAsync(m_Registry.Options);
		InitializeEnvironments(ProjectConfig);
		InitializeCloudProjectId();
		return DiagnosticsFactory;
	}

	[Conditional("ENABLE_UNITY_SERVICES_CORE_VERBOSE_LOGGING")]
	private void LogInitializationInfoJson()
	{
		JObject jObject = new JObject();
		JObject jObject2 = JObject.Parse(m_Serializer.SerializeObject(DiagnosticsFactory.CommonTags));
		JObject value = JObject.Parse(ProjectConfig.ToJson());
		JObject content = JObject.Parse("{\"installation_id\": \"" + InstallationId.Identifier + "\"}");
		jObject2.Merge(content);
		jObject.Add("CommonSettings", jObject2);
		jObject.Add("ServicesRuntimeSettings", value);
	}

	public async Task<string> GetSerializedProjectConfigurationAsync()
	{
		await InitializeProjectConfigAsync(m_Registry.Options);
		return ProjectConfig.ToJson();
	}
}
