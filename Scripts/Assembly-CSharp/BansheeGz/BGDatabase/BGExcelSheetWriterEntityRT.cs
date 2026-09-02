using System;
using System.Collections.Generic;
using System.Globalization;
using NPOI.SS.UserModel;

namespace BansheeGz.BGDatabase;

public class BGExcelSheetWriterEntityRT : BGExcelSheetWriterART
{
	private static readonly Dictionary<string, object> CustomConverter2Object = new Dictionary<string, object>();

	private static readonly Dictionary<Type, Func<BGField, int, double>> Type2Getter = new Dictionary<Type, Func<BGField, int, double>>
	{
		{
			typeof(BGFieldInt),
			(BGField field, int index) => ((BGFieldInt)field)[index]
		},
		{
			typeof(BGFieldFloat),
			(BGField field, int index) => double.Parse(((BGFieldFloat)field)[index].ToString("g7", CultureInfo.InvariantCulture), CultureInfo.InvariantCulture)
		}
	};

	private readonly BGMergeSettingsEntity settings;

	private readonly bool transferRowsOrder;

	private readonly BGRepo sourceRepo;

	private readonly BGSyncNameMapConfig nameMapConfig;

	private readonly BGSyncIdConfig idConfig;

	private readonly BGSyncRelationsResolver relationsResolver;

	private readonly bool printWarnings;

	public BGExcelSheetWriterEntityRT(BGLogger logger, BGRepo sourceRepo, BGRepo repo, IWorkbook book, BGBookInfo bookInfo, BGMergeSettingsEntity settings, bool transferRowsOrder, BGSyncNameMapConfig nameMapConfig, BGSyncIdConfig idConfig, BGSyncRelationsResolver relationsResolver, bool printWarnings)
		: base(logger, repo, book, bookInfo)
	{
		this.sourceRepo = sourceRepo;
		this.settings = settings;
		this.transferRowsOrder = transferRowsOrder;
		this.nameMapConfig = nameMapConfig;
		this.idConfig = idConfig;
		this.relationsResolver = relationsResolver;
		this.printWarnings = printWarnings;
	}

