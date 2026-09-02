using System.Collections;
using UnityEngine;
using UnityEngine.AI;

namespace Chronos.Example;

[RequireComponent(typeof(NavMeshAgent))]
public class ExampleBubble : MonoBehaviour
{
	public float delay = 5f;

	public Vector3 areaCenter;

	public Vector3 areaSize;

	private void Start()
	{
		StartCoroutine(ChangeDestinationCoroutine());
	}

	private IEnumerator ChangeDestinationCoroutine()
	{
		while (true)
		{
			ChangeDestination();
			yield return new WaitForSeconds(delay);
		}
	}

	private void ChangeDestination()
	{
		Vector3 destination = new Vector3
		{
			x = areaCenter.x + Random.Range(0f - areaSize.x, areaSize.x) / 2f,
			y = areaCenter.y + Random.Range(0f - areaSize.y, areaSize.y) / 2f,
			z = areaCenter.z + Random.Range(0f - areaSize.z, areaSize.z) / 2f
		};
		GetComponent<NavMeshAgent>().SetDestination(destination);
	}

	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.magenta;
		Gizmos.DrawWireCube(areaCenter, areaSize);
	}
}
