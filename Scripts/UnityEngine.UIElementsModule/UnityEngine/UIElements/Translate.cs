using System;
using System.Globalization;

namespace UnityEngine.UIElements;

public struct Translate(Length x, Length y, float z) : IEquatable<Translate>
{
	private Length m_X = x;

	private Length m_Y = y;

	private float m_Z = z;

	private bool m_isNone = false;

	public Length x
	{
		get
		{
			return m_X;
		}
		set
		{
			m_X = value;
		}
	}

	public Length y
	{
		get
		{
			return m_Y;
		}
		set
		{
			m_Y = value;
		}
	}

	public float z
	{
		get
		{
			return m_Z;
		}
		set
		{
			m_Z = value;
		}
	}

	public Translate(Length x, Length y)
		: this(x, y, 0f)
	{
	}

	public static Translate None()
	{
		return new Translate
		{
			m_isNone = true
		};
	}

	internal bool IsNone()
	{
		return m_isNone;
	}

	public static bool operator ==(Translate lhs, Translate rhs)
	{
		return lhs.m_X == rhs.m_X && lhs.m_Y == rhs.m_Y && lhs.m_Z == rhs.m_Z && lhs.m_isNone == rhs.m_isNone;
	}

	public static bool operator !=(Translate lhs, Translate rhs)
	{
		return !(lhs == rhs);
	}

	public bool Equals(Translate other)
	{
		return other == this;
	}

	public override bool Equals(object obj)
	{
		return obj is Translate other && Equals(other);
	}

	public override int GetHashCode()
	{
		return (m_X.GetHashCode() * 793) ^ (m_Y.GetHashCode() * 791) ^ (m_Z.GetHashCode() * 571);
	}

	public override string ToString()
	{
		string text = m_Z.ToString(CultureInfo.InvariantCulture.NumberFormat);
		return m_X.ToString() + " " + m_Y.ToString() + " " + text;
	}
}
