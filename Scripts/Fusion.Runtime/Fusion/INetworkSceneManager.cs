namespace Fusion;

public interface INetworkSceneManager
{
	void Initialize(NetworkRunner runner);

	void Shutdown(NetworkRunner runner);

	bool IsReady(NetworkRunner runner);
}
