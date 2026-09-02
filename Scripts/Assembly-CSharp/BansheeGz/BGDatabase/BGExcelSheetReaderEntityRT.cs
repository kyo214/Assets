using System;
using System.Collections.Generic;
using NPOI.SS.UserModel;

namespace BansheeGz.BGDatabase;

public class BGExcelSheetReaderEntityRT : BGExcelSheetReaderART
{
	private readonly BGMetaEntity meta;

	private readonly BGEntitySheetInfo info;

	private readonly BGLogger logger;

	private readonly bool ignoreNew;

	private readonly BGExcelIdResolverART idResolver;

	private readonly bool printWarnings;

	private readonly BGEntitySheetDataInfo data;

	public int RowsCount;

	public int RowsNewCount;

	public int RowsExistingCount;

	public BGEntitySheetInfo Info => info;

	public static void ReadEntities(IWorkbook book, BGBookInfo info, BGRepo repo, BGLogger logger, bool ignoreNew, BGSyncNameMapConfig nameMapConfig, BGExcelIdResolverFactoryRT IdResolverFactory, BGSyncRelationsResolver relationsResolver, bool printWarnings)
	{
		logger.AppendLine("Reading entities: iterating sheets..");
		List<BGExcelSheetReaderEntityRT> readers = new List<BGExcelSheetReaderEntityRT>();
		for (int i = 0; i < book.NumberOfSheets; i++)
		{
			ISheet sheet = book.GetSheetAt(i);
			if (BGSyncUtil.AppendWarning(logger, printWarnings, string.IsNullOrEmpty(sheet.SheetName), "Sheet with empty name at index $", i))
			{
				continue;
			}
			logger.SubSection(() =>
			{
				BGMetaEntity bGMetaEntity = ((nameMapConfig == null) ? repo[sheet.SheetName] : nameMapConfig.Map(repo, sheet.SheetName));
				if (!BGSyncUtil.AppendWarning(logger, printWarnings, bGMetaEntity == null, "Sheet [$] is skipped. No meta with such name found or no proper mapping.", sheet.SheetName) && !BGSyncUtil.AppendWarning(logger, printWarnings, info.HasEntitySheet(bGMetaEntity.Id), "Sheet [$] is skipped. Duplicate name, meta [$] was already been processed.", sheet.SheetName, bGMetaEntity.Name))
				{
					BGExcelSheetReaderEntityRT bGExcelSheetReaderEntityRT;
					if (sheet.PhysicalNumberOfRows == 0)
					{
						logger.AppendLine("Sheet [$] is mapped ok, but no rows found.", sheet.SheetName);
						bGExcelSheetReaderEntityRT = new BGExcelSheetReaderEntityRT(i, bGMetaEntity, ignoreNew, null, logger, sheet.SheetName, null, null, printWarnings);
					}
					else
					{
						logger.AppendLine("Sheet [$] is mapped ok to [$] meta. $ rows found.", sheet.SheetName, bGMetaEntity.Name, sheet.LastRowNum + 1);
						IRow row = sheet.GetRow(0);
						bGExcelSheetReaderEntityRT = new BGExcelSheetReaderEntityRT(i, bGMetaEntity, ignoreNew, row, logger, sheet.SheetName, nameMapConfig, (IdResolverFactory == null) ? new BGExcelIdResolverIdRT(logger) : IdResolverFactory.Create(bGMetaEntity.Id, logger, printWarnings), printWarnings);
					}
					readers.Add(bGExcelSheetReaderEntityRT);
					info.AddEntitySheet(bGMetaEntity.Id, bGExcelSheetReaderEntityRT.Info);
					if (!BGSyncUtil.AppendWarning(logger, printWarnings, !bGExcelSheetReaderEntityRT.Info.HasAnyData, "No columns found for Sheet [$].", sheet.SheetName))
					{
						logger.AppendLine("Read $ rows. $ existing entities. $ new entities. $ rows are skipped.", bGExcelSheetReaderEntityRT.RowsCount, bGExcelSheetReaderEntityRT.RowsExistingCount, bGExcelSheetReaderEntityRT.RowsNewCount, bGExcelSheetReaderEntityRT.RowsCount - bGExcelSheetReaderEntityRT.RowsExistingCount - bGExcelSheetReaderEntityRT.RowsNewCount);
					}
				}
			}, "Reading sheet $", sheet.SheetName);
		}
		logger.SubSection(() =>
		{
			foreach (BGExcelSheetReaderEntityRT item in readers)
			{
				item.FlushData();
			}
			if (relationsResolver != null)
			{
				relationsResolver.Repo = repo;
			}
			foreach (BGExcelSheetReaderEntityRT item2 in readers)
			{
				item2.FlushRelations(relationsResolver);
			}
		}, "Flushing data to database");
	}

