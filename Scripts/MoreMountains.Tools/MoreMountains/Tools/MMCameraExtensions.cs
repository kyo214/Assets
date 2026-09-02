using System;
using UnityEngine;

namespace MoreMountains.Tools;

public static class MMCameraExtensions
{
	public static float MMCameraWorldSpaceWidth(this Camera camera, float depth = 0f)
	{
		if (camera.orthographic)
		{
			return camera.aspect * camera.orthographicSize * 2f;
		}
		float f = camera.fieldOfView * (MathF.PI / 180f);
		return camera.aspect * depth * Mathf.Tan(f);
	}

	public static float MMCameraWorldSpaceHeight(this Camera camera, float depth = 0f)
	{
		if (camera.orthographic)
		{
			return camera.orthographicSize * 2f;
		}
		float f = camera.fieldOfView * (MathF.PI / 180f);
		return depth * Mathf.Tan(f);
	}
}
