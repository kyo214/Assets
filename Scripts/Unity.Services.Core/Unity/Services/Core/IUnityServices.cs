using System;
using System.Threading.Tasks;

namespace Unity.Services.Core;

public interface IUnityServices
{
	ServicesInitializationState State { get; }

	event Action Initialized;

	event Action<Exception> InitializeFailed;

	Task InitializeAsync(InitializationOptions options = null);

	string GetIdentifier()
	{
		return null;
	}

	T GetService<T>();
}
