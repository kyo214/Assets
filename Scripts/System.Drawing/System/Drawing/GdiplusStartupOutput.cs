namespace System.Drawing;

internal struct GdiplusStartupOutput
{
	internal IntPtr NotificationHook;

	internal IntPtr NotificationUnhook;

	internal static GdiplusStartupOutput MakeGdiplusStartupOutput()
	{
		GdiplusStartupOutput result = default;
		result.NotificationHook = (result.NotificationUnhook = IntPtr.Zero);
		return result;
	}
}
