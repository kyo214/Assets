namespace System.Drawing;

internal struct CarbonContext(IntPtr port, IntPtr ctx, int width, int height) : IMacContext
{
	public IntPtr port = port;

	public IntPtr ctx = ctx;

	public int width = width;

	public int height = height;

	public void Synchronize()
	{
		MacSupport.CGContextSynchronize(ctx);
	}

	public void Release()
	{
		MacSupport.ReleaseContext(port, ctx);
	}
}
