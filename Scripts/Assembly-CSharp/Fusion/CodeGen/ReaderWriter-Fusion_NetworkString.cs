using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Fusion.CodeGen;

[StructLayout(LayoutKind.Sequential, Size = 1)]
internal struct ReaderWriter_0040Fusion_NetworkString_00601_003CFusion__64_003E : IElementReaderWriter<NetworkString<_64>>
{
	public static IElementReaderWriter<NetworkString<_64>> Instance;

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public unsafe NetworkString<_64> Read(byte* data, int index)
	{
		return *(NetworkString<_64>*)(data + index * 260);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public unsafe ref NetworkString<_64> ReadRef(byte* data, int index)
	{
		return ref *(NetworkString<_64>*)(data + index * 260);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public unsafe void Write(byte* data, int index, NetworkString<_64> val)
	{
		*(NetworkString<_64>*)(data + index * 260) = val;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public int GetElementWordCount()
	{
		return 65;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static IElementReaderWriter<NetworkString<_64>> GetInstance()
	{
		if (Instance == null)
		{
			Instance = default(ReaderWriter_0040Fusion_NetworkString_00601_003CFusion__64_003E);
		}
		return Instance;
	}
}
[StructLayout(LayoutKind.Sequential, Size = 1)]
internal struct ReaderWriter_0040Fusion_NetworkString_00601_003CFusion__32_003E : IElementReaderWriter<NetworkString<_32>>
{
	public static IElementReaderWriter<NetworkString<_32>> Instance;

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public unsafe NetworkString<_32> Read(byte* data, int index)
	{
		return *(NetworkString<_32>*)(data + index * 132);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public unsafe ref NetworkString<_32> ReadRef(byte* data, int index)
	{
		return ref *(NetworkString<_32>*)(data + index * 132);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public unsafe void Write(byte* data, int index, NetworkString<_32> val)
	{
		*(NetworkString<_32>*)(data + index * 132) = val;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public int GetElementWordCount()
	{
		return 33;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static IElementReaderWriter<NetworkString<_32>> GetInstance()
	{
		if (Instance == null)
		{
			Instance = default(ReaderWriter_0040Fusion_NetworkString_00601_003CFusion__32_003E);
		}
		return Instance;
	}
}
