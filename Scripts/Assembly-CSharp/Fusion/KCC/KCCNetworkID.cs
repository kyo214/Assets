using System.Runtime.InteropServices;

namespace Fusion.KCC;

[StructLayout(LayoutKind.Explicit)]
public struct KCCNetworkID
{
	[FieldOffset(0)]
	public int A;

	[FieldOffset(4)]
	public int B;

	[FieldOffset(8)]
	public int C;

	[FieldOffset(12)]
	public int D;

	[FieldOffset(0)]
	public uint Raw;

	[FieldOffset(0)]
	public long Value0;

	[FieldOffset(8)]
	public long Value1;

	public bool IsValid => (A | B | C | D) != 0;

	public bool Equals(KCCNetworkID other)
	{
		if (A == other.A && B == other.B && C == other.C)
		{
			return D == other.D;
		}
		return false;
	}

	public unsafe static KCCNetworkID GetNetworkID(NetworkObject networkObject)
	{
		if (networkObject == null)
		{
			return default;
		}
		KCCNetworkID result = default;
		if (networkObject.Id.IsValid)
		{
			result.Raw = networkObject.Id.Raw;
		}
		else
		{
			result.Value0 = networkObject.NetworkGuid.RawGuidValue[0];
			result.Value1 = networkObject.NetworkGuid.RawGuidValue[1];
		}
		return result;
	}

	public unsafe static NetworkObject GetNetworkObject(NetworkRunner runner, KCCNetworkID networkID)
	{
		if (!networkID.IsValid)
		{
			return null;
		}
		if (networkID.B == 0 && networkID.C == 0 && networkID.D == 0)
		{
			NetworkObject networkObject = runner.FindObject(new NetworkId
			{
				Raw = networkID.Raw
			});
			if (networkObject != null)
			{
				return networkObject;
			}
		}
		NetworkObjectGuid guid = default;
		ref long rawGuidValue = ref guid.RawGuidValue[0];
		rawGuidValue = networkID.Value0;
		guid.RawGuidValue[1] = networkID.Value1;
		if (runner.Config.PrefabTable.TryGetId(guid, out var id) && runner.Config.PrefabTable.TryGetPrefab(id, out var obj))
		{
			return obj;
		}
		return null;
	}

	public override string ToString()
	{
		return $"{A} | {B} | {C} | {B}";
	}
}
