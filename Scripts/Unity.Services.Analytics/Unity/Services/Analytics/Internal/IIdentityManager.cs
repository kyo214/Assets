using System;

namespace Unity.Services.Analytics.Internal;

internal interface IIdentityManager
{
	string UserId { get; }

	string InstallId { get; }

	string PlayerId { get; }

	string ExternalId { get; }

	bool IsNewPlayer { get; }

	event Action OnPlayerChanged;

	void Initialize();
}
