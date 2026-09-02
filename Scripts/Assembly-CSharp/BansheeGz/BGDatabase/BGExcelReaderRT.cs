using System.IO;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;

namespace BansheeGz.BGDatabase;

public class BGExcelReaderRT
{
	private readonly BGBookInfo info = new BGBookInfo();

	private readonly IWorkbook book;

	private readonly BGLogger logger;

	private readonly BGSyncNameMapConfig nameMapConfig;

	private readonly BGExcelIdResolverFactoryRT idResolver;

	private readonly BGSyncRelationsResolver relationsResolver;

	private readonly bool printWarnings;

	public BGSyncRelationsResolver RelationsResolver => relationsResolver;

	public IWorkbook Book => book;

	public BGBookInfo Info => info;

	public BGExcelReaderRT(BGLogger logger, byte[] content, bool useXml, BGSyncNameMapConfig nameMapConfig, BGExcelIdResolverFactoryRT idResolver, BGSyncRelationsResolver relationsResolver, bool printWarnings)
	{
		this.logger = logger;
		this.nameMapConfig = nameMapConfig;
		this.idResolver = idResolver;
		this.relationsResolver = relationsResolver;
		this.printWarnings = printWarnings;
		logger.AppendLine("Trying to read xls file..");
		using (MemoryStream memoryStream = new MemoryStream(content))
		{
			IWorkbook workbook2;
			if (!useXml)
			{
				IWorkbook workbook = new HSSFWorkbook(memoryStream);
				workbook2 = workbook;
			}
			else
			{
				IWorkbook workbook = new XSSFWorkbook((Stream)memoryStream);
				workbook2 = workbook;
			}
			book = workbook2;
		}
		logger.AppendLine("Content is ok. $ sheets found", book.NumberOfSheets);
	}

	public void ReadEntities(BGRepo repo, bool ignoreNew)
	{
		BGExcelSheetReaderEntityRT.ReadEntities(book, info, repo, logger, ignoreNew, nameMapConfig, idResolver, relationsResolver, printWarnings);
	}
}
