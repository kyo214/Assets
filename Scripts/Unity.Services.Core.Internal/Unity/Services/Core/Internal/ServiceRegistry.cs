using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace Unity.Services.Core.Internal;

internal class ServiceRegistry : IServiceRegistry
{
	[NotNull]
	internal Dictionary<int, object> ServiceTypeHashToInstance { get; }

	public ServiceRegistry()
	{
		ServiceTypeHashToInstance = new Dictionary<int, object>();
	}

	public ServiceRegistry([NotNull] Dictionary<int, object> serviceTypeHashToInstance)
	{
		ServiceTypeHashToInstance = serviceTypeHashToInstance;
	}

	public void RegisterService<T>(T service)
	{
		Type typeFromHandle = typeof(T);
		if (service.GetType() == typeFromHandle)
		{
			throw new ArgumentException("Interface type of service not specified.");
		}
		int hashCode = typeFromHandle.GetHashCode();
		ServiceTypeHashToInstance[hashCode] = service;
	}

	public T GetService<T>()
	{
		Type typeFromHandle = typeof(T);
		if (!ServiceTypeHashToInstance.TryGetValue(typeFromHandle.GetHashCode(), out var value))
		{
			return default;
		}
		return (T)value;
	}
}
