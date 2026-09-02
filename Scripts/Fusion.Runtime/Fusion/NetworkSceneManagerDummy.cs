namespace Fusion;

internal sealed class NetworkSceneManagerDummy : INetworkSceneManager
{
	public void Initialize(NetworkRunner runner)
	{
	}

	public bool IsReady(NetworkRunner runner)
	{
		return true;
	}

	public void Shutdown(NetworkRunner runner)
	{
	}
}
