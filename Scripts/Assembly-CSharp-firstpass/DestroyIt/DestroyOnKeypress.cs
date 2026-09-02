using System;
using UnityEngine;

namespace DestroyIt;

public class DestroyOnKeypress : MonoBehaviour
{
	public float force = 500f;

	public float radius = 10f;

	public float upwardModifier = -1f;

	public ObjectToDestroy[] objectsToDestroy;

	private void Update()
	{
		for (int i = 0; i < objectsToDestroy.Length; i++)
		{
			if (string.IsNullOrEmpty(objectsToDestroy[i].key) || !Input.GetKeyUp((KeyCode)Enum.Parse(typeof(KeyCode), objectsToDestroy[i].key.ToUpper())))
			{
				continue;
			}
			Destructible[] destructibles = objectsToDestroy[i].destructibles;
			for (int j = 0; j < destructibles.Length; j++)
			{
				if (destructibles[j] == null)
				{
					continue;
				}
				Collider componentInChildren = destructibles[j].GetComponentInChildren<Collider>();
				if (componentInChildren != null)
				{
					Vector3 position = destructibles[j].transform.position;
					Vector3 vector = componentInChildren.ClosestPoint(new Vector3(position.x, position.y + 5000f, position.z));
					float num = UnityEngine.Random.Range(1, 4);
					if (UnityEngine.Random.Range(0, 2) == 1)
					{
						num *= -1f;
					}
					float num2 = UnityEngine.Random.Range(1, 4);
					if (UnityEngine.Random.Range(0, 2) == 1)
					{
						num2 *= -1f;
					}
					Vector3 position2 = new Vector3(vector.x + num, vector.y, vector.z + num2);
					destructibles[j].ApplyDamage(new ExplosiveDamage
					{
						BlastForce = force,
						DamageAmount = destructibles[j].currentHitPoints + 1f,
						Position = position2,
						Radius = radius,
						UpwardModifier = upwardModifier
					});
				}
				else
				{
					destructibles[j].Destroy();
				}
			}
		}
	}
}
