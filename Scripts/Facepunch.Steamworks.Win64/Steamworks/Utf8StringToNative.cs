using System;
using System.Runtime.InteropServices;

namespace Steamworks;

internal struct Utf8StringToNative : IDisposable
{
	public IntPtr Pointer { get; private set; }

	public unsafe Utf8StringToNative(string value)
	{
		if (value == null)
		{
			Pointer = IntPtr.Zero;
			return;
		}
		fixed (char* chars = value)
		{
			int byteCount = Utility.Utf8NoBom.GetByteCount(value);
			IntPtr intPtr = Marshal.AllocHGlobal(byteCount + 1);
			int bytes = Utility.Utf8NoBom.GetBytes(chars, value.Length, (byte*)(void*)intPtr, byteCount + 1);
			((sbyte*)(void*)intPtr)[bytes] = 0;
			Pointer = intPtr;
		}
	}

	public void Dispose()
	{
		if (Pointer != IntPtr.Zero)
		{
			Marshal.FreeHGlobal(Pointer);
			Pointer = IntPtr.Zero;
		}
	}
}
