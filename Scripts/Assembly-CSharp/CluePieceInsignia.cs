using UnityEngine;

public class CluePieceInsignia : MonoBehaviour
{
	[SerializeField]
	private Transform[] _spotLights;

	private void Start()
	{
		for (int i = 0; i < _spotLights.Length; i++)
		{
			_spotLights[i].gameObject.SetActive(value: false);
		}
	}

	public void SetClue(Sprite insigniaSprite, int charsetIndex)
	{
		_spotLights[charsetIndex].gameObject.SetActive(value: true);
	}
}
