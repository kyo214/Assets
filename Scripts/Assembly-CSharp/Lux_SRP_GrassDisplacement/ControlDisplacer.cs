using System;
using UnityEngine;

namespace Lux_SRP_GrassDisplacement;

[ExecuteInEditMode]
public class ControlDisplacer : MonoBehaviour
{
	public float maxDistance = 1f;

	public float fallOff = 2f;

	[Layer]
	public int layerMask;

	[Space(5f)]
	public bool DebugRay = true;

	private Transform trans;

	private Renderer rend;

	private MaterialPropertyBlock mpb;

	private RaycastHit hit;

	private float alpha;

	private void OnEnable()
	{
		trans = GetComponent<Transform>();
		rend = GetComponent<Renderer>();
		mpb = new MaterialPropertyBlock();
		mpb.Clear();
		rend.SetPropertyBlock(mpb);
	}

	private void OnDisable()
	{
		mpb.Clear();
		rend.SetPropertyBlock(null);
	}

	private void Update()
	{
		int num = 1 << layerMask;
		if (Physics.Raycast(trans.position, Vector3.down, out hit, maxDistance, num))
		{
			alpha = (float)(1.0 - Math.Pow(hit.distance / maxDistance, fallOff));
			mpb.SetFloat("_Alpha", alpha);
			rend.SetPropertyBlock(mpb);
		}
		else if (alpha != 0f)
		{
			alpha = 0f;
			mpb.SetFloat("_Alpha", 0f);
			rend.SetPropertyBlock(mpb);
		}
	}
}
