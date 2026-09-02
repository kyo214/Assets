using System.Collections;
using UnityEngine;

namespace DestroyIt;

public class BulbFlash : MonoBehaviour
{
	private void OnEnable()
	{
		StartCoroutine(RemovePower());
	}

	private IEnumerator RemovePower()
	{
		yield return 0;
		Transform parent = base.transform.parent;
		if (parent != null)
		{
			parent.gameObject.RemoveTag(Tag.Powered);
			parent.gameObject.RemoveComponent<PoweredTag>();
		}
		StopCoroutine("RemovePower");
	}
}
