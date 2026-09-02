using System;

namespace MoreMountains.Tools;

[Serializable]
public class MMPropertyEmitter : MMPropertyPicker
{
	public enum Vector2Options
	{
		X = 0,
		Y = 1
	}

	public enum Vector3Options
	{
		X = 0,
		Y = 1,
		Z = 2
	}

	public enum Vector4Options
	{
		X = 0,
		Y = 1,
		Z = 2,
		W = 3
	}

	public bool ClampMin = true;

	public bool ClampMax = true;

	public Vector2Options Vector2Option;

	public Vector3Options Vector3Option;

	public Vector4Options Vector4Option;

	public float BoolRemapFalse;

	public float BoolRemapTrue = 1f;

	public int IntRemapMinToZero;

	public int IntRemapMaxToOne = 1;

	public float FloatRemapMinToZero;

	public float FloatRemapMaxToOne = 1f;

	public float QuaternionRemapMinToZero;

	public float QuaternionRemapMaxToOne = 360f;

	public float Level;

	public virtual float GetLevel()
	{
		return _propertySetter.GetLevel(this, _targetMMProperty);
	}
}
