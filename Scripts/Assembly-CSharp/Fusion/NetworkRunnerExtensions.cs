namespace Fusion;

public static class NetworkRunnerExtensions
{
	public static bool SetActiveScene(this NetworkRunner runner, string sceneNameOrPath)
	{
		if (runner.SceneManager is NetworkSceneManagerBase networkSceneManagerBase)
		{
			if (networkSceneManagerBase.TryGetSceneRef(sceneNameOrPath, out var sceneRef))
			{
				runner.SetActiveScene(sceneRef);
				return true;
			}
			return false;
		}
		int sceneBuildIndex = FusionUnitySceneManagerUtils.GetSceneBuildIndex(sceneNameOrPath);
		if (sceneBuildIndex >= 0)
		{
			runner.SetActiveScene(sceneBuildIndex);
			return true;
		}
		return false;
	}
}
