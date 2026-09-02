using Unity.Services.Core.Internal;
using UnityEngine;

namespace Unity.Services.Core.Components;

public abstract class ServicesBehaviour : MonoBehaviour
{
	[Header("Services Registry")]
	[Tooltip("Use this to setup a custom services registry. All services in a registry are unique.")]
	[SerializeField]
	public bool UseCustomServices;

	[SerializeField]
	[Tooltip("Unique local identifier for the custom set of services. Used as the key in the registries dictionary.")]
	[Visibility("UseCustomServices", true)]
	public string ServicesIdentifier;

	public IUnityServices Services { get; internal set; }

	internal virtual void Start()
	{
		SetRegistry();
		if (Services != null)
		{
			if (Services.State == ServicesInitializationState.Initialized)
			{
				OnServicesInitialized();
				return;
			}
			Services.Initialized -= OnServicesInitialized;
			Services.Initialized += OnServicesInitialized;
		}
	}

	internal virtual void OnDestroy()
	{
		if (Services != null)
		{
			Services.Initialized -= OnServicesInitialized;
		}
		Cleanup();
	}

	private void SetRegistry()
	{
		Services = ((!UseCustomServices) ? UnityServices.Instance : (UnityServices.Services.ContainsKey(ServicesIdentifier) ? UnityServices.Services[ServicesIdentifier] : UnityServices.CreateServices(ServicesIdentifier)));
		OnServicesReady();
	}

	protected abstract void OnServicesReady();

	protected abstract void OnServicesInitialized();

	protected abstract void Cleanup();
}
