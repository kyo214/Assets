using UnityEngine;

namespace DestroyIt;

[RequireComponent(typeof(Destructible))]
public class WhenDamagedDamageParents : MonoBehaviour
{
	private Destructible _destObj;

	private void Start()
	{
		_destObj = base.gameObject.GetComponent<Destructible>();
		if (_destObj != null)
		{
			_destObj.DamagedEvent += OnDamaged;
		}
	}

	private void OnDisable()
	{
		if (!(_destObj == null))
		{
			_destObj.DamagedEvent -= OnDamaged;
		}
	}

	private void OnDamaged()
	{
		Destructible[] componentsInParent = base.gameObject.GetComponentsInParent<Destructible>();
		for (int i = 0; i < componentsInParent.Length; i++)
		{
			if (!(componentsInParent[i] == _destObj))
			{
				componentsInParent[i].ApplyDamage(_destObj.LastDamagedAmount);
			}
		}
	}
}
