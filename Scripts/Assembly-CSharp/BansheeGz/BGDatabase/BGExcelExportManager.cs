using System;
using System.IO;
using NPOI.SS.UserModel;

namespace BansheeGz.BGDatabase;

public class BGExcelExportManager
{
	private BGLogger Logger;

	public BGExcelExportManager()
	{
	}

	public BGExcelExportManager(bool ignoreErrors)
	{
	}

	public BGLogger Export(string path, bool exportMetaOnlyIfSheetExists, BGMergeSettingsEntity settings, BGSyncNameMapConfig NameMapConfig, BGSyncIdConfig idConfig, BGSyncRelationsConfig relationsConfig, bool printWarnings)
	{
		Logger = new BGLogger();
		ExportTo(path, exportMetaOnlyIfSheetExists, settings ?? new BGMergeSettingsEntity
		{
			Mode = BGMergeModeEnum.Merge,
			UpdateMatching = true,
			AddMissing = true
		}, NameMapConfig, idConfig, relationsConfig, printWarnings);
		return Logger;
	}

	private void ExportTo(string file, bool exportMetaOnlyIfSheetExists, BGMergeSettingsEntity settings, BGSyncNameMapConfig NameMapConfig, BGSyncIdConfig idConfig, BGSyncRelationsConfig relationsConfig, bool printWarnings)
	{
		BGRepo repo = settings.NewRepo(BGRepo.I, copyValues: false);
		BGExcelReaderRT reader = null;
		if (File.Exists(file))
		{
			Logger.Section("Export: Reading repo", () =>
			{
				reader = new BGExcelReaderRT(Logger, File.ReadAllBytes(file), BGExcelImportManager.IsUsingXml(file), NameMapConfig, (idConfig == null) ? null : new BGExcelIdResolverFactoryRT(BGRepo.I, idConfig), new BGSyncRelationsResolver(relationsConfig, idConfig, BGRepo.I)
				{
					Repo = repo
				}, printWarnings);
				reader.ReadEntities(repo, ignoreNew: true);
				if (exportMetaOnlyIfSheetExists)
				{
					settings.AddMissing = false;
					settings.UpdateMatching = false;
					repo.ForEachMeta((BGMetaEntity meta) =>
					{
						if (reader.Info.HasEntitySheet(meta.Id))
						{
							BGMergeSettingsEntity.MetaSettings metaSettings = settings.Ensure(meta.Id);
							bool addMissing = (metaSettings.UpdateMatching = true);
							metaSettings.AddMissing = addMissing;
						}
					});
					if (repo.CountMeta == 0)
					{
						throw new Exception("You chose to export only if sheet for meta exists in xls file, but no sheet for existing meta was found in the file- so no meta to export");
					}
				}
			});
		}
		else if (exportMetaOnlyIfSheetExists)
		{
			throw new Exception("You chose to export only if sheet for meta exists in xls file, but xls file does not exist- so no meta to export");
		}
		new BGMergerEntity(Logger, BGRepo.I, repo, settings).Merge();
		BGSyncRelationsResolver relationsResolver = new BGSyncRelationsResolver(relationsConfig, idConfig, BGRepo.I)
		{
			Repo = repo
		};
		WriteWorkbook(file, (reader == null) ? new BGExcelWriterRT(Logger, BGRepo.I, repo, BGExcelImportManager.IsUsingXml(file), transferRowsOrder: false, NameMapConfig, idConfig, relationsResolver, printWarnings).Book : new BGExcelWriterRT(Logger, BGRepo.I, repo, settings, reader.Book, reader.Info, transferRowsOrder: false, NameMapConfig, idConfig, relationsResolver, printWarnings).Book);
	}

	public static void WriteWorkbook(string path, IWorkbook newBook)
	{
		using MemoryStream memoryStream = new MemoryStream();
		newBook.Write(memoryStream);
		File.WriteAllBytes(path, memoryStream.ToArray());
	}
}