	public void Write()
	{
		repo.ForEachMeta((BGMetaEntity meta) =>
		{
			BGMetaEntity sourceMeta = sourceRepo.GetMeta(meta.Id);
			logger.SubSection(() =>
			{
				string sheetName = ((nameMapConfig != null) ? nameMapConfig.GetName(meta) : meta.Name);
				Sheet(sheetName, settings == null || settings.Mode == BGMergeModeEnum.Transfer, () => bookInfo.GetEntitySheet(meta.Id), () =>
				{
					BGEntitySheetInfo bGEntitySheetInfo = new BGEntitySheetInfo(meta.Id, meta.Name, book.NumberOfSheets - 1)
					{
						SheetName = sheetName
					};
					bookInfo.AddEntitySheet(meta.Id, bGEntitySheetInfo);
					return bGEntitySheetInfo;
				}, (BGEntitySheetInfo info) =>
				{
					logger.SubSection(() =>
					{
						Row(0, () =>
						{
							info.IndexId = MapHeader(meta.Id, "_id", info.IndexId);
							meta.ForEachField((BGField bGField) =>
							{
								if (info.HasField(bGField.Id))
								{
									logger.AppendLine("Field $ column found at index $", bGField.Name, info.GetFieldColumn(bGField.Id));
								}
								else
								{
									info.AddField(bGField.Id, base.NewCellIndex);
									Cell(info.GetFieldColumn(bGField.Id), (nameMapConfig == null) ? bGField.Name : nameMapConfig.GetName(bGField));
									logger.AppendLine("Field $ column not found. Created new column at index $", bGField.Name, info.GetFieldColumn(bGField.Id));
								}
							});
						});
					}, "Mapping for ($) entities.", meta.Name);
					bool isAdding = settings == null || settings.IsAddingMissing(meta.Id);
					bool isUpdating = settings == null || settings.IsUpdatingMatching(meta.Id);
					BGRowsOrder order = ((transferRowsOrder && sourceMeta != null) ? new BGRowsOrder(logger, meta, Swap) : null);
					Tuple<BGField, int, Func<BGField, int, double>, BGSyncRelationResolver>[] fieldsInfo = new Tuple<BGField, int, Func<BGField, int, double>, BGSyncRelationResolver>[meta.CountFields];
					for (int num = 0; num < fieldsInfo.Length; num++)
					{
						BGField field = meta.GetField(num);
						int fieldColumn = info.GetFieldColumn(field.Id);
						Func<BGField, int, double> value = null;
						if (!field.CustomStringFormatSupported)
						{
							Type2Getter.TryGetValue(field.GetType(), out value);
						}
						BGSyncRelationResolver item = null;
						if (field is BGAbstractRelationI)
						{
							item = relationsResolver.GetResolver(field);
						}
						fieldsInfo[num] = new Tuple<BGField, int, Func<BGField, int, double>, BGSyncRelationResolver>(field, fieldColumn, value, item);
					}
					BGSyncDuplicateEntitiesMonitor duplicatesMonitor = BGSyncDuplicateEntitiesMonitor.Get(idConfig, meta);
					meta.ForEachEntity((BGEntity entity) =>
					{
						if ((duplicatesMonitor == null || duplicatesMonitor.Process(entity, logger, printWarnings)) && GetRowIndex(info, entity.Id, isAdding, isUpdating, out var rowIndex))
						{
							Row(rowIndex, () =>
							{
								if (info.IndexId >= 0)
								{
									Cell(info.IndexId, entity.Id.ToString());
								}
								Tuple<BGField, int, Func<BGField, int, double>, BGSyncRelationResolver>[] array = fieldsInfo;
								for (int i = 0; i < array.Length; i++)
								{
									var (bGField2, index, func2, bGSyncRelationResolver2) = array[i];
									if (!bGField2.CustomStringFormatSupported)
									{
										if (bGField2 is BGFieldBool bGFieldBool)
										{
											Cell(index, bGFieldBool[entity.Index]);
											continue;
										}
										if (func2 != null)
										{
											Cell(index, func2(bGField2, entity.Index));
											continue;
										}
									}
									if (bGSyncRelationResolver2 != null)
									{
										Cell(index, bGSyncRelationResolver2.ToExternalFormat(entity.Index));
									}
									else
									{
										string value2 = BGUtil.ToString(bGField2, entity.Index);
										Cell(index, value2);
									}
								}
							});
							if (order != null)
							{
								BGEntity entity2 = sourceMeta.GetEntity(entity.Id);
								if (entity2 != null)
								{
									order.Add(new BGRowsOrder.EntityOrderInfo(entity2, entity, rowIndex));
								}
							}
						}
					});
					order?.Complete(CleanUp);
					if (settings == null || settings.IsRemovingOrphaned(meta.Id))
					{
						Remove(info, (BGId id) => !meta.HasEntity(id));
					}
					logger.AppendLine("$ entities are processed.", meta.CountEntities);
				});
			}, "Writing entities for $ meta", meta.Name);
		});
	}

	public static object GetProcessor(string typeName)
	{
		try
		{
			if (CustomConverter2Object.TryGetValue(typeName, out var value))
			{
				return value;
			}
			Type type = BGUtil.GetType(typeName);
			if (type == null)
			{
				return null;
			}
			value = Activator.CreateInstance(type);
			CustomConverter2Object[typeName] = value;
			return value;
		}
		catch
		{
			return null;
		}
	}

	private void Swap(int index1, int index2)
	{
		if (index1 != index2)
		{
			IRow sourceRow = sheet.GetRow(index1);
			IRow targetRow = sheet.GetRow(index2);
			Swap(sourceRow, targetRow);
		}
	}

	private void CleanUp()
	{
	}

	protected int MapHeader(BGId metaId, string header, int index)
	{
		BGSyncIdConfig.BGSyncIdConfigMeta bGSyncIdConfigMeta = idConfig?.GetMetaConfig(metaId);
		if (bGSyncIdConfigMeta != null && bGSyncIdConfigMeta.configType != BGSyncIdConfig.IdConfigEnum.IdColumn)
		{
			return -1;
		}
		return MapHeader(header, index);
	}

