using System.IO;
using NPOI.Util;

namespace NPOI.POIFS.Storage;

public class RawDataBlock : ListManagedBlock
{
	private byte[] _data;

	private bool _eof;

	private bool _hasData;

	private static POILogger log = POILogFactory.GetLogger(typeof(RawDataBlock));

	public bool EOF => _eof;

	public bool HasData => _hasData;

	public byte[] Data
	{
		get
		{
			if (!HasData)
			{
				throw new IOException("Cannot return empty data");
			}
			return _data;
		}
	}

	public int BigBlockSize => _data.Length;

	public RawDataBlock(Stream stream)
		: this(stream, 512)
	{
	}

	public RawDataBlock(Stream stream, int blockSize)
	{
		_data = new byte[blockSize];
		int num = IOUtils.ReadFully(stream, _data);
		_hasData = num > 0;
		if (num == -1)
		{
			_eof = true;
		}
		else if (num != blockSize)
		{
			_eof = true;
			string text = " byte" + ((num == 1) ? "" : "s");
			log.Log(7, "Unable to read entire block; " + num + text + " read before EOF; expected " + blockSize + " bytes. Your document was either written by software that ignores the spec, or has been truncated!");
		}
		else
		{
			_eof = false;
		}
	}

	public override string ToString()
	{
		return "RawDataBlock of size " + _data.Length;
	}
}
