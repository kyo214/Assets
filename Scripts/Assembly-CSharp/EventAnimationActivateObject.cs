using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class EventAnimationActivateObject : MonoBehaviour
{
	[SerializeField]
	private List<GameObject> listObjectActivate = new List<GameObject>();

	[SerializeField]
	private Transform TransformPos;

	public void OnActivateObjectHollowMother(int index)
	{
		listObjectActivate[index].SetActive(value: true);
		listObjectActivate[index].transform.position = TransformPos.position;
		listObjectActivate[index].transform.localRotation = TransformPos.localRotation;
		UniTaskUtil.DelayedCall(this, 3f, () =>
		{
			listObjectActivate[index].SetActive(value: false);
		}).Forget();
	}
}