	private static void Swap(IRow sourceRow, IRow targetRow)
	{
		for (int i = sourceRow.FirstCellNum; i < sourceRow.LastCellNum; i++)
		{
			ICell cell = sourceRow.GetCell(i);
			ICell cell2 = targetRow.GetCell(i);
			if (cell == null)
			{
				if (cell2 != null)
				{
					cell = sourceRow.CreateCell(i);
					CopyCell(cell2, cell);
					targetRow.RemoveCell(cell2);
				}
				continue;
			}
			if (cell2 == null)
			{
				cell2 = targetRow.CreateCell(i);
				CopyCell(cell, cell2);
				sourceRow.RemoveCell(cell);
				continue;
			}
			ICell cell3 = cell;
			ICell cell4 = cell2;
			ICellStyle cellStyle = cell2.CellStyle;
			ICellStyle cellStyle2 = cell.CellStyle;
			ICellStyle cellStyle3 = (cell3.CellStyle = cellStyle);
			cellStyle3 = (cell4.CellStyle = cellStyle2);
			ICell cell5 = cell;
			cell4 = cell2;
			IComment cellComment = cell2.CellComment;
			IComment cellComment2 = cell.CellComment;
			IComment comment = (cell5.CellComment = cellComment);
			comment = (cell4.CellComment = cellComment2);
			ICell cell6 = cell;
			cell4 = cell2;
			IHyperlink hyperlink = cell2.Hyperlink;
			IHyperlink hyperlink2 = cell.Hyperlink;
			IHyperlink hyperlink3 = (cell6.Hyperlink = hyperlink);
			hyperlink3 = (cell4.Hyperlink = hyperlink2);
			switch (cell.CellType)
			{
			case CellType.Numeric:
			{
				double numericCellValue = cell.NumericCellValue;
				MoveValue(cell2, cell);
				cell2.SetCellValue(numericCellValue);
				break;
			}
			case CellType.String:
			{
				IRichTextString richStringCellValue = cell.RichStringCellValue;
				MoveValue(cell2, cell);
				cell2.SetCellValue(richStringCellValue);
				break;
			}
			case CellType.Formula:
			{
				string cellFormula = cell.CellFormula;
				MoveValue(cell2, cell);
				cell2.SetCellFormula(cellFormula);
				break;
			}
			case CellType.Blank:
			{
				string stringCellValue = cell.StringCellValue;
				MoveValue(cell2, cell);
				cell2.SetCellValue(stringCellValue);
				break;
			}
			case CellType.Boolean:
			{
				bool booleanCellValue = cell.BooleanCellValue;
				MoveValue(cell2, cell);
				cell2.SetCellValue(booleanCellValue);
				break;
			}
			case CellType.Error:
			{
				byte errorCellValue = cell.ErrorCellValue;
				MoveValue(cell2, cell);
				cell2.SetCellErrorValue(errorCellValue);
				break;
			}
			}
		}
	}

	private static void MoveValue(ICell from, ICell to)
	{
		switch (from.CellType)
		{
		case CellType.Numeric:
			to.SetCellValue(from.NumericCellValue);
			break;
		case CellType.String:
			to.SetCellValue(from.RichStringCellValue);
			break;
		case CellType.Formula:
			to.SetCellFormula(from.CellFormula);
			break;
		case CellType.Blank:
			to.SetCellValue(from.StringCellValue);
			break;
		case CellType.Boolean:
			to.SetCellValue(from.BooleanCellValue);
			break;
		case CellType.Error:
			to.SetCellErrorValue(from.ErrorCellValue);
			break;
		default:
			throw new ArgumentOutOfRangeException();
		}
	}

	private static void CopyCell(ICell sourceCell, ICell targetCell)
	{
		if (sourceCell.CellStyle != null)
		{
			targetCell.CellStyle = sourceCell.CellStyle;
		}
		if (sourceCell.CellComment != null)
		{
			targetCell.CellComment = sourceCell.CellComment;
		}
		if (sourceCell.Hyperlink != null)
		{
			targetCell.Hyperlink = sourceCell.Hyperlink;
		}
		targetCell.SetCellType(sourceCell.CellType);
		switch (sourceCell.CellType)
		{
		case CellType.Numeric:
			targetCell.SetCellValue(sourceCell.NumericCellValue);
			break;
		case CellType.String:
			targetCell.SetCellValue(sourceCell.RichStringCellValue);
			break;
		case CellType.Formula:
			targetCell.SetCellFormula(sourceCell.CellFormula);
			break;
		case CellType.Blank:
			targetCell.SetCellValue(sourceCell.StringCellValue);
			break;
		case CellType.Boolean:
			targetCell.SetCellValue(sourceCell.BooleanCellValue);
			break;
		case CellType.Error:
			targetCell.SetCellErrorValue(sourceCell.ErrorCellValue);
			break;
		}
	}

	private void ClearCells(IRow row)
	{
		sheetInfo.ForEachRow((BGId id, int index) =>
		{
			ClearCell(row, index);
		});
	}

	private static void ClearCell(IRow row, int index)
	{
		ICell cell = row.GetCell(index);
		if (cell != null)
		{
			switch (cell.CellType)
			{
			case CellType.Numeric:
				cell.SetCellValue(0.0);
				break;
			case CellType.String:
				cell.SetCellValue((string)null);
				break;
			case CellType.Boolean:
				cell.SetCellValue(value: false);
				break;
			case CellType.Formula:
			case CellType.Blank:
			case CellType.Error:
				break;
			}
		}
	}
}
