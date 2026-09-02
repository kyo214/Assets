using System.Runtime.InteropServices;
using UnityEngine;

namespace Fusion;

[StructLayout(LayoutKind.Explicit)]
public struct Rotation
{
	public enum RotationTypes
	{
		Quaternion = 0,
		Euler = 1
	}

	[FieldOffset(0)]
	public readonly RotationTypes Type;

	[FieldOffset(4)]
	private readonly Quaternion quaternion;

	[FieldOffset(4)]
	private readonly Vector3 euler;

	[FieldOffset(4)]
	private readonly float x;

	[FieldOffset(8)]
	private readonly float y;

	[FieldOffset(12)]
	private readonly float z;

	[FieldOffset(16)]
	private readonly float w;

	public Quaternion AsQuaternion => this;

	public Vector3 AsEuler => this;

	public float AsFloatZ => this;

	public Rotation(Quaternion quaternion)
	{
		this = default;
		Type = RotationTypes.Quaternion;
		this.quaternion = quaternion;
	}

	public Rotation(Vector3 euler)
	{
		this = default;
		Type = RotationTypes.Quaternion;
		this.euler = euler;
	}

	public Rotation(float z)
	{
		this = default;
		Type = RotationTypes.Quaternion;
		this.z = z;
	}

	public static implicit operator Quaternion(Rotation rotation)
	{
		if (rotation.Type == RotationTypes.Quaternion)
		{
			return rotation.quaternion;
		}
		return Quaternion.Euler(rotation.euler);
	}

	public static implicit operator Vector3(Rotation rotation)
	{
		RotationTypes type = rotation.Type;
		if (type == RotationTypes.Euler)
		{
			return rotation.euler;
		}
		return rotation.quaternion.eulerAngles;
	}

	public static implicit operator float(Rotation rotation)
	{
		RotationTypes type = rotation.Type;
		if (type == RotationTypes.Euler)
		{
			return rotation.z;
		}
		return rotation.quaternion.eulerAngles.z;
	}

	public static implicit operator Rotation(Quaternion quaternion)
	{
		return new Rotation(quaternion);
	}

	public static implicit operator Rotation(Vector3 euler)
	{
		return new Rotation(euler);
	}

	public static implicit operator Rotation(float z)
	{
		return new Rotation(z);
	}
}
