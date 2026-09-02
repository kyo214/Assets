using UnityEngine;

namespace _Modules.UILobby.Scripts;

public class PentagramLamp : MonoBehaviour
{
	[SerializeField]
	private GameObject[] _gameObjects;

	[SerializeField]
	private GameObject _explosionObjects;

	public void SetActive(bool setActive)
	{
		for (int num = _gameObjects.Length - 1; num >= 0; num--)
		{
			_gameObjects[num]?.gameObject.SetActive(setActive);
		}
	}

	public void SetActiveParentExplosion(bool setActive)
	{
		_explosionObjects?.gameObject.SetActive(setActive);
	}
}
