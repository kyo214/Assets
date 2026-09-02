using System.Collections.Generic;
using UnityEngine;

public class MapController : MonoBehaviour
{
	public List<ObjectArray> arrPosEnemy = new List<ObjectArray>();

	public static MapController Instance { get; private set; }

	private void Awake()
	{
		if (Instance != null && Instance != this)
		{
			Object.Destroy(base.gameObject);
		}
		else
		{
			Instance = this;
		}
		int num = Random.Range(0, arrPosEnemy.Count);
		for (int i = 0; i < arrPosEnemy.Count; i++)
		{
			if (num == i)
			{
				continue;
			}
			foreach (GameObject item in arrPosEnemy[i].arrObject)
			{
				bool flag = false;
				for (int j = 0; j < arrPosEnemy[num].arrObject.Count; j++)
				{
					if (arrPosEnemy[num].arrObject[j] == item)
					{
						flag = true;
						break;
					}
				}
				if (!flag)
				{
					Object.Destroy(item);
				}
			}
		}
	}
}
