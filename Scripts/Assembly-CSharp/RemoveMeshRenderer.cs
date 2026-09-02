using UnityEngine;

public class RemoveMeshRenderer : MonoBehaviour
{
	private void Start()
	{
		Object.Destroy(GetComponent<MeshRenderer>());
		Object.Destroy(GetComponent<MeshFilter>());
	}
}
