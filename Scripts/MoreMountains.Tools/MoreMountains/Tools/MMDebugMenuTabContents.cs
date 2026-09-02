using UnityEngine;

namespace MoreMountains.Tools;

public class MMDebugMenuTabContents : MonoBehaviour
{
	public int Index;

	public Transform Parent;

	public bool ForceScaleOne = true;

	protected virtual void Start()
	{
		Initialization();
	}

	protected virtual void Initialization()
	{
		if (ForceScaleOne)
		{
			base.gameObject.GetComponent<RectTransform>().localScale = Vector3.one;
		}
	}
}
