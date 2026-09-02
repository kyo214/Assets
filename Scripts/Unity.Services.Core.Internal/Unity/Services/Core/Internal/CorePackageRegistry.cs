using JetBrains.Annotations;

namespace Unity.Services.Core.Internal;

public sealed class CorePackageRegistry
{
	public static CorePackageRegistry Instance { get; internal set; }

	internal IPackageRegistry Registry { get; set; }

	internal CorePackageRegistry()
	{
		Registry = new PackageRegistry(new DependencyTree());
	}

	internal CorePackageRegistry(IPackageRegistry registry)
	{
		Registry = registry;
	}

	public CoreRegistration Register<TPackage>([NotNull] TPackage package) where TPackage : IInitializablePackage
	{
		return Registry.RegisterPackage(package);
	}

	internal void Lock()
	{
		if (!(Registry is LockedPackageRegistry))
		{
			Registry = new LockedPackageRegistry(Registry);
		}
	}
}
