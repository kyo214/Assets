using UnityEngine;

namespace MoreMountains.Feedbacks;

public class MMMiniPoolableObject : MonoBehaviour
{
	public delegate void Events();

	public float LifeTime;

	public event Events OnSpawnComplete;

	public virtual void Destroy()
	{
		base.gameObject.SetActive(value: false);
	}

	protected virtual void OnEnable()
	{
		if (LifeTime > 0f)
		{
			Invoke("Destroy", LifeTime);
		}
	}

	protected virtual void OnDisable()
	{
		CancelInvoke();
	}

	public virtual void TriggerOnSpawnComplete()
	{
		if (OnSpawnComplete != null)
		{
			OnSpawnComplete();
		}
	}
}
