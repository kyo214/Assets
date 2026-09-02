using System;
using Fusion;

namespace Dissonance.Integrations.PhotonFusion;

public readonly struct FusionPeer(PlayerRef playerRef, bool loopback) : IEquatable<FusionPeer>
{
	public readonly PlayerRef PlayerRef = playerRef;

	public readonly bool IsLoopback = loopback;

	public bool Equals(FusionPeer other)
	{
		if (PlayerRef == other.PlayerRef)
		{
			return IsLoopback == other.IsLoopback;
		}
		return false;
	}

	public override bool Equals(object obj)
	{
		if (obj is FusionPeer other)
		{
			return Equals(other);
		}
		return false;
	}

	public override int GetHashCode()
	{
		return PlayerRef.GetHashCode() + (IsLoopback ? 1 : 0);
	}
}
