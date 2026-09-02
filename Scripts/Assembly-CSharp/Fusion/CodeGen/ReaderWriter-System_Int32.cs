using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Fusion.CodeGen;

[StructLayout(LayoutKind.Sequential, Size = 1)]
internal struct ReaderWriter_0040System_Int32 : IElementReaderWriter<int>
{
	public static IElementReaderWriter<int> Instance;

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public unsafe int Read(byte* data, int index)
	{
		return *(int*)(data + index * 4);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public unsafe ref int ReadRef(byte* data, int index)
	{
		return ref *(int*)(data + index * 4);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public unsafe void Write(byte* data, int index, int val)
	{
		*(int*)(data + index * 4) = val;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public int GetElementWordCount()
	{
		return 1;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static IElementReaderWriter<int> GetInstance()
	{
		if (Instance == null)
		{
			Instance = default(ReaderWriter_0040System_Int32);
		}
		return Instance;
	}
}
