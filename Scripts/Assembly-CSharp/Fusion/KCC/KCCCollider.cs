using UnityEngine;

namespace Fusion.KCC;

public sealed class KCCCollider
{
	public GameObject GameObject;

	public Transform Transform;

	public CapsuleCollider Collider;

	public bool IsSpawned;

	public bool IsTrigger;

	public float Radius;

	public float Height;

	public int Layer;

	public void Update(Transform parent, KCCSettings settings)
	{
		if (!IsSpawned)
		{
			IsTrigger = settings.IsTrigger;
			Radius = settings.Radius;
			Height = settings.Height;
			Layer = settings.ColliderLayer;
			GameObject = new GameObject("KCCCollider");
			GameObject.layer = settings.ColliderLayer;
			Transform = GameObject.transform;
			Transform.SetParent(parent, worldPositionStays: false);
			Transform.localPosition = Vector3.zero;
			Transform.localRotation = Quaternion.identity;
			Transform.localScale = Vector3.one;
			Collider = GameObject.AddComponent<CapsuleCollider>();
			Collider.direction = 1;
			Collider.isTrigger = settings.IsTrigger;
			Collider.radius = settings.Radius;
			Collider.height = settings.Height;
			Collider.center = new Vector3(0f, settings.Height * 0.5f, 0f);
			IsSpawned = true;
		}
		if (IsTrigger != settings.IsTrigger)
		{
			IsTrigger = settings.IsTrigger;
			Collider.isTrigger = settings.IsTrigger;
		}
		if (Radius != settings.Radius)
		{
			Radius = settings.Radius;
			Collider.radius = settings.Radius;
		}
		if (Height != settings.Height)
		{
			Height = settings.Height;
			Collider.height = settings.Height;
			Collider.center = new Vector3(0f, settings.Height * 0.5f, 0f);
		}
		if (Layer != settings.ColliderLayer)
		{
			Layer = settings.ColliderLayer;
			GameObject.layer = settings.ColliderLayer;
		}
	}

	public void Destroy()
	{
		if (IsSpawned)
		{
			if (Collider != null)
			{
				Collider.enabled = false;
			}
			Object.Destroy(GameObject);
			GameObject = null;
			Transform = null;
			Collider = null;
			IsSpawned = false;
			IsTrigger = false;
			Radius = 0f;
			Height = 0f;
			Layer = 0;
		}
	}
}
