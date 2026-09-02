using System;
using System.Collections.Generic;
using UnityEngine;

namespace Fusion;

public struct LagCompensatedHit
{
	public HitType Type;

	public GameObject GameObject;

	public Vector3 Normal;

	public Vector3 Point;

	public float Distance;

	public Hitbox Hitbox;

	public Collider Collider;

	internal float _sortAux;

	[Obsolete("Use 'GameObject' instead.")]
	public GameObject Object => GameObject;

	public static explicit operator LagCompensatedHit(RaycastHit raycastHit)
	{
		return new LagCompensatedHit
		{
			Normal = raycastHit.normal,
			Distance = raycastHit.distance,
			Point = raycastHit.point,
			GameObject = raycastHit.collider.gameObject,
			Hitbox = null,
			Collider = raycastHit.collider,
			Type = HitType.PhysX
		};
	}

	internal static LagCompensatedHit FromHitboxHit(ref HitboxHit hitboxHit)
	{
		return new LagCompensatedHit
		{
			Normal = hitboxHit.Normal,
			Distance = hitboxHit.Distance,
			Point = hitboxHit.Point,
			GameObject = hitboxHit.Hitbox.gameObject,
			Hitbox = hitboxHit.Hitbox,
			Collider = null,
			Type = HitType.Hitbox
		};
	}

	internal static void QuickSort(List<LagCompensatedHit> hits, int low, int high)
	{
		if (low >= high)
		{
			return;
		}
		float sortAux = hits[high]._sortAux;
		int num = low;
		LagCompensatedHit value;
		for (int i = low; i < high; i++)
		{
			if (hits[i]._sortAux < sortAux)
			{
				value = hits[num];
				hits[num] = hits[i];
				hits[i] = value;
				num++;
			}
		}
		value = hits[num];
		hits[num] = hits[high];
		hits[high] = value;
		QuickSort(hits, low, num - 1);
		QuickSort(hits, num + 1, high);
	}

	internal static void QuickSortDistance(List<LagCompensatedHit> hits, int low, int high)
	{
		if (low >= high)
		{
			return;
		}
		float distance = hits[high].Distance;
		int num = low;
		LagCompensatedHit value;
		for (int i = low; i < high; i++)
		{
			if (hits[i].Distance < distance)
			{
				value = hits[num];
				hits[num] = hits[i];
				hits[i] = value;
				num++;
			}
		}
		value = hits[num];
		hits[num] = hits[high];
		hits[high] = value;
		QuickSortDistance(hits, low, num - 1);
		QuickSortDistance(hits, num + 1, high);
	}
}
