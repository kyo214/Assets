using System;

namespace Unity.Services.Analytics.Internal;

internal interface IAnalyticsForgetter
{
	bool DeletionInProgress { get; }

	void ResetDataDeletionStatus();

	void AttemptToForget(string userId, string installationId, string playerId, string timestamp, string callingMethod, Action successfulUploadCallback);
}
