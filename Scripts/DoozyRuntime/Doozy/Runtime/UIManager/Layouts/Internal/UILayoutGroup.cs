using Doozy.Runtime.Common.ScriptableObjects;
using Doozy.Runtime.UIManager.ScriptableObjects;
using UnityEngine;
using UnityEngine.UI;

namespace Doozy.Runtime.UIManager.Layouts.Internal;

[RequireComponent(typeof(RectTransform))]
[DisallowMultipleComponent]
[ExecuteAlways]
public abstract class UILayoutGroup : LayoutGroup
{
	public static UIManagerInputSettings inputSettings => SingletonRuntimeScriptableObject<UIManagerInputSettings>.instance;
}
