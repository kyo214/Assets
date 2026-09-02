using UnityEngine;

namespace Fusion.KCC;

public sealed class KCCIgnore
{
	public KCCNetworkID NetworkID;

	public NetworkObject NetworkObject;

	public Collider Collider;

	public void CopyFromOther(KCCIgnore other)
	{
		NetworkID = other.NetworkID;
		NetworkObject = other.NetworkObject;
		Collider = other.Collider;
	}

	public void Clear()
	{
		NetworkID = default;
		NetworkObject = null;
		Collider = null;
	}
}
