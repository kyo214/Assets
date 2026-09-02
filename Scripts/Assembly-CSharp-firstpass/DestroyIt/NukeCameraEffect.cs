using UnityEngine;

namespace DestroyIt;

[RequireComponent(typeof(Camera))]
public class NukeCameraEffect : MonoBehaviour
{
	[Tooltip("Stores the initial, regular tonemapping settings of the main camera for later use.")]
	public Tonemapping regularTonemapping;

	[Tooltip("Alternate tonemapping settings to make the scene look over-exposed and strange while the nuke is active.")]
	public Tonemapping nukeTonemapping;

	[Tooltip("Optional additional color correction curves to use on the main camera to make the scene look strange while the nuke is active.")]
	public ColorCorrectionCurves nukeColorCorrection;

	public void Start()
	{
		Tonemapping[] components = GetComponents<Tonemapping>();
		if (components != null && components.Length == 2)
		{
			if (components[0].enabled)
			{
				regularTonemapping = components[0];
				nukeTonemapping = components[1];
			}
			else
			{
				nukeTonemapping = components[0];
				regularTonemapping = components[1];
			}
			nukeTonemapping.enabled = false;
		}
		ColorCorrectionCurves component = GetComponent<ColorCorrectionCurves>();
		if (component != null && !component.enabled)
		{
			nukeColorCorrection = component;
		}
	}

	public void OnNukeStart()
	{
		if (regularTonemapping != null && regularTonemapping.enabled)
		{
			regularTonemapping.enabled = false;
			nukeTonemapping.enabled = true;
		}
		if (nukeColorCorrection != null && !nukeColorCorrection.enabled)
		{
			nukeColorCorrection.enabled = true;
		}
	}

	public void OnNukeEnd()
	{
		if (regularTonemapping != null && !regularTonemapping.enabled && nukeTonemapping != null && nukeTonemapping.enabled)
		{
			nukeTonemapping.enabled = false;
			regularTonemapping.enabled = true;
		}
		if (nukeColorCorrection != null && nukeColorCorrection.enabled)
		{
			nukeColorCorrection.enabled = false;
		}
	}
}
