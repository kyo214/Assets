using System;
using System.Collections.Generic;
using UnityEngine;

namespace BansheeGz.BGDatabase;

public static class BGAddressablesMonitor
{
	public interface BGAddressablesMonitorDelegateI
	{
		void Unload<T>(string address, int times) where T : UnityEngine.Object;

		void UnloadAll<T>(string address, int times) where T : UnityEngine.Object;
	}

	private static readonly Dictionary<Tuple<BGId, BGId>, int> Origin2Count = new Dictionary<Tuple<BGId, BGId>, int>();

	public static BGAddressablesMonitorDelegateI UnloadDelegate;

	public static bool UnloadOnRowDelete;

	public static bool DebugOn;

	public static bool Enabled => UnloadDelegate != null;

	public static void AssetWasLoaded(BGField field, BGId entityId)
	{
		Tuple<BGId, BGId> key = Tuple.Create(field.Id, entityId);
		int num = ((!Origin2Count.TryGetValue(key, out var value)) ? 1 : (value + 1));
		Origin2Count[key] = num;
		if (DebugOn)
		{
			Debug.Log($"BGAddressablesMonitor debug: Asset for {field.FullName}[{entityId}] was loaded for {num} times");
		}
	}

	public static void UnloadAsset<T>(BGFieldUnityAssetA<T> field, BGId entityId) where T : UnityEngine.Object
	{
		UnloadAsset(field, entityId, field.Meta.FindEntityIndex(entityId));
	}

	public static void UnloadAsset<T>(BGFieldUnityAssetA<T> field, BGId entityId, int entityIndex) where T : UnityEngine.Object
	{
		int count = GetCount(field, entityId, entityIndex, out var address);
		if (count != 0)
		{
			UnloadDelegate.Unload<T>(address, count);
		}
	}

	public static void UnloadAsset<T>(BGFieldUnityAssetArrayA<T> field, BGId entityId) where T : UnityEngine.Object
	{
		UnloadAsset(field, entityId, field.Meta.FindEntityIndex(entityId));
	}

	public static void UnloadAsset<T>(BGFieldUnityAssetArrayA<T> field, BGId entityId, int entityIndex) where T : UnityEngine.Object
	{
		int count = GetCount(field, entityId, entityIndex, out var address);
		if (count != 0)
		{
			UnloadDelegate.UnloadAll<T>(address, count);
		}
	}

	private static int GetCount(BGField field, BGId entityId, int entityIndex, out string address)
	{
		address = null;
		if (entityIndex < 0)
		{
			return 0;
		}
		address = ((BGAddressablesAssetI)field).GetAddressablesAddress(entityIndex);
		if (string.IsNullOrEmpty(address))
		{
			return 0;
		}
		Tuple<BGId, BGId> key = Tuple.Create(field.Id, entityId);
		if (!Origin2Count.TryGetValue(key, out var value))
		{
			return 0;
		}
		if (DebugOn)
		{
			Debug.Log($"BGAddressablesMonitor debug: Unloading asset for {field.FullName}[{entityId}] {value} times");
		}
		Origin2Count.Remove(key);
		return value;
	}
}
