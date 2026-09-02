namespace Unity.Services.Analytics.Internal;

internal interface IDispatcher
{
	int ConsecutiveFailedUploadCount { get; }

	void SetBuffer(IBuffer buffer);

	void Flush();
}
