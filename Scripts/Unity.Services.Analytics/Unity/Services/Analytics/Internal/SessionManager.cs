using System;

namespace Unity.Services.Analytics.Internal;

internal class SessionManager : ISessionManager
{
	public string SessionId { get; private set; }

	public SessionManager()
	{
		StartNewSession();
	}

	public void StartNewSession()
	{
		SessionId = Guid.NewGuid().ToString();
	}
}
