using UnityEngine;

public class EnemyPartPool : MonoBehaviour
{
	public float timeDespawn;

	private float timePassed;

	private bool despawned;

	[SerializeField]
	private GameObject partBody;

	[SerializeField]
	private GameObject partBlood;

	[SerializeField]
	private GameObject partEye;

	[SerializeField]
	private GameObject partHead;

	private void OnEnable()
	{
		despawned = false;
		timePassed = 0f;
	}

	public void initType(int type)
	{
		if (type == 1)
		{
			partBody.SetActive(value: true);
		}
		if (type == -1)
		{
			partBlood.SetActive(value: true);
			return;
		}
		partHead.SetActive(value: true);
		partEye.SetActive(value: true);
		partBlood.SetActive(value: false);
	}

	private void FixedUpdate()
	{
		timePassed += Time.deltaTime;
		if (!despawned && timePassed > timeDespawn)
		{
			despawned = true;
			partBody.SetActive(value: false);
			partBlood.SetActive(value: false);
			partEye.SetActive(value: false);
			partHead.SetActive(value: false);
			EnemyPartSpawner.Instance.Release(this);
		}
	}
}