	public BGExcelSheetReaderEntityRT(int sheetNumber, BGMetaEntity meta, bool ignoreNew, IRow headersRow, BGLogger logger, string sheetName, BGSyncNameMapConfig nameMapConfig, BGExcelIdResolverART idResolver, bool printWarnings)
	{
		BGExcelSheetReaderEntityRT bGExcelSheetReaderEntityRT = this;
		this.meta = meta;
		this.ignoreNew = ignoreNew;
		info = new BGEntitySheetInfo(meta.Id, meta.Name, sheetNumber)
		{
			SheetName = (sheetName ?? meta.Name)
		};
		this.idResolver = idResolver ?? new BGExcelIdResolverIdRT(logger);
		this.logger = logger;
		this.printWarnings = printWarnings;
		if (headersRow == null)
		{
			return;
		}
		logger.SubSection(() =>
		{
			bGExcelSheetReaderEntityRT.info.PhysicalColumnCount = headersRow.Cells.Count;
			BGExcelSheetReaderART.ForEachCell(headersRow, (int i, ICell cell) =>
			{
				string stringCellValue;
				if (cell.CellType == CellType.Formula)
				{
					if (BGSyncUtil.AppendWarning(logger, printWarnings, cell.CachedFormulaResultType != CellType.String, "[$]->[error:header is formula, but formula type is not a string (type=$)],", i, cell.CachedFormulaResultType.ToString()))
					{
						return;
					}
					stringCellValue = cell.StringCellValue;
				}
				else
				{
					if (BGSyncUtil.AppendWarning(logger, printWarnings, cell.CellType != CellType.String, "[$]->[error:not a string and not a formula],", i))
					{
						return;
					}
					stringCellValue = cell.StringCellValue;
				}
				int columnIndex = cell.ColumnIndex;
				if (!BGSyncUtil.AppendWarning(logger, printWarnings, string.IsNullOrEmpty(stringCellValue), "[$]->[error:empty string],", i))
				{
					if (stringCellValue == "_id")
					{
						logger.AppendLine("[column #$ $]->[_id],", i, "_id");
						bGExcelSheetReaderEntityRT.info.IndexId = columnIndex;
					}
					else
					{
						BGField bGField = ((nameMapConfig == null) ? meta.GetField(stringCellValue, errorIfNotFound: false) : nameMapConfig.Map(meta, stringCellValue));
						if (!BGSyncUtil.AppendWarning(logger, printWarnings, bGField == null, "[column #$ $]->[warning: no field with such name or no proper mapping- skipping,", i, stringCellValue))
						{
							logger.AppendLine("[column #$ $]->[$],", i, stringCellValue, bGField.Name);
							bGExcelSheetReaderEntityRT.info.AddField(bGField.Id, columnIndex);
						}
					}
				}
			});
		}, "Mapping for [$]", meta.Name);
		if (!Info.HasAnyData)
		{
			return;
		}
		List<Tuple<BGField, int>> columns = Info.GetFieldsInfo(meta);
		data = new BGEntitySheetDataInfo(BGEntitySheetInfo.GetFieldsArray(columns));
		BGExcelSheetReaderART.ForEachRowNoHeader(headersRow.Sheet, (IRow row) =>
		{
			if (row != null && row.RowNum != 0)
			{
				bGExcelSheetReaderEntityRT.RowsCount++;
				BGId bGId;
				try
				{
					bGId = idResolver.ResolveId(bGExcelSheetReaderEntityRT, bGExcelSheetReaderEntityRT.info, row);
				}
				catch (Exception ex)
				{
					BGSyncUtil.AppendWarning(logger, printWarnings, "Exception while trying to fetch entity's id, row number=$. Error=$", row.RowNum, ex.Message);
					throw new ExitException();
				}
				if (!bGId.IsEmpty)
				{
					if (bGExcelSheetReaderEntityRT.info.HasRow(bGId))
					{
						BGSyncUtil.AppendWarning(logger, printWarnings, "Duplicate entity found. id=$", bGId);
						throw new ExitException();
					}
					bGExcelSheetReaderEntityRT.info.AddRow(bGId, row.RowNum);
				}
				else if (ignoreNew || idResolver is BGExcelIdFieldResolverIRT || bGExcelSheetReaderEntityRT.IsRowEmpty(row))
				{
					return;
				}
				if (bGId.IsEmpty)
				{
					bGExcelSheetReaderEntityRT.RowsNewCount++;
				}
				else
				{
					bGExcelSheetReaderEntityRT.RowsExistingCount++;
				}
				string[] array = new string[columns.Count];
				for (int i = 0; i < columns.Count; i++)
				{
					var (_, cellnum) = columns[i];
					array[i] = BGExcelSheetReaderART.ReadAsString(row.GetCell(cellnum));
				}
				bGExcelSheetReaderEntityRT.data.AddRow(new BGEntitySheetDataInfo.RowData(bGId, array, row));
			}
		});
	}

