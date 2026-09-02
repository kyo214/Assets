using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStatusUI : MonoBehaviour
{
	[SerializeField]
	private List<GameObject> _mashButtonAnimation = new List<GameObject>();

	public List<Slider> ProgresBar = new List<Slider>();

	public static PlayerStatusUI Instance { get; private set; }

	private void Start()
	{
		if (Instance != null && Instance != this)
		{
			Object.Destroy(this);
		}
		else
		{
			Instance = this;
		}
	}

	public void SetEnableMashButton(int idx)
	{
		_mashButtonAnimation[idx].SetActive(value: true);
	}

	public void SetDisableMashButton(int idx)
	{
		_mashButtonAnimation[idx].SetActive(value: false);
	}
}
