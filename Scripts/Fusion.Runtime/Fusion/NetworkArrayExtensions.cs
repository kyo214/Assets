namespace Fusion;

public static class NetworkArrayExtensions
{
	public static ref T GetRef<T>(this NetworkArray<T> array, int index) where T : unmanaged
	{
		return ref array.GetRef(index);
	}
}
