using UnityEngine;

public class RootTest : MonoBehaviour
{
	public GameObject rootAnimatorPrefab;

	public GameObject rootVertexAnimPrefab;

	public Transform animatorParent;

	public Transform vertexAnimParent;

	public int spawnCount;

	public float spawnRadius;

	public void Start()
	{
		Application.targetFrameRate = -1;
		QualitySettings.vSyncCount = 0;
		Vector3 position = default;
		for (int i = 0; i < spawnCount; i++)
		{
			position.x = Random.Range(0f - spawnRadius, spawnRadius);
			position.y = Random.Range(0f - spawnRadius, spawnRadius);
			position.z = Random.Range(0f - spawnRadius, spawnRadius);
			Object.Instantiate(rootAnimatorPrefab, position, Quaternion.identity, animatorParent).GetComponent<Animator>().Play("Animated", -1, Random.Range(0f, 1f));
			Object.Instantiate(rootVertexAnimPrefab, position, Quaternion.identity, vertexAnimParent);
		}
	}

	public void ToggleRootGroups()
	{
		if (animatorParent.gameObject.activeSelf)
		{
			animatorParent.gameObject.SetActive(value: false);
			vertexAnimParent.gameObject.SetActive(value: true);
			return;
		}
		animatorParent.gameObject.SetActive(value: true);
		vertexAnimParent.gameObject.SetActive(value: false);
		foreach (Transform item in animatorParent)
		{
			item.GetComponent<Animator>().Play("Animated", -1, Random.Range(0f, 1f));
		}
	}
}
