namespace Unity.Services.Analytics.Internal;

internal interface IServiceDebug
{
	bool IsActive { get; }

	IIdentityManager UserIdentity { get; }
}
