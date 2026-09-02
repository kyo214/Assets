using System;
using Unity.Services.Authentication.Internal;
using Unity.Services.Core.Configuration.Internal;
using Unity.Services.Core.Device.Internal;

namespace Unity.Services.Analytics.Internal;

internal class IdentityManager : IIdentityManager
{
	internal const string k_UnityAnalyticsInstallationIdKey = "UnityAnalyticsInstallationId";

	internal const string k_UnityAnalyticsUserIdKey = "UnityAnalyticsUserId";

	private readonly IPlayerId m_PlayerId;

	private readonly IExternalUserId m_ExternalIdProvider;

	private readonly IPersistence m_Persistence;

	private bool m_Initialized;

	public string UserId { get; private set; }

	public string InstallId { get; private set; }

	public string PlayerId => m_PlayerId?.PlayerId;

	public string ExternalId { get; private set; }

	public bool IsNewPlayer { get; private set; }

	public event Action OnPlayerChanged;

	public IdentityManager(IInstallationId installId, IPlayerId playerId, IExternalUserId externalId, IPersistence persistence)
	{
		InstallId = installId.GetOrCreateIdentifier();
		m_PlayerId = playerId;
		m_ExternalIdProvider = externalId;
		m_Persistence = persistence;
		if (m_ExternalIdProvider != null)
		{
			m_ExternalIdProvider.UserIdChanged += ExternalUserIdChanged;
			ExternalId = m_ExternalIdProvider.UserId;
		}
		UserId = ((!string.IsNullOrEmpty(ExternalId)) ? ExternalId : InstallId);
	}

	public void Initialize()
	{
		if (!m_Initialized)
		{
			string text = m_Persistence.LoadString("UnityAnalyticsUserId");
			bool flag = false;
			if (string.IsNullOrEmpty(text))
			{
				text = m_Persistence.LoadString("UnityAnalyticsInstallationId");
				m_Persistence.ClearValue("UnityAnalyticsInstallationId");
				flag = !string.IsNullOrEmpty(text);
			}
			if (m_ExternalIdProvider != null)
			{
				ExternalId = m_ExternalIdProvider.UserId;
			}
			UserId = (string.IsNullOrEmpty(ExternalId) ? InstallId : ExternalId);
			IsNewPlayer = string.IsNullOrEmpty(text) || !text.Equals(UserId, StringComparison.Ordinal);
			if (IsNewPlayer | flag)
			{
				m_Persistence.SaveValue("UnityAnalyticsUserId", UserId);
			}
			m_Initialized = true;
		}
	}

	private void ExternalUserIdChanged(string newName)
	{
		if (m_Initialized)
		{
			if (!UserId.Equals(newName, StringComparison.Ordinal))
			{
				if (string.IsNullOrEmpty(newName))
				{
					UserId = InstallId;
				}
				else if (string.IsNullOrEmpty(ExternalId))
				{
					UserId = newName;
				}
				else if (!ExternalId.Equals(newName, StringComparison.Ordinal))
				{
					UserId = newName;
				}
				m_Persistence.SaveValue("UnityAnalyticsUserId", UserId);
				ExternalId = newName;
				IsNewPlayer = true;
				OnPlayerChanged?.Invoke();
			}
		}
		else
		{
			ExternalId = newName;
			UserId = ((!string.IsNullOrEmpty(ExternalId)) ? ExternalId : InstallId);
		}
	}
}
