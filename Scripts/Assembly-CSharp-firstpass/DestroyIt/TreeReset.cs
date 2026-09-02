using System;
using UnityEngine;

namespace DestroyIt;

[Serializable]
public class TreeReset
{
	public int prototypeIndex;

	public Vector3 position;

	public DateTime resetTime;

	public Color color;

	public float heightScale;

	public float widthScale;
}
