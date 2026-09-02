using System;
using System.Threading.Tasks;
using Unity.Services.Core.Environments;
using Unity.Services.Core.Internal;
using UnityEngine;

namespace Unity.Services.Core.Components;

[AddComponentMenu("Services/Services Initialization")]
public class ServicesInitialization : ServicesBehaviour
{
	[Header("Automation")]
	[Tooltip("This will attempt to initialize the services in Start().")]
	[SerializeField]
	public bool InitializeOnStart;

	[SerializeField]
	[Tooltip("Use this to set a custom environment in the initialization options. Defaults to the environment defined in the project settings or production.")]
	[Visibility("InitializeOnStart", true)]
	public bool UseCustomEnvironment;

	[SerializeField]
	[Tooltip("Choose the environment name to pass in the initialization options. You can configure environments in the unity dashboard.")]
	[Visibility("UseCustomEnvironment", true)]
	public string EnvironmentName = "production";

	[Header("Events")]
	[SerializeField]
	public ServicesInitializationEvents Events = new ServicesInitializationEvents();

	internal bool IsSetupDone { get; private set; }

	internal ServicesInitialization()
	{
	}

	protected override async void OnServicesReady()
	{
		await SetupAsync();
	}

	protected override void OnServicesInitialized()
	{
	}

	protected override void Cleanup()
	{
		if (base.Services != null)
		{
			base.Services.Initialized -= OnInitialized;
			base.Services.InitializeFailed -= OnInitializeFailed;
		}
	}

	internal async Task SetupAsync()
	{
		if (base.Services.State != ServicesInitializationState.Initialized)
		{
			base.Services.Initialized -= OnInitialized;
			base.Services.Initialized += OnInitialized;
			base.Services.InitializeFailed -= OnInitializeFailed;
			base.Services.InitializeFailed += OnInitializeFailed;
		}
		if (base.Services.State == ServicesInitializationState.Uninitialized && InitializeOnStart)
		{
			await InitializeOnStartAsync();
		}
		IsSetupDone = true;
	}

	internal async Task InitializeOnStartAsync()
	{
		if (base.Services == null)
		{
			Events?.InitializeFailed?.Invoke(new Exception("Trying to initiliaze services before the registry is set."));
			return;
		}
		try
		{
			await base.Services.InitializeAsync(BuildInitializationOptions());
		}
		catch (Exception)
		{
		}
	}

	internal InitializationOptions BuildInitializationOptions()
	{
		InitializationOptions initializationOptions = new InitializationOptions();
		if (UseCustomEnvironment)
		{
			initializationOptions.SetEnvironmentName(EnvironmentName);
		}
		return initializationOptions;
	}

	private void OnInitialized()
	{
		Events?.Initialized?.Invoke();
	}

	private void OnInitializeFailed(Exception e)
	{
		Events?.InitializeFailed?.Invoke(e);
	}
}
