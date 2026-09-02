namespace NPOI.POIFS.Common;

public class POIFSBigBlockSize
{
	private int bigBlockSize;

	private short headerValue;

	internal POIFSBigBlockSize(int bigBlockSize, short headerValue)
	{
		this.bigBlockSize = bigBlockSize;
		this.headerValue = headerValue;
	}

	public int GetBigBlockSize()
	{
		return bigBlockSize;
	}

	public short GetHeaderValue()
	{
		return headerValue;
	}

	public int GetPropertiesPerBlock()
	{
		return bigBlockSize / 128;
	}

	public int GetBATEntriesPerBlock()
	{
		return bigBlockSize / 4;
	}

	public int GetXBATEntriesPerBlock()
	{
		return GetBATEntriesPerBlock() - 1;
	}

	public int GetNextXBATChainOffset()
	{
		return GetXBATEntriesPerBlock() * 4;
	}
}
