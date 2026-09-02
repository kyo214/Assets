using System.Runtime.CompilerServices;

namespace Fusion;

public static class NetworkObjectFlagsExtensions
{
	private const NetworkObjectFlags CurrentVersion = NetworkObjectFlags.V1;

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int GetVersion(this NetworkObjectFlags flags)
	{
		return (int)(flags & NetworkObjectFlags.MaskVersion);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool IsVersionCurrent(this NetworkObjectFlags flags)
	{
		return 1 == flags.GetVersion();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static NetworkObjectFlags SetCurrentVersion(this NetworkObjectFlags flags)
	{
		return SetWithMask(flags, NetworkObjectFlags.V1, NetworkObjectFlags.MaskVersion);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static NetworkObjectFlags SetType(this NetworkObjectFlags flags, NetworkObjectFlags type)
	{
		return SetWithMask(flags, type, NetworkObjectFlags.MaskType);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool IsPrefab(this NetworkObjectFlags flags)
	{
		return (flags & NetworkObjectFlags.MaskType) == NetworkObjectFlags.TypePrefab;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool IsSpawnedPrefabRoot(this NetworkObjectFlags flags)
	{
		return (flags & NetworkObjectFlags.MaskType) == NetworkObjectFlags.TypeSpawnedPrefab;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool IsSpawnedPrefabNestedObject(this NetworkObjectFlags flags)
	{
		return (flags & NetworkObjectFlags.MaskType) == NetworkObjectFlags.TypeSpawnedPrefabChild;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool IsSceneObject(this NetworkObjectFlags flags)
	{
		return (flags & NetworkObjectFlags.MaskType) == NetworkObjectFlags.TypeSceneObject;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool IsIgnored(this NetworkObjectFlags flags)
	{
		return (flags & NetworkObjectFlags.Ignore) == NetworkObjectFlags.Ignore;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static NetworkObjectFlags SetIgnored(this NetworkObjectFlags flags, bool value)
	{
		if (value)
		{
			return flags | NetworkObjectFlags.Ignore;
		}
		return flags & ~NetworkObjectFlags.Ignore;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool IsActivatedByUser(this NetworkObjectFlags flags)
	{
		return (flags & NetworkObjectFlags.ActivatedByUser) == NetworkObjectFlags.ActivatedByUser;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static NetworkObjectFlags SetActivatedByUser(this NetworkObjectFlags flags, bool value)
	{
		if (value)
		{
			return flags | NetworkObjectFlags.ActivatedByUser;
		}
		return flags & ~NetworkObjectFlags.ActivatedByUser;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static NetworkObjectFlags SetWithMask(NetworkObjectFlags flags, NetworkObjectFlags value, NetworkObjectFlags mask)
	{
		flags &= ~mask;
		flags |= value;
		return flags;
	}
}
