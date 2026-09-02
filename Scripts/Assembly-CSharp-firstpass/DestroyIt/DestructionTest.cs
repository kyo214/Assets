using UnityEngine;

namespace DestroyIt;

[DisallowMultipleComponent]
public class DestructionTest : MonoBehaviour
{
	public Destructible objectToDestroy;

	public int damagePerPress = 13;

	public void Update()
	{
		if (!Input.GetKeyUp("0"))
		{
			return;
		}
		if (objectToDestroy != null)
		{
			objectToDestroy.ApplyDamage(damagePerPress);
			return;
		}
		Destructible[] array = Object.FindObjectsOfType<Destructible>();
		for (int i = 0; i < array.Length; i++)
		{
			array[i].ApplyDamage(damagePerPress);
		}
	}
}