	private void FlushData()
	{
		if (data == null)
		{
			return;
		}
		for (int i = 0; i < data.RowsCount; i++)
		{
			BGEntitySheetDataInfo.RowData row = data.GetRow(i);
			BGEntity bGEntity = EnsureEntity((IRow)row.ExtraData, row);
			for (int j = 0; j < data.FieldsCount; j++)
			{
				BGField field = data.GetField(j);
				if (field is BGAbstractRelationI)
				{
					continue;
				}
				string value = row.GetValue(j);
				if (!string.IsNullOrEmpty(value))
				{
					try
					{
						BGUtil.FromString(field, bGEntity.Index, value);
					}
					catch (Exception ex)
					{
						BGSyncUtil.AppendWarning(logger, printWarnings, "Can not fetch field $ value for entity with id=$. Value=$. Error=$", field.Name, row.EntityId, value, ex.Message);
					}
				}
			}
		}
	}

	private void FlushRelations(BGSyncRelationsResolver relationsResolver)
	{
		if (data == null)
		{
			return;
		}
		for (int i = 0; i < data.FieldsCount; i++)
		{
			BGField field = data.GetField(i);
			if (!(field is BGAbstractRelationI))
			{
				continue;
			}
			BGSyncRelationResolver resolver = relationsResolver.GetResolver(field);
			for (int j = 0; j < data.RowsCount; j++)
			{
				BGEntitySheetDataInfo.RowData row = data.GetRow(j);
				string value = row.GetValue(i);
				if (!string.IsNullOrEmpty(value))
				{
					try
					{
						resolver.ToDatabase(j, value);
					}
					catch (Exception ex)
					{
						BGSyncUtil.AppendWarning(logger, printWarnings, "Can not fetch field $ value for entity with id=$. Value=$. Error=$", field.Name, row.EntityId, value, ex.Message);
					}
				}
			}
		}
	}

	private bool IsRowEmpty(IRow row)
	{
		bool hasValue = false;
		info.ForEachField((BGId id, int index) =>
		{
			if (!hasValue)
			{
				hasValue = !BGExcelSheetWriterART.IsCellEmpty(row, index);
			}
		});
		return !hasValue;
	}

	private BGEntity EnsureEntity(IRow row, BGEntitySheetDataInfo.RowData rowsData)
	{
		BGEntity bGEntity;
		if (!rowsData.EntityId.IsEmpty)
		{
			bGEntity = meta.NewEntity(rowsData.EntityId);
		}
		else
		{
			bGEntity = meta.NewEntity();
			rowsData.EntityId = bGEntity.Id;
			if (info.IndexId >= 0)
			{
				ICell obj = row.GetCell(info.IndexId) ?? row.CreateCell(info.IndexId);
				obj.SetCellType(CellType.String);
				obj.SetCellValue(bGEntity.Id.ToString());
			}
		}
		rowsData.Entity = bGEntity;
		return bGEntity;
	}
}
