namespace NPOI.SS.Formula.Functions;

internal class BinarySearchIndexes
{
	private int _lowIx;

	private int _highIx;

	public BinarySearchIndexes(int highIx)
	{
		_lowIx = -1;
		_highIx = highIx;
	}

	public int GetMidIx()
	{
		int num = _highIx - _lowIx;
		if (num < 2)
		{
			return -1;
		}
		return _lowIx + num / 2;
	}

	public int GetLowIx()
	{
		return _lowIx;
	}

	public int GetHighIx()
	{
		return _highIx;
	}

	public void NarrowSearch(int midIx, bool isLessThan)
	{
		if (isLessThan)
		{
			_highIx = midIx;
		}
		else
		{
			_lowIx = midIx;
		}
	}
}
