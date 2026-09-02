using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Fusion.CodeGen;

[StructLayout(LayoutKind.Sequential, Size = 1)]
internal struct ReaderWriter_0040System_Boolean : IElementReaderWriter<bool>
{
	public static IElementReaderWriter<bool> Instance;

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public unsafe bool Read(byte* data, int index)
	{
		return ReadWriteUtilsForWeaver.ReadBoolean((int*)(data + index * 4));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public unsafe ref bool ReadRef(byte* data, int index)
	{
		throw new NotSupportedException("Only supported for trivially copyable types. System.Boolean is not trivially copyable.");
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public unsafe void Write(byte* data, int index, bool val)
	{
		ReadWriteUtilsForWeaver.WriteBoolean((int*)(data + index * 4), val);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public int GetElementWordCount()
	{
		return 1;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static IElementReaderWriter<bool> GetInstance()
	{
		if (Instance == null)
		{
			Instance = default(ReaderWriter_0040System_Boolean);
		}
		return Instance;
	}
}
