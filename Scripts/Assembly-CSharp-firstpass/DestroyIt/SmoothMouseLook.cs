using System.Collections.Generic;
using UnityEngine;

namespace DestroyIt;

public class SmoothMouseLook : MonoBehaviour
{
	public float sensitivityX = 2f;

	public float sensitivityY = 2f;

	public float minimumY = -60f;

	public float maximumY = 60f;

	public int frameCounterX = 20;

	public int frameCounterY = 20;

	private float rotationX;

	private float rotationY;

	private Quaternion xQuaternion;

	private Quaternion yQuaternion;

	private Quaternion origRotation;

	private List<float> rotationsX = new List<float>();

	private List<float> rotationsY = new List<float>();

	private void Start()
	{
		Cursor.lockState = CursorLockMode.Locked;
		if ((bool)GetComponent<Rigidbody>())
		{
			GetComponent<Rigidbody>().freezeRotation = true;
		}
		origRotation = base.transform.localRotation;
	}

	private void Update()
	{
		if (Cursor.lockState == CursorLockMode.Locked)
		{
			rotationX += Input.GetAxis("Mouse X") * sensitivityX;
			rotationsX.Add(rotationX);
			rotationY += Input.GetAxis("Mouse Y") * sensitivityY;
			rotationY = ClampAngle(rotationY, minimumY, maximumY);
			rotationsY.Add(rotationY);
			float angle = AverageRotations(rotationsX, frameCounterX);
			float angle2 = AverageRotations(rotationsY, frameCounterY);
			xQuaternion = Quaternion.AngleAxis(angle, Vector3.up);
			yQuaternion = Quaternion.AngleAxis(angle2, Vector3.left);
			base.transform.localRotation = origRotation * xQuaternion * yQuaternion;
		}
		else
		{
			Cursor.visible = true;
		}
	}

	private static float AverageRotations(List<float> rotations, int frameCounter)
	{
		float num = 0f;
		if (rotations.Count >= frameCounter)
		{
			rotations.RemoveAt(0);
		}
		for (int i = 0; i < rotations.Count; i++)
		{
			num += rotations[i];
		}
		return num / (float)rotations.Count;
	}

	private float ClampAngle(float angle, float min, float max)
	{
		if (angle < -360f)
		{
			angle += 360f;
		}
		if (angle > 360f)
		{
			angle -= 360f;
		}
		return Mathf.Clamp(angle, min, max);
	}
}
