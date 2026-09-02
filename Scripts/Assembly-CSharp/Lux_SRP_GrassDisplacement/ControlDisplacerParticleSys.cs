using System;
using UnityEngine;

namespace Lux_SRP_GrassDisplacement;

[RequireComponent(typeof(ParticleSystem))]
public class ControlDisplacerParticleSys : MonoBehaviour
{
	public float maxDistance = 1f;

	public float fallOff = 2f;

	[Layer]
	public int layerMask;

	[Space(5f)]
	public bool DebugRay = true;

	private Transform trans;

	private ParticleSystem ps;

	private ParticleSystem.MainModule main;

	private RaycastHit hit;

	private float alpha;

	private float min_alpha;

	private float max_alpha;

	private Color min_StartColor;

	private Color max_StartColor;

	private void OnEnable()
	{
		trans = GetComponent<Transform>();
		ps = GetComponent<ParticleSystem>();
		main = ps.main;
		min_StartColor = main.startColor.colorMin;
		min_alpha = min_StartColor.a;
		max_StartColor = main.startColor.colorMax;
		max_alpha = max_StartColor.a;
	}

	private void Update()
	{
		int num = 1 << layerMask;
		if (Physics.Raycast(trans.position, Vector3.down, out hit, maxDistance, num))
		{
			alpha = (float)(1.0 - Math.Pow(hit.distance / maxDistance, fallOff));
			min_StartColor.a = min_alpha * alpha;
			max_StartColor.a = max_alpha * alpha;
			main.startColor = new ParticleSystem.MinMaxGradient(min_StartColor, max_StartColor);
		}
		else if (alpha != 0f)
		{
			alpha = 0f;
			min_StartColor.a = alpha;
			max_StartColor.a = alpha;
			main.startColor = new ParticleSystem.MinMaxGradient(min_StartColor, max_StartColor);
		}
	}
}
