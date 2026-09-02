using System.Collections.Generic;
using UnityEngine;

namespace MoreMountains.Tools;

[AddComponentMenu("More Mountains/Tools/Camera/MMAspectRatioSafeZones")]
public class MMAspectRatioSafeZones : MonoBehaviour
{
	[Header("Center")]
	public bool DrawCenterCrosshair = true;

	public float CenterCrosshairSize = 1f;

	public Color CenterCrosshairColor = MMColors.Wheat;

	[Header("Ratios")]
	public bool DrawRatios = true;

	public float CameraSize = 5f;

	public float UnsafeZonesOpacity = 0.2f;

	public List<Ratio> Ratios;

	[MMInspectorButton("AutoSetup")]
	public bool AutoSetupButton;

	public virtual void AutoSetup()
	{
		Ratios.Clear();
		Ratios.Add(new Ratio(drawRatio: true, new Vector2(16f, 9f), MMColors.DeepSkyBlue));
		Ratios.Add(new Ratio(drawRatio: true, new Vector2(16f, 10f), MMColors.GreenYellow));
		Ratios.Add(new Ratio(drawRatio: true, new Vector2(4f, 3f), MMColors.HotPink));
	}
}
