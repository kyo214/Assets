using System.IO;
using ICSharpCode.SharpZipLib.GZip;
using NPOI.Util;
using NPOI.XSSF.Model;

namespace NPOI.XSSF.Streaming;

public class GZIPSheetDataWriter : SheetDataWriter
{
	public GZIPSheetDataWriter()
	{
	}

	public GZIPSheetDataWriter(SharedStringsTable sharedStringsTable)
		: base(sharedStringsTable)
	{
	}

	public override FileInfo CreateTempFile()
	{
		return TempFile.CreateTempFile("poi-sxssf-sheet-xml", ".gz");
	}

	protected override Stream DecorateInputStream(Stream fis)
	{
		return new GZipInputStream(fis);
	}

	protected override Stream DecorateOutputStream(Stream fos)
	{
		return new GZipOutputStream(fos);
	}
}
