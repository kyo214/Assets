using UnityEngine;

namespace DestroyIt;

[RequireComponent(typeof(Destructible))]
public class WhenDamaged : MonoBehaviour
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
		Debug.Log($"{_destObj.name} was damaged for {_destObj.LastDamagedAmount} hit points");
	}
}
