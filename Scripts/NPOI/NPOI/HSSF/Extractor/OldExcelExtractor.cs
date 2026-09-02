using System;
using System.IO;
using System.Text;
using NPOI.HSSF.Model;
using NPOI.HSSF.Record;
using NPOI.POIFS.FileSystem;
using NPOI.SS.UserModel;
using NPOI.Util;

namespace NPOI.HSSF.Extractor;

public class OldExcelExtractor
{
	private RecordInputStream ris;

	private ICloseable toClose;

	private Stream toCloseStream;

	private int biffVersion;

	private int fileType;

	public int BiffVersion => biffVersion;

	public int FileType => fileType;

	public string Text
	{
		get
		{
			StringBuilder stringBuilder = new StringBuilder();
			CodepageRecord codePage = null;
			while (ris.HasNextRecord)
			{
				int nextSid = ris.GetNextSid();
				ris.NextRecord();
				switch (nextSid)
				{
				case 133:
				{
					OldSheetRecord oldSheetRecord = new OldSheetRecord(ris);
					oldSheetRecord.SetCodePage(codePage);
					stringBuilder.Append("Sheet: ");
					stringBuilder.Append(oldSheetRecord.Sheetname);
					stringBuilder.Append('\n');
					break;
				}
				case 4:
				case 516:
				{
					OldLabelRecord oldLabelRecord = new OldLabelRecord(ris);
					oldLabelRecord.SetCodePage(codePage);
					stringBuilder.Append(oldLabelRecord.Value);
					stringBuilder.Append('\n');
					break;
				}
				case 7:
				case 519:
				{
					OldStringRecord oldStringRecord = new OldStringRecord(ris);
					oldStringRecord.SetCodePage(codePage);
					stringBuilder.Append(oldStringRecord.GetString());
					stringBuilder.Append('\n');
					break;
				}
				case 515:
				{
					NumberRecord numberRecord = new NumberRecord(ris);
					handleNumericCell(stringBuilder, numberRecord.Value);
					break;
				}
				case 6:
				case 518:
				case 1030:
					if (biffVersion == 5)
					{
						FormulaRecord formulaRecord = new FormulaRecord(ris);
						if (formulaRecord.CachedResultType == CellType.Numeric)
						{
							handleNumericCell(stringBuilder, formulaRecord.Value);
						}
					}
					else
					{
						OldFormulaRecord oldFormulaRecord = new OldFormulaRecord(ris);
						if (oldFormulaRecord.GetCachedResultType() == CellType.Numeric)
						{
							handleNumericCell(stringBuilder, oldFormulaRecord.Value);
						}
					}
					break;
				case 638:
				{
					RKRecord rKRecord = new RKRecord(ris);
					handleNumericCell(stringBuilder, rKRecord.RKNumber);
					break;
				}
				case 66:
					codePage = new CodepageRecord(ris);
					break;
				default:
					ris.ReadFully(new byte[ris.Remaining]);
					break;
				}
			}
			Close();
			ris = null;
			return stringBuilder.ToString();
		}
	}

	public OldExcelExtractor(Stream input)
	{
		Open(input);
	}

	public OldExcelExtractor(FileInfo f)
	{
		NPOIFSFileSystem nPOIFSFileSystem = null;
		try
		{
			nPOIFSFileSystem = (NPOIFSFileSystem)(toClose = new NPOIFSFileSystem(f));
			Open(nPOIFSFileSystem);
			return;
		}
		catch (OldExcelFormatException)
		{
			nPOIFSFileSystem?.Close();
		}
		catch (NotOLE2FileException)
		{
			nPOIFSFileSystem?.Close();
		}
		FileStream fileStream = f.OpenRead();
		try
		{
			Open(fileStream);
		}
		catch (IOException ex3)
		{
			fileStream.Close();
			throw ex3;
		}
		catch (RuntimeException ex4)
		{
			fileStream.Close();
			throw ex4;
		}
	}

	public OldExcelExtractor(NPOIFSFileSystem fs)
	{
		Open(fs);
	}

	public OldExcelExtractor(DirectoryNode directory)
	{
		Open(directory);
	}

	private void Open(Stream biffStream)
	{
		BufferedStream bufferedStream = ((biffStream is BufferedStream) ? ((BufferedStream)biffStream) : new BufferedStream(biffStream, 8));
		if (NPOIFSFileSystem.HasPOIFSHeader(bufferedStream))
		{
			NPOIFSFileSystem nPOIFSFileSystem = new NPOIFSFileSystem(bufferedStream);
			try
			{
				Open(nPOIFSFileSystem);
				return;
			}
			finally
			{
				nPOIFSFileSystem.Close();
			}
		}
		ris = new RecordInputStream(bufferedStream);
		toCloseStream = bufferedStream;
		Prepare();
	}

	private void Open(NPOIFSFileSystem fs)
	{
		Open(fs.Root);
	}

	private void Open(DirectoryNode directory)
	{
		DocumentNode documentNode;
		try
		{
			documentNode = (DocumentNode)directory.GetEntry(InternalWorkbook.OLD_WORKBOOK_DIR_ENTRY_NAME);
		}
		catch (FileNotFoundException)
		{
			documentNode = (DocumentNode)directory.GetEntry(InternalWorkbook.WORKBOOK_DIR_ENTRY_NAMES[0]);
		}
		if (documentNode == null)
		{
			throw new IOException("No Excel 5/95 Book stream found");
		}
		ris = new RecordInputStream(directory.CreateDocumentInputStream(documentNode));
		Prepare();
	}

	public static void main(string[] args)
	{
		if (args.Length < 1)
		{
			Console.WriteLine("Use:");
			Console.WriteLine("   OldExcelExtractor <filename>");
		}
		else
		{
			OldExcelExtractor oldExcelExtractor = new OldExcelExtractor(new FileInfo(args[0]));
			Console.WriteLine(oldExcelExtractor.Text);
			oldExcelExtractor.Close();
		}
	}

	private void Prepare()
	{
		if (!ris.HasNextRecord)
		{
			throw new ArgumentException("File Contains no records!");
		}
		ris.NextRecord();
		int sid = ris.Sid;
		switch (sid)
		{
		case 9:
			biffVersion = 2;
			break;
		case 521:
			biffVersion = 3;
			break;
		case 1033:
			biffVersion = 4;
			break;
		case 2057:
			biffVersion = 5;
			break;
		default:
			throw new ArgumentException("File does not begin with a BOF, found sid of " + sid);
		}
		BOFRecord bOFRecord = new BOFRecord(ris);
		fileType = (int)bOFRecord.Type;
	}

	protected void handleNumericCell(StringBuilder text, double value)
	{
		text.Append(value);
		text.Append('\n');
	}

	public void Close()
	{
		if (toClose != null)
		{
			IOUtils.CloseQuietly(toClose);
			toClose = null;
		}
		if (toCloseStream != null)
		{
			IOUtils.CloseQuietly(toCloseStream);
			toClose = null;
		}
	}
}
