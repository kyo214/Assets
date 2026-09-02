using System.Collections.Generic;
using UnityEngine;

namespace MoreMountains.Tools;

[AddComponentMenu("More Mountains/Tools/Property Controllers/LightController")]
public class LightController : MonoBehaviour
{
	[Header("Binding")]
	[MMInformation("Use this component to control the properties of one or more lights at runtime. Plays well with a FloatController. This component will try to auto set the TargetLight if there's a Light component on this object.", MMInformationAttribute.InformationType.Info, false)]
	public Light TargetLight;

	public List<Light> TargetLights;

	[Header("Light Settings")]
	public float Intensity = 1f;

	public float Multiplier = 1f;

	public float Range = 1f;

	[Header("Color")]
	public Color LightColor;

	protected virtual void Start()
	{
		Initialization();
	}

	protected virtual void Initialization()
	{
		if (TargetLight == null)
		{
			TargetLight = base.gameObject.GetComponent<Light>();
		}
		if (TargetLight != null)
		{
			TargetLight.range = Range;
			TargetLight.color = LightColor;
		}
		if (TargetLights.Count <= 0)
		{
			return;
		}
		foreach (Light targetLight in TargetLights)
		{
			if (targetLight != null)
			{
				targetLight.range = Range;
				targetLight.color = LightColor;
			}
		}
	}

	protected virtual void Update()
	{
		ApplyLightSettings();
	}

	protected virtual void ApplyLightSettings()
	{
		if (TargetLight != null)
		{
			TargetLight.intensity = Intensity * Multiplier;
		}
		if (TargetLights.Count <= 0)
		{
			return;
		}
		foreach (Light targetLight in TargetLights)
		{
			if (targetLight != null)
			{
				targetLight.intensity = Intensity * Multiplier;
			}
		}
	}
}
