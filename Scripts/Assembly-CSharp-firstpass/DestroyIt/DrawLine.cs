using UnityEngine;

namespace DestroyIt;

public class DrawLine : MonoBehaviour
{
	public bool isActive = true;

	private void OnDrawGizmos()
	{
		if (isActive)
		{
			Gizmos.DrawLine(base.transform.position, base.transform.position + base.transform.forward * 10f);
		}
	}
}
