using UnityEngine;

namespace MoreMountains.Feedbacks;

public class DemoGhost : MonoBehaviour
{
	public virtual void OnAnimationEnd()
	{
		base.gameObject.SetActive(value: false);
	}
}
