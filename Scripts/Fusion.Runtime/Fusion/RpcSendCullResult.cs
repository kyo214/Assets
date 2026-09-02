namespace Fusion;

public enum RpcSendCullResult
{
	NotCulled = 0,
	NotInvokableDuringResim = 1,
	InsufficientSourceAuthority = 2,
	NoActiveConnections = 3,
	TargetPlayerUnreachable = 4,
	TargetPlayerIsLocalButRpcIsNotInvokableLocally = 5,
	PayloadSizeExceeded = 6
}
