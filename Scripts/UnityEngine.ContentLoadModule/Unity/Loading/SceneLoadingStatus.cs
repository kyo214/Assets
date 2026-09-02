namespace Unity.Loading;

public enum SceneLoadingStatus
{
	InProgress = 0,
	WaitingForIntegrate = 1,
	WillIntegrateNextFrame = 2,
	Complete = 3,
	Failed = 4
}
