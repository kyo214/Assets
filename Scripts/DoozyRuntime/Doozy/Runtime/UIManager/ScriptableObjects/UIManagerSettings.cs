using Doozy.Runtime.Common.Attributes;
using Doozy.Runtime.Common.ScriptableObjects;

namespace Doozy.Runtime.UIManager.ScriptableObjects;

public class UIManagerSettings : SingletonRuntimeScriptableObject<UIManagerSettings>
{
	public bool UseOrientationDetection;

	[RestoreData("UIManagerSettings")]
	public static UIManagerSettings Get()
	{
		return SingletonRuntimeScriptableObject<UIManagerSettings>.instance;
	}
}
