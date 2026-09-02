using System.Threading.Tasks;

namespace Unity.Services.Core.Internal;

public interface IInitializablePackageV2 : IInitializablePackage
{
	void Register(CorePackageRegistry registry);

	Task InitializeInstanceAsync(CoreRegistry registry);
}
