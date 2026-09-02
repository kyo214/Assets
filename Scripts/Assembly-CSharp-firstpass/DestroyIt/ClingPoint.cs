using UnityEngine;

namespace DestroyIt;

public class ClingPoint : MonoBehaviour
{
	public int chanceToCling = 75;

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.yellow;
		Gizmos.DrawWireSphere(base.transform.position - base.transform.forward * 0.025f, 0.01f);
		Gizmos.DrawRay(base.transform.position - base.transform.forward * 0.025f, base.transform.forward * 0.075f);
	}
}
