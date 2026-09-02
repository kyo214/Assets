using System;

namespace Fusion;

[OrderAfter(new Type[] { typeof(NetworkBehaviour) })]
public abstract class NetworkAreaOfInterestBehaviour : NetworkBehaviour
{
	public abstract int PositionWordOffset { get; }
}
