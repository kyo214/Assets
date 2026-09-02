using System;

namespace BansheeGz.BGDatabase;

public class BGSyncA
{
	protected readonly BGLogger Logger;

	protected readonly BGRepo MainRepo;

	protected readonly BGMergeSettingsEntity EntitySettings;

	protected readonly BGMergeSettingsMeta MetaSettings;

	protected readonly BGSyncNameMapConfig NameMapConfig;

	protected readonly BGSyncIdConfig IdConfig;

	protected readonly BGSyncRelationsConfig RelationsConfig;

	protected readonly bool PrintWarnings;

	protected BGSyncA(BGLogger logger, BGRepo mainRepo, BGMergeSettingsEntity entitySettings, BGMergeSettingsMeta metaSettings)
		: this(logger, mainRepo, entitySettings, metaSettings, null)
	{
	}

	protected BGSyncA(BGLogger logger, BGRepo mainRepo, BGMergeSettingsEntity entitySettings, BGMergeSettingsMeta metaSettings, BGSyncNameMapConfig nameMapConfig)
		: this(logger, mainRepo, entitySettings, metaSettings, nameMapConfig, null)
	{
	}

	protected BGSyncA(BGLogger logger, BGRepo mainRepo, BGMergeSettingsEntity entitySettings, BGMergeSettingsMeta metaSettings, BGSyncNameMapConfig nameMapConfig, BGSyncIdConfig idConfig)
		: this(logger, mainRepo, entitySettings, metaSettings, nameMapConfig, idConfig, null, printWarnings: false)
	{
	}

	protected BGSyncA(BGLogger logger, BGRepo mainRepo, BGMergeSettingsEntity entitySettings, BGMergeSettingsMeta metaSettings, BGSyncNameMapConfig nameMapConfig, BGSyncIdConfig idConfig, BGSyncRelationsConfig relationsConfig, bool printWarnings)
	{
		Logger = logger ?? new BGLogger(useRichText: false);
		MainRepo = mainRepo;
		EntitySettings = entitySettings;
		MetaSettings = metaSettings;
		NameMapConfig = nameMapConfig;
		IdConfig = idConfig;
		RelationsConfig = relationsConfig;
		PrintWarnings = printWarnings;
	}

	protected void Export(bool forceTransferMode, Action<BGRepo> setup, Func<BGRepo, bool, BGRepo> read, Action<bool, BGRepo, BGRepo> write)
	{
		BGRepo repo = EntitySettings.NewRepo(MainRepo, copyValues: false);
		setup?.Invoke(repo);
		if (forceTransferMode || (EntitySettings.Mode == BGMergeModeEnum.Transfer && (MetaSettings.Mode == BGMergeModeEnum.Transfer || !MetaSettings.HasAny)))
		{
			new BGMergerEntity(Logger, new BGRepo(MainRepo, copyValues: true), repo, EntitySettings).Merge();
			BGRepo bGRepo = null;
			if (MetaSettings.HasAny)
			{
				bGRepo = new BGRepo();
				new BGMergerMeta(Logger, new BGRepo(MainRepo), bGRepo, MetaSettings).Merge();
			}
			write(arg1: true, repo, bGRepo);
			return;
		}
		bool mergeMeta = MetaSettings.HasAny;
		BGRepo metaRepo = null;
		Logger.Section("Reading repo", () =>
		{
			metaRepo = read(repo, mergeMeta);
		});
		if (mergeMeta)
		{
			if (MetaSettings.Mode == BGMergeModeEnum.Merge && metaRepo != null)
			{
				new BGMergerMeta(Logger, MainRepo, metaRepo, MetaSettings).Merge();
			}
			else
			{
				metaRepo = MainRepo;
			}
		}
		new BGMergerEntity(Logger, MainRepo, repo, EntitySettings).Merge();
		write(arg1: false, repo, metaRepo);
	}

	public void Import(bool updateNewIds, bool transferRowsOrder, Action<BGRepo> setup, Func<BGRepo, BGRepo> readMeta, Action<BGRepo> readEntity, Action write, Func<BGRepo, BGBookInfo> readerInfo)
	{
		MainRepo.Transaction(() =>
		{
			BGRepo repo = ((EntitySettings.Mode == BGMergeModeEnum.Transfer) ? new BGRepo(MainRepo) : EntitySettings.NewRepo(MainRepo, copyValues: false));
			setup?.Invoke(repo);
			if (MetaSettings.HasAny)
			{
				BGRepo metaRepo = null;
				Logger.Section("Reading meta", () =>
				{
					metaRepo = readMeta(repo);
					Logger.AppendLine((metaRepo == null) ? "Can not read meta due to the errors above." : "Meta read ok");
				});
				if (metaRepo != null)
				{
					new BGMergerMeta(Logger, metaRepo, MainRepo, MetaSettings).Merge();
				}
			}
			Logger.Section("Reading entities", () =>
			{
				readEntity(repo);
			});
			if (updateNewIds && write != null)
			{
				write();
			}
			if (EntitySettings.Mode == BGMergeModeEnum.Transfer)
			{
				MainRepo.Merge(repo);
				MainRepo.Addons.ForEachAddon((BGAddon addon) =>
				{
					addon.OnTransfer(repo);
				});
			}
			else
			{
				BGMergeSettingsEntity settingsClone = (BGMergeSettingsEntity)EntitySettings.Clone();
				settingsClone.RemoveNotExistent(repo, readerInfo(repo));
				new BGMergerEntity(Logger, repo, MainRepo, settingsClone).Merge();
				if (transferRowsOrder)
				{
					repo.ForEachMeta((BGMetaEntity meta) =>
					{
						if (settingsClone.IsMetaIncluded(meta.Id))
						{
							BGMetaEntity targetMeta = MainRepo.GetMeta(meta.Id);
							if (targetMeta != null)
							{
								BGRowsOrder order = new BGRowsOrder(Logger, meta, (int index1, int index2) =>
								{
									targetMeta.SwapEntities(index1, index2);
								});
								targetMeta.ForEachEntity((BGEntity entity) =>
								{
									BGEntity entity2 = meta.GetEntity(entity.Id);
									if (entity2 != null)
									{
										order.Add(new BGRowsOrder.EntityOrderInfo(entity2, entity, entity.Index));
									}
								});
								order.Complete(null);
							}
						}
					});
				}
			}
		});
	}
}
