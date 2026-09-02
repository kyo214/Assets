using UnityEngine;

namespace MoreMountains.Tools;

public class MMObservableDemoSubject : MonoBehaviour
{
	public MMObservable<float> PositionX;

	protected virtual void Update()
	{
		PositionX.Value = base.transform.position.x;
	}
}
