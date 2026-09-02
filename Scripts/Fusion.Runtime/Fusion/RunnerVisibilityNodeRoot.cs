using UnityEngine;

namespace Fusion;

[AddComponentMenu("")]
internal class RunnerVisibilityNodeRoot : MonoBehaviour
{
	private void Awake()
	{
		base.hideFlags = HideFlags.HideInInspector;
	}
}
