using Doozy.Runtime.Common.ScriptableObjects;
using Doozy.Runtime.SceneManagement.ScriptableObjects;

namespace Doozy.Runtime.SceneManagement;

public static class SceneUtils
{
	private static SceneManagementSettings settings => SingletonRuntimeScriptableObject<SceneManagementSettings>.instance;
}
