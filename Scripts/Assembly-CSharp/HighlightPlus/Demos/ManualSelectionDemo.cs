using UnityEngine;

namespace HighlightPlus.Demos;

public class ManualSelectionDemo : MonoBehaviour
{
	private HighlightManager hm;

	public Transform objectToSelect;

	private void Start()
	{
		hm = Object.FindObjectOfType<HighlightManager>();
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.Alpha1))
		{
			hm.SelectObject(objectToSelect);
		}
		if (Input.GetKeyDown(KeyCode.Alpha2))
		{
			hm.ToggleObject(objectToSelect);
		}
		if (Input.GetKeyDown(KeyCode.Alpha3))
		{
			hm.UnselectObject(objectToSelect);
		}
	}
}
