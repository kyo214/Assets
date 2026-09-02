using System;

namespace Fusion;

public struct NetworkPrefabInfo
{
	public NetworkPrefabId Prefab;

	public unsafe NetworkObjectHeader* Header;

	public unsafe bool HasHeader => Header != null;

	public unsafe int* Data
	{
		get
		{
			if (HasHeader)
			{
				return (int*)Header + 20;
			}
			throw new InvalidOperationException();
		}
	}
}
