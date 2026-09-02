namespace System.Drawing;

public struct CharacterRange(int First, int Length)
{
	private int first = First;

	private int length = Length;

	public int First
	{
		get
		{
			return first;
		}
		set
		{
			first = value;
		}
	}

	public int Length
	{
		get
		{
			return length;
		}
		set
		{
			length = value;
		}
	}

	public override bool Equals(object obj)
	{
		if (!(obj is CharacterRange characterRange))
		{
			return false;
		}
		return this == characterRange;
	}

	public override int GetHashCode()
	{
		return first ^ length;
	}

	public static bool operator ==(CharacterRange cr1, CharacterRange cr2)
	{
		if (cr1.first == cr2.first)
		{
			return cr1.length == cr2.length;
		}
		return false;
	}

	public static bool operator !=(CharacterRange cr1, CharacterRange cr2)
	{
		if (cr1.first == cr2.first)
		{
			return cr1.length != cr2.length;
		}
		return true;
	}
}
