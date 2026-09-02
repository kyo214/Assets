using System;
using System.Runtime.InteropServices;

namespace Fusion;

[Serializable]
[StructLayout(LayoutKind.Explicit)]
[NetworkStructWeaved(1)]
public struct SceneRef : INetworkStruct
{
	public const int SIZE = 4;

	[FieldOffset(0)]
	private int _index;

	public static SceneRef None => default;

	public bool IsValid => _index > 0;

	public override bool Equals(object obj)
	{
		if (obj is SceneRef sceneRef)
		{
			return _index == sceneRef._index;
		}
		return false;
	}

	public override int GetHashCode()
	{
		return _index;
	}

	public override string ToString()
	{
		return (_index > 0) ? $"[Scene:{_index - 1}]" : "[Scene:None]";
	}

	public static implicit operator bool(SceneRef value)
	{
		return value._index > 0;
	}

	public static implicit operator int(SceneRef value)
	{
		return value._index - 1;
	}

	public static implicit operator SceneRef(int value)
	{
		SceneRef result = default;
		result._index = value + 1;
		return result;
	}

	public static bool operator ==(SceneRef a, SceneRef b)
	{
		return a._index == b._index;
	}

	public static bool operator !=(SceneRef a, SceneRef b)
	{
		return a._index != b._index;
	}
}
