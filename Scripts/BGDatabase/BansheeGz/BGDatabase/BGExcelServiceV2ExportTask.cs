namespace BansheeGz.BGDatabase;

public class BGExcelServiceV2ExportTask
{
	public BGLogger logger;

	public byte[] content;

	public BGRepo repo;

	public BGMergeSettingsEntity modelSettingsEntity;

	public bool transferRowsOrder;

	public bool useXml;

	public BGSyncNameMapConfig nameMapConfig;
}
