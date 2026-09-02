using UnityEngine;

namespace DestroyIt;

public class DestructibleParent : MonoBehaviour
{
	private void OnCollisionEnter(Collision collision)
	{
		if (collision.contacts.Length == 0)
		{
			return;
		}
		Destructible componentInParent = collision.contacts[0].thisCollider.gameObject.GetComponentInParent<Destructible>();
		if (componentInParent != null)
		{
			Rigidbody attachedRigidbody = collision.contacts[0].otherCollider.attachedRigidbody;
			if (attachedRigidbody != null)
			{
				componentInParent.ProcessDestructibleCollision(collision, attachedRigidbody);
			}
			else
			{
				componentInParent.ProcessDestructibleCollision(collision, GetComponent<Rigidbody>());
			}
		}
	}
}
