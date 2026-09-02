using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Doozy.Runtime.Common.Utils;

public static class TypeUtils
{
	public static T CastObject<T>(object input)
	{
		return (T)input;
	}

	public static T ConvertObject<T>(object input)
	{
		return (T)Convert.ChangeType(input, typeof(T));
	}

	public static IEnumerable<Type> GetDerivedTypesOfType(Type type)
	{
		return from domainAssembly in ReflectionUtils.domainAssemblies
			from assemblyType in domainAssembly.GetTypes()
			where type.IsAssignableFrom(assemblyType)
			where assemblyType.IsSubclassOf(type) && !assemblyType.IsAbstract
			select assemblyType;
	}
}
