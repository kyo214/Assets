namespace NPOI.POIFS.Common;

public class POIFSConstants
{
	public const int SMALLER_BIG_BLOCK_SIZE = 512;

	public static readonly POIFSBigBlockSize SMALLER_BIG_BLOCK_SIZE_DETAILS = new POIFSBigBlockSize(512, 9);

	public const int LARGER_BIG_BLOCK_SIZE = 4096;

	public static readonly POIFSBigBlockSize LARGER_BIG_BLOCK_SIZE_DETAILS = new POIFSBigBlockSize(4096, 12);

	public const int BIG_BLOCK_SIZE = 512;

	public const int MINI_BLOCK_SIZE = 64;

	public const int SMALL_BLOCK_SIZE = 64;

	public const int PROPERTY_SIZE = 128;

	public const int BIG_BLOCK_MINIMUM_DOCUMENT_SIZE = 4096;

	public const int LARGEST_REGULAR_SECTOR_NUMBER = -5;

	public const int FAT_SECTOR_BLOCK = -3;

	public const int DIFAT_SECTOR_BLOCK = -4;

	public const int END_OF_CHAIN = -2;

	public const int UNUSED_BLOCK = -1;

	public static readonly byte[] OOXML_FILE_HEADER = new byte[4] { 80, 75, 3, 4 };

	public static readonly byte[] RAW_XML_FILE_HEADER = new byte[5] { 60, 63, 120, 109, 108 };
}
