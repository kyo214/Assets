using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.Core.Telemetry.Internal;

namespace Unity.Services.Core.Internal;

internal class CoreDiagnostics
{
	internal const string CorePackageName = "com.unity.services.core";

	internal const string CircularDependencyDiagnosticName = "circular_dependency";

	internal const string CorePackageInitDiagnosticName = "core_package_init";

	internal const string OperateServicesInitDiagnosticName = "operate_services_init";

	internal const string ProjectConfigTagName = "project_config";

	public static CoreDiagnostics Instance { get; internal set; }

	public IDictionary<string, string> CoreTags { get; } = new Dictionary<string, string>();

	internal IDiagnosticsComponentProvider DiagnosticsComponentProvider { get; set; }

	internal IDiagnostics Diagnostics { get; set; }

	public void SetProjectConfiguration(string serializedProjectConfig)
	{
	}

	public void SendCircularDependencyDiagnostics(Exception exception)
	{
	}

	public void SendCorePackageInitDiagnostics(Exception exception)
	{
	}

	public void SendOperateServicesInitDiagnostics(Exception exception)
	{
	}

	internal async Task SendCoreDiagnosticsAsync(string diagnosticName, Exception exception)
	{
		await Task.CompletedTask;
	}

	private static void OnSendFailed(Task failedSendTask)
	{
	}

	internal async Task<IDiagnostics> GetOrCreateDiagnosticsAsync()
	{
		if (Diagnostics == null)
		{
			Diagnostics = (await DiagnosticsComponentProvider.CreateDiagnosticsComponents()).Create("com.unity.services.core");
		}
		return Diagnostics;
	}
}
