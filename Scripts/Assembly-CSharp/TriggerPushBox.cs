using System;
using UnityEngine;

public class TriggerPushBox : MonoBehaviour
{
	private enum EnumDir
	{
		NORTH = 0,
		EAST = 1,
		SOUTH = 2,
		WEST = 3
	}

	[SerializeField]
	private EnumDir enumDir;

	public event Action<Collider, Vector3> TriggerEnter;

	private void OnTriggerStay(Collider other)
	{
		Vector3 arg = Vector3.zero;
		switch (enumDir)
		{
		case EnumDir.NORTH:
			arg = Vector3.back;
			break;
		case EnumDir.EAST:
			arg = Vector3.left;
			break;
		case EnumDir.SOUTH:
			arg = Vector3.forward;
			break;
		case EnumDir.WEST:
			arg = Vector3.right;
			break;
		}
		TriggerEnter?.Invoke(other, arg);
	}
}
