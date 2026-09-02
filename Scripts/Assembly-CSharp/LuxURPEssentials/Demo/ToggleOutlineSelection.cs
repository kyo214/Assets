using UnityEngine;

namespace LuxURPEssentials.Demo;

public class ToggleOutlineSelection : MonoBehaviour
{
	public Material SelectionMaterial;

	public Material OutlineMaterial;

	private Renderer rend;

	private Material[] BaseMatArray = new Material[1];

	private Material[] SelectedMatArray = new Material[2];

	private bool Selected;

	private void OnEnable()
	{
		rend = GetComponent<Renderer>();
		BaseMatArray[0] = rend.sharedMaterials[0];
		SelectedMatArray[0] = SelectionMaterial;
		SelectedMatArray[1] = OutlineMaterial;
	}

	public void Select()
	{
		if (!Selected)
		{
			rend.sharedMaterials = SelectedMatArray;
			Selected = true;
		}
		else
		{
			rend.sharedMaterials = BaseMatArray;
			Selected = false;
		}
	}
}
