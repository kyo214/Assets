using System;
using UnityEngine;

namespace MoreMountains.Tools;

[Serializable]
public class MMSpawnAroundProperties
{
	public enum MMSpawnAroundShapes
	{
		Sphere = 0,
		Cube = 1
	}

	[Header("Shape")]
	[Tooltip("the shape within which objects should spawn")]
	public MMSpawnAroundShapes Shape;

	[Header("Position")]
	[Tooltip("a Vector3 that specifies the normal to the plane you want to spawn objects on (if you want to spawn objects on the x/z plane, the normal to that plane would be the y axis (0,1,0)")]
	public Vector3 NormalToSpawnPlane = Vector3.up;

	[Tooltip("the minimum distance to the origin of the spawn at which objects can be spawned")]
	[MMEnumCondition("Shape", new int[] { 0 })]
	public float MinimumSphereRadius = 1f;

	[Tooltip("the maximum distance to the origin of the spawn at which objects can be spawned")]
	[MMEnumCondition("Shape", new int[] { 0 })]
	public float MaximumSphereRadius = 2f;

	[Tooltip("the minimum size of the cube's base")]
	[MMEnumCondition("Shape", new int[] { 1 })]
	public Vector3 MinimumCubeBaseSize = Vector3.one;

	[Tooltip("the maximum size of the cube's base")]
	[MMEnumCondition("Shape", new int[] { 1 })]
	public Vector3 MaximumCubeBaseSize = new Vector3(2f, 2f, 2f);

	[Header("NormalAxisOffset")]
	[Tooltip("the minimum offset to apply on the normal axis")]
	public float MinimumNormalAxisOffset;

	[Tooltip("the maximum offset to apply on the normal axis")]
	public float MaximumNormalAxisOffset;

	[Header("NormalAxisOffsetCurve")]
	[Tooltip("whether or not to use a curve to offset the object's spawn position along the spawn plane")]
	public bool UseNormalAxisOffsetCurve;

	[Tooltip("a curve used to define how distance to the origin should be altered (potentially above min/max distance)")]
	[MMCondition("UseNormalAxisOffsetCurve", true)]
	public AnimationCurve NormalOffsetCurve = new AnimationCurve(new Keyframe(0f, 1f), new Keyframe(1f, 1f));

	[Tooltip("the value to which the curve's zero should be remapped to")]
	[MMCondition("UseNormalAxisOffsetCurve", true)]
	public float NormalOffsetCurveRemapZero;

	[Tooltip("the value to which the curve's one should be remapped to")]
	[MMCondition("UseNormalAxisOffsetCurve", true)]
	public float NormalOffsetCurveRemapOne = 1f;

	[Tooltip("whether or not to invert the curve (horizontally)")]
	[MMCondition("UseNormalAxisOffsetCurve", true)]
	public bool InvertNormalOffsetCurve;

	[Header("Rotation")]
	[Tooltip("the minimum random rotation to apply (in degrees)")]
	public Vector3 MinimumRotation = Vector3.zero;

	[Tooltip("the maximum random rotation to apply (in degrees)")]
	public Vector3 MaximumRotation = Vector3.zero;

	[Header("Scale")]
	[Tooltip("the minimum random scale to apply")]
	public Vector3 MinimumScale = Vector3.one;

	[Tooltip("the maximum random scale to apply")]
	public Vector3 MaximumScale = Vector3.one;
}
