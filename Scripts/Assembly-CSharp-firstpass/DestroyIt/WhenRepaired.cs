using UnityEngine;

namespace DestroyIt;

[RequireComponent(typeof(Destructible))]
public class WhenRepaired : MonoBehaviour
{
	private Destructible _destObj;

	private void Start()
	{
		_destObj = base.gameObject.GetComponent<Destructible>();
		if (_destObj != null)
		{
			_destObj.RepairedEvent += OnRepaired;
		}
	}

	private void OnDisable()
	{
		if (!(_destObj == null))
		{
			_destObj.RepairedEvent -= OnRepaired;
		}
	}

	private void OnRepaired()
	{
		Debug.Log($"{_destObj.name} was repaired {_destObj.LastRepairedAmount} hit points");
	}
}
