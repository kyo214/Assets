namespace MeshCombineStudio;

public struct Int3(int x, int y, int z)
{
	public int x = x;

	public int y = y;

	public int z = z;

	public static Int3 operator +(Int3 a, Int3 b)
	{
		Int3 result = default;
		result.x = a.x + b.x;
		result.y = a.y + b.y;
		result.z = a.z + b.z;
		return result;
	}
}
