namespace System.Xml.Xsl;

internal struct Int32Pair(int left, int right)
{
	private int left = left;

	private int right = right;

	public int Left => left;

	public int Right => right;

	public override bool Equals(object other)
	{
		if (other is Int32Pair int32Pair)
		{
			if (left == int32Pair.left)
			{
				return right == int32Pair.right;
			}
			return false;
		}
		return false;
	}

	public override int GetHashCode()
	{
		return left.GetHashCode() ^ right.GetHashCode();
	}
}
