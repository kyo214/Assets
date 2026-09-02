using System.Collections.Generic;
using Unity.Services.Core.Internal;

namespace Unity.Services.Core.Telemetry.Internal;

internal class DiagnosticsFactory : IDiagnosticsFactory, IServiceComponent
{
	public IReadOnlyDictionary<string, string> CommonTags { get; } = new Dictionary<string, string>();

	public IDiagnostics Create(string packageName)
	{
		return new Diagnostics();
	}
}
