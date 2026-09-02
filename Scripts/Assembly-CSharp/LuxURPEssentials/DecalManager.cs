using UnityEngine;

namespace LuxURPEssentials;

public class DecalManager : MonoBehaviour
{
	public bool Gizmos = true;

	public static bool DrawDecalGizmos = true;

	private void OnValidate()
	{
		DrawDecalGizmos = Gizmos;
	}
}
