using BansheeGz.BGDatabase.Editor;

namespace BansheeGz.BGDatabase;

public interface BGGoogleSheetServiceI
{
	void Init(BGDsGoogleSheets dataSource);

	BGGoogleSheetRefreshTokenServiceI CreateRefreshTokenService();

	void Export(BGLogger logger, BGRepo repo, BGMergeSettingsEntity modelSettingsEntity, BGMergeSettingsMeta mergeSettingsMeta, bool transferRowsOrder);

	void Import(BGLogger logger, BGRepo repo, BGMergeSettingsEntity modelSettingsEntity, BGMergeSettingsMeta mergeSettingsMeta, bool updateNewIds, bool transferRowsOrder);
}
