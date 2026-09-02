using System;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine;

[RequiredByNativeCode(GenerateProxy = true)]
[NativeHeader("Runtime/Mono/AssemblyFullName.h")]
internal struct AssemblyVersion(ushort major, ushort minor, ushort build, ushort revision)
{
	public ushort major = major;

	public ushort minor = minor;

	public ushort build = build;

	public ushort revision = revision;

	public static bool operator ==(AssemblyVersion lhs, AssemblyVersion rhs)
	{
		return lhs.major == rhs.major && lhs.minor == rhs.minor && lhs.build == rhs.build && lhs.revision == rhs.revision;
	}

	public static bool operator !=(AssemblyVersion lhs, AssemblyVersion rhs)
	{
		return !(lhs == rhs);
	}

	public static bool operator <(AssemblyVersion lhs, AssemblyVersion rhs)
	{
		if (lhs.major != rhs.major)
		{
			return lhs.major < rhs.major;
		}
		if (lhs.minor != rhs.minor)
		{
			return lhs.minor < rhs.minor;
		}
		if (lhs.build != rhs.build)
		{
			return lhs.build < rhs.build;
		}
		if (lhs.revision != rhs.revision)
		{
			return lhs.revision < rhs.revision;
		}
		return false;
	}

	public static bool operator >(AssemblyVersion lhs, AssemblyVersion rhs)
	{
		if (lhs.major != rhs.major)
		{
			return lhs.major > rhs.major;
		}
		if (lhs.minor != rhs.minor)
		{
			return lhs.minor > rhs.minor;
		}
		if (lhs.build != rhs.build)
		{
			return lhs.build > rhs.build;
		}
		if (lhs.revision != rhs.revision)
		{
			return lhs.revision > rhs.revision;
		}
		return false;
	}

	public override string ToString()
	{
		return $"{major}.{minor}.{build}.{revision}";
	}

	public override bool Equals(object other)
	{
		return other is AssemblyVersion assemblyVersion && major == assemblyVersion.major && minor == assemblyVersion.minor && build == assemblyVersion.build && revision == assemblyVersion.revision;
	}

	public override int GetHashCode()
	{
		return HashCode.Combine(major, minor, build, revision);
	}
}
