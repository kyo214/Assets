using JetBrains.Annotations;

namespace Unity.Services.Core.Internal;

public sealed class CoreRegistry
{
	public static CoreRegistry Instance { get; internal set; }

	public string InstanceId { get; }

	internal ServicesType Type { get; private set; }

	internal InitializationOptions Options { get; set; }

	[NotNull]
	internal IPackageRegistry PackageRegistry { get; private set; }

	[NotNull]
	internal IComponentRegistry ComponentRegistry { get; private set; }

	[NotNull]
	internal IServiceRegistry ServiceRegistry { get; private set; }

	internal CoreRegistry()
	{
		Type = ServicesType.Default;
		InstanceId = null;
		PackageRegistry = new PackageRegistry(new DependencyTree());
		ComponentRegistry = new ComponentRegistry();
		ServiceRegistry = new ServiceRegistry();
	}

	internal CoreRegistry(IPackageRegistry packageRegistry, ServicesType type = ServicesType.Default, string instanceId = null)
	{
		Type = type;
		InstanceId = instanceId;
		PackageRegistry = packageRegistry;
		ComponentRegistry = new ComponentRegistry();
		ServiceRegistry = new ServiceRegistry();
	}

	public CoreRegistration RegisterPackage<TPackage>([NotNull] TPackage package) where TPackage : IInitializablePackage
	{
		return PackageRegistry.RegisterPackage(package);
	}

	public void RegisterServiceComponent<TComponent>([NotNull] TComponent component) where TComponent : IServiceComponent
	{
		ComponentRegistry.RegisterServiceComponent(component);
	}

	public TComponent GetServiceComponent<TComponent>() where TComponent : IServiceComponent
	{
		return ComponentRegistry.GetServiceComponent<TComponent>();
	}

	public bool TryGetServiceComponent<TComponent>(out TComponent component) where TComponent : IServiceComponent
	{
		return ComponentRegistry.TryGetServiceComponent<TComponent>(out component);
	}

	public void RegisterService<T>([NotNull] T service)
	{
		ServiceRegistry.RegisterService(service);
	}

	public T GetService<T>()
	{
		return ServiceRegistry.GetService<T>();
	}

	internal void LockComponentRegistration()
	{
		if (!(ComponentRegistry is LockedComponentRegistry))
		{
			ComponentRegistry = new LockedComponentRegistry(ComponentRegistry);
		}
	}

	internal void LockServiceRegistration()
	{
		if (!(ServiceRegistry is LockedServiceRegistry))
		{
			ServiceRegistry = new LockedServiceRegistry(ServiceRegistry);
		}
	}
}
