namespace Unity.Services.Analytics.Data;

internal interface IDataGenerator
{
	void GameRunning(string callingMethodIdentifier);

	void SdkStartup(string callingMethodIdentifier);

	void NewPlayer(string callingMethodIdentifier);

	void GameStarted(string callingMethodIdentifier);

	void GameEnded(string callingMethodIdentifier, DataGenerator.SessionEndState quitState);

	void ClientDevice(string callingMethodIdentifier);

	void PushCommonParams(string callingMethodIdentifier);

	void PushEvent(string callingMethodIdentifier, Event e);

	void PushEmptyEvent(string name);
}
