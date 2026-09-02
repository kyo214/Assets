using System;
using System.Collections.Generic;

namespace BansheeGz.BGDatabase;

public class BGLiveUpdateDataSourceUrls : BGLiveUpdateDataSourceA
{
	private readonly BGLiveUpdateContext context;

	private readonly BGLiveUpdateUrls urls;

	private BGLiveUpdateDataSourceGoogleSheetsAPI jsonDataSource;

	private BGLiveUpdateDataSourceChartsAPI csvDataSource;

	private readonly Dictionary<BGId, BGLiveUpdateUrl> meta2Url = new Dictionary<BGId, BGLiveUpdateUrl>();

	private readonly BGLiveUpdateContext contextClone;

	public BGLiveUpdateDataSourceUrls(BGLiveUpdateContext context, BGLiveUpdateUrls urls)
		: base(context)
	{
		this.context = context;
		contextClone = context.Clone();
		contextClone.loader = loader;
		this.urls = urls;
		if (urls?.urls == null)
		{
			return;
		}
		foreach (BGLiveUpdateUrl url in urls.urls)
		{
			if (!string.IsNullOrEmpty(url.URL) && !string.IsNullOrEmpty(url.MetaId))
			{
				BGId bGId = BGId.Parse(url.MetaId);
				if (context.Repo.HasMeta(bGId))
				{
					meta2Url[bGId] = url;
				}
			}
		}
	}

	public override void Load(BGMetaEntity meta, BGMergeSettingsEntity actualSettings, BGLiveUpdateUrl notUsedUrl = null, bool applyLastDataOnFailure = false)
	{
		if (!meta2Url.TryGetValue(meta.Id, out var value))
		{
			return;
		}
		BGLiveUpdateDataSourceA bGLiveUpdateDataSourceA;
		switch (value.URLType)
		{
		case BGLiveUpdateUrlTypeEnum.Json:
		{
			BGLiveUpdateDataSourceGoogleSheetsAPI obj2 = jsonDataSource ?? new BGLiveUpdateDataSourceGoogleSheetsAPI(contextClone);
			BGLiveUpdateDataSourceGoogleSheetsAPI bGLiveUpdateDataSourceGoogleSheetsAPI = obj2;
			jsonDataSource = obj2;
			bGLiveUpdateDataSourceA = bGLiveUpdateDataSourceGoogleSheetsAPI;
			if (applyLastDataOnFailure)
			{
				bGLiveUpdateDataSourceA.LocalFileID = "WSJ";
			}
			break;
		}
		case BGLiveUpdateUrlTypeEnum.Csv:
		{
			BGLiveUpdateDataSourceChartsAPI obj = csvDataSource ?? new BGLiveUpdateDataSourceChartsAPI(contextClone);
			BGLiveUpdateDataSourceChartsAPI bGLiveUpdateDataSourceChartsAPI = obj;
			csvDataSource = obj;
			bGLiveUpdateDataSourceA = bGLiveUpdateDataSourceChartsAPI;
			if (applyLastDataOnFailure)
			{
				bGLiveUpdateDataSourceA.LocalFileID = "WSC";
			}
			break;
		}
		default:
			throw new ArgumentOutOfRangeException("url.URLType");
		}
		bGLiveUpdateDataSourceA.Load(meta, actualSettings, value, applyLastDataOnFailure);
	}
}
