using System;
using System.IO;

namespace BansheeGz.BGDatabase;

public class BGExcelImportManager
{
	private BGLogger Logger;

	private BGMergeSettingsEntity EntitySettings;

	private BGMergeSettingsEntity NewEntitySettings => new BGMergeSettingsEntity
	{
		Mode = BGMergeModeEnum.Merge,
		UpdateMatching = true
	};

	public BGExcelImportManager()
	{
	}

	public BGExcelImportManager(bool ignoreErrors)
	{
	}

	public BGLogger Import(string path, BGMergeSettingsEntity settings, BGSyncNameMapConfig NameMapConfig, BGSyncIdConfig idConfig, BGSyncRelationsConfig relationsConfig, bool printWarnings)
	{
		if (!File.Exists(path))
		{
			throw new Exception("File does not exists: " + path);
		}
		BGExcelReaderRT reader = null;
		Logger = new BGLogger();
		EntitySettings = settings ?? NewEntitySettings;
		ReadFile(Logger, path, (byte[] content) =>
		{
			reader = new BGExcelReaderRT(Logger, content, IsUsingXml(path), NameMapConfig, (idConfig == null) ? null : new BGExcelIdResolverFactoryRT(BGRepo.I, idConfig), new BGSyncRelationsResolver(relationsConfig, idConfig, BGRepo.I), printWarnings);
		});
		Import((BGRepo repo) =>
		{
			reader.ReadEntities(repo, ignoreNew: false);
		}, (BGRepo repo) => reader.Info);
		return Logger;
	}

	private void Import(Action<BGRepo> readEntity, Func<BGRepo, BGBookInfo> readerInfo)
	{
		BGRepo repo = EntitySettings.NewRepo(BGRepo.I, copyValues: false);
		Logger.Section("Reading entities", () =>
		{
			readEntity(repo);
		});
		BGMergeSettingsEntity bGMergeSettingsEntity = (BGMergeSettingsEntity)EntitySettings.Clone();
		bGMergeSettingsEntity.RemoveNotExistent(repo, readerInfo(repo));
		new BGMergerEntity(Logger, repo, BGRepo.I, bGMergeSettingsEntity).Merge();
	}

	public static void ReadFile(BGLogger logger, string path, Action<byte[]> action)
	{
		logger.AppendLine("Trying to read file at ($)..", path);
		byte[] array;
		using (FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
		{
			array = new byte[fileStream.Length];
			fileStream.Read(array, 0, array.Length);
		}
		if (!logger.AppendLine(array.Length == 0, "Content of file is empty"))
		{
			logger.AppendLine("File is read successfully. ($) bytes", array.Length);
			action(array);
		}
	}

	public static bool IsUsingXml(string path)
	{
		return path?.Trim().EndsWith(".xlsx") ?? false;
	}
}
