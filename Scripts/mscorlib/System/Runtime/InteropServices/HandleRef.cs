namespace System.Runtime.InteropServices;

public readonly struct HandleRef(object wrapper, IntPtr handle)
{
	private readonly object _wrapper = wrapper;

	private readonly IntPtr _handle = handle;

	public object Wrapper => _wrapper;

	public IntPtr Handle => _handle;

	public static explicit operator IntPtr(HandleRef value)
	{
		return value._handle;
	}

	public static IntPtr ToIntPtr(HandleRef value)
	{
		return value._handle;
	}
}
