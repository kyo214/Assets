using System;
using JetBrains.Annotations;

namespace Unity.Services.Core.Internal;

internal class LockedServiceRegistry : IServiceRegistry
{
	private const string k_ErrorMessage = "Service registration has been locked. Make sure to register service services before all packages have finished initializing.";

	[NotNull]
	internal IServiceRegistry Registry { get; }

	public LockedServiceRegistry([NotNull] IServiceRegistry registryToLock)
	{
		Registry = registryToLock;
	}

	public void RegisterService<T>(T service)
	{
		throw new InvalidOperationException("Service registration has been locked. Make sure to register service services before all packages have finished initializing.");
	}

	public T GetService<T>()
	{
		return Registry.GetService<T>();
	}
}
