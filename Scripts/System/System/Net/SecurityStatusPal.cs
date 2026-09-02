namespace System.Net;

internal readonly struct SecurityStatusPal(SecurityStatusPalErrorCode errorCode, Exception exception = null)
{
	public readonly SecurityStatusPalErrorCode ErrorCode = errorCode;

	public readonly Exception Exception = exception;

	public override string ToString()
	{
		if (Exception != null)
		{
			return string.Format("{0}={1}, {2}={3}", "ErrorCode", ErrorCode, "Exception", Exception);
		}
		return string.Format("{0}={1}", "ErrorCode", ErrorCode);
	}
}
