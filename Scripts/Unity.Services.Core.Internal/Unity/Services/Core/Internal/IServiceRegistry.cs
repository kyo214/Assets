using JetBrains.Annotations;

namespace Unity.Services.Core.Internal;

internal interface IServiceRegistry
{
	void RegisterService<T>([NotNull] T service);

	T GetService<T>();
}
