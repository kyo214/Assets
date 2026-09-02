namespace Unity.Services.Analytics.Internal;

internal interface ISessionManager
{
	string SessionId { get; }

	void StartNewSession();
}
