using System;
using Cinemachine;
using UnityEngine;

namespace MoreMountains.Tools;

[Serializable]
[AddComponentMenu("More Mountains/Tools/Cinemachine/MMGyroCam")]
public class MMGyroCam
{
	public CinemachineVirtualCamera Cam;

	public Transform LookAt;

	public Transform RotationCenter;

	public Vector2 MinRotation = new Vector2(-2f, -2f);

	public Vector2 MaxRotation = new Vector2(2f, 2f);

	public Transform AnimatedPosition;

	[MMReadOnly]
	public Vector3 InitialAngles;

	[MMReadOnly]
	public Vector3 InitialPosition;
}
