using System;
using System.Collections.Generic;
using Unity.Services.Core.Configuration.Internal;
using Unity.Services.Core.Telemetry.Internal;

namespace Unity.Services.Core.Internal;

internal class CoreMetrics
{
	internal const string PackageInitTimeMetricName = "package_init_time";

	internal const string AllPackagesInitSuccessMetricName = "all_packages_init_success";

	internal const string AllPackagesInitTimeMetricName = "all_packages_init_time";

	internal const string PackageInitializerNamesKeyFormat = "{0}.initializer-assembly-qualified-names";

	internal const char PackageInitializerNamesSeparator = ';';

	internal const string AllPackageNamesKey = "com.unity.services.core.all-package-names";

	internal const char AllPackageNamesSeparator = ';';

	public static CoreMetrics Instance { get; internal set; }

	internal IMetrics Metrics { get; set; }

	internal IDictionary<Type, IMetrics> AllPackageMetrics { get; } = new Dictionary<Type, IMetrics>();

	public void SendAllPackagesInitSuccessMetric()
	{
	}

	public void SendAllPackagesInitTimeMetric(double initTimeSeconds)
	{
	}

	public void SendInitTimeMetricForPackage(Type packageType, double initTimeSeconds)
	{
	}

	public void Initialize(IProjectConfiguration configuration, IMetricsFactory factory, Type corePackageType)
	{
	}

	internal void FindAndCacheAllPackageMetrics(IProjectConfiguration configuration, IMetricsFactory factory)
	{
	}
}
