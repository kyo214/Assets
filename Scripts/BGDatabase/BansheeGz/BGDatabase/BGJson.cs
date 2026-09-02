using System;
using System.IO;
using System.Text;

namespace BansheeGz.BGDatabase;

public static class BGJson
{
	public static void Export(BGRepo repo, string path, bool skipData = false, bool removeSensitive = false, BGJsonFormatEnum format = BGJsonFormatEnum.Classic)
	{
		File.WriteAllText(path, ExportToString(repo, skipData, removeSensitive, format));
	}

	public static string ExportToString(BGRepo repo, bool skipData = false, bool removeSensitive = false, BGJsonFormatEnum format = BGJsonFormatEnum.Classic)
	{
		return format switch
		{
			BGJsonFormatEnum.Classic => new BGJsonWriter().Write(repo, skipData, removeSensitive ? new Action<BGJsonRepoModel>(RemoveSensitive) : null), 
			BGJsonFormatEnum.CompactRowBased => new BGJsonCompactWriter().Write(repo, new BGJsonCompactRowBased(), skipData, removeSensitive), 
			BGJsonFormatEnum.CompactFieldBased => new BGJsonCompactWriter().Write(repo, new BGJsonCompactFieldBased(), skipData, removeSensitive), 
			_ => throw new ArgumentOutOfRangeException("format", format, null), 
		};
	}

	public static void Import(BGLogger logger, BGRepo targetRepo, string path, bool skipData = false, BGJsonFormatEnum format = BGJsonFormatEnum.Classic)
	{
		BGSyncUtil.ReadFile(logger, path, (byte[] content) =>
		{
			ImportFromString(logger, targetRepo, Encoding.UTF8.GetString(content), skipData, format);
		});
	}

	public static void ImportFromString(BGLogger logger, BGRepo targetRepo, string jsonContent, bool skipData = false, BGJsonFormatEnum format = BGJsonFormatEnum.Classic)
	{
		targetRepo.Transaction(() =>
		{
			targetRepo.Addons.Clear();
			BGRepo bGRepo = format switch
			{
				BGJsonFormatEnum.Classic => new BGJsonReader(jsonContent, skipData).Repo, 
				BGJsonFormatEnum.CompactRowBased => new BGJsonCompactReader(jsonContent, new BGJsonCompactRowBased(), skipData).Repo, 
				BGJsonFormatEnum.CompactFieldBased => new BGJsonCompactReader(jsonContent, new BGJsonCompactFieldBased(), skipData).Repo, 
				_ => throw new ArgumentOutOfRangeException("format", format, null), 
			};
			targetRepo.Addons.AddFrom(bGRepo.Addons);
			targetRepo.Merge(bGRepo);
		});
	}

	private static void RemoveSensitive(BGJsonRepoModel model)
	{
		if (model?.Addons == null || model.Addons.Count == 0)
		{
			return;
		}
		model.Addons.ForEach((BGJsonRepoModel.Addon addon) =>
		{
			if (!(addon.Type != typeof(BGAddonLiveUpdate).FullName))
			{
				addon.Config = "{\"content\" : \"[sensitive]\"}";
			}
		});
	}
}
