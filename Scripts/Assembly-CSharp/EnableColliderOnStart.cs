using UnityEngine;

public class EnableColliderOnStart : MonoBehaviour
{
	private void Start()
	{
		GetComponent<Collider>().enabled = true;
		GameManager.Instance.AStarPath.UpdateGraphs(GetComponent<Collider>().bounds);
		GameManager.Instance.AStarPath.FlushGraphUpdates();
	}
}
