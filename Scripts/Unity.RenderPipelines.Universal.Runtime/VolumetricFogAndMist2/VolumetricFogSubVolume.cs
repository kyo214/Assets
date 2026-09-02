using System.Collections.Generic;
using UnityEngine;

namespace VolumetricFogAndMist2;

public class VolumetricFogSubVolume : MonoBehaviour
{
	public VolumetricFogProfile profile;

	public float fadeDistance = 1f;

	public static List<VolumetricFogSubVolume> subVolumes = new List<VolumetricFogSubVolume>();

	private void OnDrawGizmos()
	{
		Gizmos.color = new Color(1f, 1f, 1f, 0.35f);
		Gizmos.DrawWireCube(base.transform.position, base.transform.lossyScale);
	}

	private void OnEnable()
	{
		if (!subVolumes.Contains(this))
		{
			subVolumes.Add(this);
		}
	}

	private void OnDisable()
	{
		if (subVolumes.Contains(this))
		{
			subVolumes.Remove(this);
		}
	}

	public Bounds GetBounds()
	{
		return new Bounds(base.transform.position, base.transform.lossyScale);
	}

	public void SetBounds(Bounds bounds)
	{
		Transform parent = base.transform.parent;
		Vector3 size = bounds.size;
		if (parent != null)
		{
			Vector3 lossyScale = base.transform.parent.lossyScale;
			size.x /= lossyScale.x;
			size.y /= lossyScale.y;
			size.z /= lossyScale.z;
		}
		base.transform.localScale = size;
		base.transform.position = bounds.center;
	}
}
