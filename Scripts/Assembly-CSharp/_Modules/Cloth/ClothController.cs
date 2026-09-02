using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace _Modules.Cloth;

public class ClothController : MonoBehaviour
{
	[SerializeField]
	private UnityEngine.Cloth _cloth;

	private Dictionary<int, CapsuleCollider> _collidersDic = new Dictionary<int, CapsuleCollider>();

	private void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag("PlayerCollider"))
		{
			int instanceID = other.GetInstanceID();
			if (!_collidersDic.ContainsKey(instanceID))
			{
				CapsuleCollider component = other.GetComponent<CapsuleCollider>();
				_collidersDic.Add(instanceID, component);
				_cloth.capsuleColliders = _collidersDic.Values.ToArray();
			}
		}
		else if (other.CompareTag("Enemy"))
		{
			int instanceID2 = other.GetInstanceID();
			if (!_collidersDic.ContainsKey(instanceID2))
			{
				EnemyController component2 = other.GetComponent<EnemyController>();
				_collidersDic.Add(instanceID2, component2.bodyCollider);
				_cloth.capsuleColliders = _collidersDic.Values.ToArray();
			}
		}
	}

	private void OnTriggerExit(Collider other)
	{
	}

	public void SetWind(Vector3 wind)
	{
		_cloth.externalAcceleration = wind;
	}
}
