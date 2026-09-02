using System;

namespace Doozy.Runtime.UIElements;

public struct EdgeValues(float left, float top, float right, float bottom) : IEquatable<EdgeValues>
{
	public float Left = left;

	public float Top = top;

	public float Right = right;

	public float Bottom = bottom;

	private static readonly EdgeValues ZeroValues = new EdgeValues(0f, 0f, 0f, 0f);

	private static readonly EdgeValues OneValues = new EdgeValues(1f, 1f, 1f, 1f);

	private static readonly EdgeValues OneLeftRightValues = new EdgeValues(1f, 0f, 1f, 0f);

	private static readonly EdgeValues OneTopBottomValues = new EdgeValues(0f, 1f, 0f, 1f);

	private static readonly EdgeValues TwoValues = new EdgeValues(2f, 2f, 2f, 2f);

	private static readonly EdgeValues TwoLeftRightValues = new EdgeValues(2f, 0f, 2f, 0f);

	private static readonly EdgeValues TwoTopBottomValues = new EdgeValues(0f, 2f, 0f, 2f);

	private static readonly EdgeValues ThreeValues = new EdgeValues(3f, 3f, 3f, 3f);

	private static readonly EdgeValues ThreeLeftRightValues = new EdgeValues(3f, 0f, 3f, 0f);

	private static readonly EdgeValues ThreeTopBottomValues = new EdgeValues(0f, 3f, 0f, 3f);

	private static readonly EdgeValues FourValues = new EdgeValues(4f, 4f, 4f, 4f);

	private static readonly EdgeValues FourLeftRightValues = new EdgeValues(4f, 0f, 4f, 0f);

	private static readonly EdgeValues FourTopBottomValues = new EdgeValues(0f, 4f, 0f, 4f);

	public static EdgeValues zero => ZeroValues;

	public static EdgeValues one => OneValues;

	public static EdgeValues oneLeftRight => OneLeftRightValues;

	public static EdgeValues oneTopBottom => OneTopBottomValues;

	public static EdgeValues two => TwoValues;

	public static EdgeValues twoLeftRight => TwoLeftRightValues;

	public static EdgeValues twoTopBottom => TwoTopBottomValues;

	public static EdgeValues three => ThreeValues;

	public static EdgeValues threeLeftRight => ThreeLeftRightValues;

	public static EdgeValues threeTopBottom => ThreeTopBottomValues;

	public static EdgeValues four => FourValues;

	public static EdgeValues fourLeftRight => FourLeftRightValues;

	public static EdgeValues fourTopBottom => FourTopBottomValues;

	public EdgeValues Set(float newLeft, float newTop, float newRight, float newBottom)
	{
		Left = newLeft;
		Top = newTop;
		Right = newRight;
		Bottom = newBottom;
		return this;
	}

	public EdgeValues SetLeft(float newLeft)
	{
		Left = newLeft;
		return this;
	}

	public EdgeValues SetTop(float newTop)
	{
		Top = newTop;
		return this;
	}

	public EdgeValues SetRight(float newRight)
	{
		Right = newRight;
		return this;
	}

	public EdgeValues SetBottom(float newBottom)
	{
		Bottom = newBottom;
		return this;
	}

	public bool Equals(EdgeValues other)
	{
		if ((double)Left == (double)other.Left && (double)Top == (double)other.Top && (double)Right == (double)other.Right)
		{
			return (double)Bottom == (double)other.Bottom;
		}
		return false;
	}

	public override bool Equals(object obj)
	{
		if (obj is EdgeValues other)
		{
			return Equals(other);
		}
		return false;
	}

	public override int GetHashCode()
	{
		return (((((Left.GetHashCode() * 397) ^ Top.GetHashCode()) * 397) ^ Right.GetHashCode()) * 397) ^ Bottom.GetHashCode();
	}

	public static EdgeValues operator +(EdgeValues a, EdgeValues b)
	{
		return new EdgeValues(a.Left + b.Left, a.Top + b.Top, a.Right + b.Right, a.Bottom + b.Bottom);
	}

	public static EdgeValues operator -(EdgeValues a, EdgeValues b)
	{
		return new EdgeValues(a.Left - b.Left, a.Top - b.Top, a.Right - b.Right, a.Bottom - b.Bottom);
	}

	public static EdgeValues operator -(EdgeValues a)
	{
		return new EdgeValues(0f - a.Left, 0f - a.Top, 0f - a.Right, 0f - a.Bottom);
	}

	public static EdgeValues operator *(EdgeValues a, float value)
	{
		return new EdgeValues(a.Left * value, a.Top * value, a.Right * value, a.Bottom * value);
	}

	public static EdgeValues operator *(float value, EdgeValues a)
	{
		return new EdgeValues(a.Left * value, a.Top * value, a.Right * value, a.Bottom * value);
	}

	public static EdgeValues operator *(EdgeValues a, int value)
	{
		return new EdgeValues(a.Left * (float)value, a.Top * (float)value, a.Right * (float)value, a.Bottom * (float)value);
	}

	public static EdgeValues operator *(int value, EdgeValues a)
	{
		return new EdgeValues(a.Left * (float)value, a.Top * (float)value, a.Right * (float)value, a.Bottom * (float)value);
	}

	public static EdgeValues operator /(EdgeValues a, float value)
	{
		return new EdgeValues(a.Left / value, a.Top / value, a.Right / value, a.Bottom / value);
	}

	public static EdgeValues operator /(EdgeValues a, int value)
	{
		return new EdgeValues(a.Left / (float)value, a.Top / (float)value, a.Right / (float)value, a.Bottom / (float)value);
	}

	public static bool operator ==(EdgeValues a, EdgeValues b)
	{
		float num = a.Left - b.Left;
		float num2 = a.Top - b.Top;
		float num3 = a.Right - b.Right;
		float num4 = a.Bottom - b.Bottom;
		return (double)num * (double)num + (double)num2 * (double)num2 + (double)num3 * (double)num3 + (double)num4 * (double)num4 < 9.99999943962493E-11;
	}

	public static bool operator !=(EdgeValues a, EdgeValues b)
	{
		return !(a == b);
	}

	public override string ToString()
	{
		return $"({Left}, {Top}, {Right}, {Bottom})";
	}

	public string ToString(bool verbose)
	{
		if (!verbose)
		{
			return $"({Left}, {Top}, {Right}, {Bottom})";
		}
		return $"(left: {Left}, top: {Top}, right: {Right}, bottom: {Bottom})";
	}
}
