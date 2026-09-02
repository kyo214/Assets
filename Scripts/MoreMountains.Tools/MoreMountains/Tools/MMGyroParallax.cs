using System.Collections.Generic;
using UnityEngine;

namespace MoreMountains.Tools;

public class MMGyroParallax : MMGyroscope
{
	[Header("Cameras")]
	public List<MMGyroCam> Cams;

	protected Vector3 _newAngles;

	protected override void Start()
	{
		base.Start();
		Initialization();
	}

	public virtual void Initialization()
	{
		foreach (MMGyroCam cam in Cams)
		{
			cam.InitialAngles = cam.Cam.transform.localEulerAngles;
			cam.InitialPosition = cam.Cam.transform.position;
		}
	}

	protected override void Update()
	{
		base.Update();
		MoveCameras();
	}

	protected virtual void MoveCameras()
	{
		foreach (MMGyroCam cam in Cams)
		{
			float num = 0f;
			float num2 = 0f;
			Vector3 lerpedCalibratedGyroscopeGravity = MMGyroscope.LerpedCalibratedGyroscopeGravity;
			if (lerpedCalibratedGyroscopeGravity.x > 0f)
			{
				num = MMMaths.Remap(MMGyroscope.LerpedCalibratedGyroscopeGravity.x, 0.5f, 0f, cam.MinRotation.x, 0f);
			}
			if (lerpedCalibratedGyroscopeGravity.x < 0f)
			{
				num = MMMaths.Remap(MMGyroscope.LerpedCalibratedGyroscopeGravity.x, 0f, -0.5f, 0f, cam.MaxRotation.x);
			}
			if (lerpedCalibratedGyroscopeGravity.y > 0f)
			{
				num2 = MMMaths.Remap(MMGyroscope.LerpedCalibratedGyroscopeGravity.y, 0.5f, 0f, cam.MinRotation.y, 0f);
			}
			if (lerpedCalibratedGyroscopeGravity.y < 0f)
			{
				num2 = MMMaths.Remap(MMGyroscope.LerpedCalibratedGyroscopeGravity.y, 0f, -0.5f, 0f, cam.MaxRotation.y);
			}
			Transform transform = cam.Cam.transform;
			if (cam.AnimatedPosition != null)
			{
				_newAngles = cam.AnimatedPosition.localEulerAngles;
				_newAngles.x += num;
				_newAngles.z += num2;
				transform.position = cam.AnimatedPosition.position;
				transform.localEulerAngles = cam.AnimatedPosition.localEulerAngles;
			}
			else
			{
				_newAngles = cam.InitialAngles;
				_newAngles.x += num;
				_newAngles.z += num2;
				transform.position = cam.InitialPosition;
				transform.localEulerAngles = cam.InitialAngles;
			}
			Transform transform2 = cam.RotationCenter.transform;
			transform.RotateAround(transform2.position, transform2.up, num);
			transform.RotateAround(transform2.position, transform2.right, num2);
			if (cam.Cam.LookAt == null)
			{
				if (cam.LookAt != null)
				{
					transform.LookAt(cam.LookAt);
				}
				else
				{
					transform.LookAt(cam.RotationCenter);
				}
			}
		}
	}
}
