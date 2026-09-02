using System;
using NPOI.Util;

namespace NPOI.HPSF;

public class Thumbnail
{
	public const int OFFSet_CFTAG = 4;

	public const int OFFSet_CF = 8;

	public const int OFFSet_WMFDATA = 20;

	public const int CFTAG_WINDOWS = -1;

	public const int CFTAG_MACINTOSH = -2;

	public const int CFTAG_FMTID = -3;

	public const int CFTAG_NODATA = 0;

	public const int CF_METAFILEPICT = 3;

	public const int CF_DIB = 8;

	public const int CF_ENHMETAFILE = 14;

	[Obsolete]
	public const int CF_BITMAP = 2;

	private byte[] thumbnailData;

	public byte[] ThumbnailData
	{
		get
		{
			return thumbnailData;
		}
		set
		{
			thumbnailData = value;
		}
	}

	public long ClipboardFormatTag => LittleEndian.GetInt(ThumbnailData, 4);

	public Thumbnail()
	{
	}

	public Thumbnail(byte[] thumbnailData)
	{
		this.thumbnailData = thumbnailData;
	}

	public long GetClipboardFormat()
	{
		if (ClipboardFormatTag != -1)
		{
			throw new HPSFException("Clipboard Format Tag of Thumbnail must be CFTAG_WINDOWS.");
		}
		return LittleEndian.GetInt(ThumbnailData, 8);
	}

	public byte[] GetThumbnailAsWMF()
	{
		if (ClipboardFormatTag != -1)
		{
			throw new HPSFException("Clipboard Format Tag of Thumbnail must be CFTAG_WINDOWS.");
		}
		if (GetClipboardFormat() != 3)
		{
			throw new HPSFException("Clipboard Format of Thumbnail must be CF_METAFILEPICT.");
		}
		byte[] array = ThumbnailData;
		int num = array.Length - 20;
		byte[] array2 = new byte[num];
		System.Array.Copy(array, 20, array2, 0, num);
		return array2;
	}
}
