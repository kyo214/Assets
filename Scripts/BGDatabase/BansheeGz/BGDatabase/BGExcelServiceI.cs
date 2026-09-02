namespace BansheeGz.BGDatabase;

public interface BGExcelServiceI
{
	byte[] Import(BGLogger logger, byte[] content, BGRepo repo, BGMergeSettingsEntity modelSettingsEntity, BGMergeSettingsMeta mergeSettingsMeta, bool updateNewIds, bool transferRowsOrder, bool useXml);

	byte[] Export(BGLogger logger, byte[] content, BGRepo repo, BGMergeSettingsEntity modelSettingsEntity, BGMergeSettingsMeta mergeSettingsMeta, bool transferRowsOrder, bool useXml);
}
