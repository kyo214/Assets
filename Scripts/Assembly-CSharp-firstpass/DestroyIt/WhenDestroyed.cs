using UnityEngine;

namespace DestroyIt;

[RequireComponent(typeof(Destructible))]
public class WhenDestroyed : MonoBehaviour
{
	private Destructible _destObj;

	private void Start()
	{
		_destObj = base.gameObject.GetComponent<Destructible>();
		if (_destObj != null)
		{
			_destObj.DestroyedEvent += OnDestroyed;
		}
	}

	private void OnDisable()
	{
		if (!(_destObj == null))
		{
			_destObj.DestroyedEvent -= OnDestroyed;
		}
	}

	private void OnDestroyed()
	{
		Debug.Log($"{_destObj.name} was destroyed at world coordinates: {_destObj.transform.position}");
	}
}
