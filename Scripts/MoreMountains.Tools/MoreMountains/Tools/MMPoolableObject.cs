using UnityEngine;

namespace MoreMountains.Tools;

[AddComponentMenu("More Mountains/Tools/Object Pool/MMPoolableObject")]
public class MMPoolableObject : MMObjectBounds
{
	public delegate void Events();

	[Header("Poolable Object")]
	public float LifeTime;

	public event Events OnSpawnComplete;

	public virtual void Destroy()
	{
		base.gameObject.SetActive(value: false);
	}

	protected virtual void Update()
	{
	}

	protected virtual void OnEnable()
	{
		base.Size = GetBounds().extents * 2f;
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
		OnSpawnComplete?.Invoke();
	}
}
