namespace Fusion;

internal interface INetworkString
{
	bool Equals<OtherSize>(ref NetworkString<OtherSize> other) where OtherSize : unmanaged, IFixedStorage;
}
