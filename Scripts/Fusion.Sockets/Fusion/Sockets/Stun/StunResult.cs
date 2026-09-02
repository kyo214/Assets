namespace Fusion.Sockets.Stun;

internal class StunResult
{
	public NATType NatType = NATType.Invalid;

	public static readonly StunResult Invalid = new StunResult(NetAddress.AnyIPv4Addr, NetAddress.AnyIPv4Addr);

	public bool IsValid => PublicEndPoint.IsValid && PrivateEndPoint.IsValid;

	public NetAddress PublicEndPoint { get; private set; } = default;

	public NetAddress PrivateEndPoint { get; private set; } = default;

	internal StunResult(NetAddress publicEndPoint = default(NetAddress), NetAddress privateEndPoint = default(NetAddress))
	{
		PublicEndPoint = publicEndPoint;
		PrivateEndPoint = privateEndPoint;
	}

	public override string ToString()
	{
		return string.Format("[{0}: {1}={2}, {3}={4}, {5}={6}]", "StunResult", "PublicEndPoint", PublicEndPoint, "PrivateEndPoint", PrivateEndPoint, "NatType", NatType);
	}
}
