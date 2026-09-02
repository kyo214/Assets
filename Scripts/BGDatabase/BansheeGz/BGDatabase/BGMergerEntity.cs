using System;
using System.Collections.Generic;

namespace BansheeGz.BGDatabase;

public class BGMergerEntity : BGMergerA
{
	public abstract class MergeResult
	{
	}

	public class MergeResultTransfer : MergeResult
	{
		public int MetaCount;
	}

	public class MergeResultCombine : MergeResult
	{
		public int AddedCount;

		public int AddedSkippedCount;

		public int RemovedCount;

		public int RemovedSkippedCount;

		public int UpdatedCount;

		public int UpdatedSkippedCount;

		public int UpdatedFieldsCount;

		public int UpdatedFieldsSkippedCount;

		public object Controller;
	}

	public interface ParseResultI
	{
		bool HasEntitySheet(BGId metaId);

		bool HasFieldInEntitySheet(BGId metaId, BGId fieldId);
	}

	private readonly BGMergeSettingsEntity settings;

	private readonly List<BGEntity> Missing = new List<BGEntity>();

	private readonly List<BGEntity> Orphaned = new List<BGEntity>();

	private readonly Dictionary<BGEntity, BGEntity> Other2Mine = new Dictionary<BGEntity, BGEntity>();

	private readonly BGIdDictionary<BGEntity> Id2EntityMine = new BGIdDictionary<BGEntity>();

	private readonly BGIdDictionary<BGEntity> Id2EntityOther = new BGIdDictionary<BGEntity>();

	public BGMergerEntity(BGLogger logger, BGRepo from, BGRepo to, BGMergeSettingsEntity settings)
		: base(logger, from, to)
	{
		this.settings = settings;
	}

	public MergeResult Merge()
	{
		MergeResult result = null;
		Section("Merging Entities", () =>
		{
			To.Events.Batch(() =>
			{
				switch (settings?.Mode ?? BGMergeModeEnum.Transfer)
				{
				case BGMergeModeEnum.Transfer:
					result = Transfer();
					break;
				case BGMergeModeEnum.Merge:
					result = Combine();
					break;
				default:
					throw new ArgumentOutOfRangeException();
				}
			});
		});
		return result;
	}

	private MergeResultTransfer Transfer()
	{
		To.Clear();
		To.BinaryFormatVersion = From.BinaryFormatVersion;
		From.ForEachMeta((BGMetaEntity meta) =>
		{
			meta.SwitchTo(To);
		});
		From.ForEachView((BGMetaView view) =>
		{
			view.SwitchTo(To);
		});
		From.Clear();
		AppendLine("$ meta transferred.", To.CountMeta);
		return new MergeResultTransfer
		{
			MetaCount = To.CountMeta
		};
	}

	private MergeResultCombine Combine()
	{
		ClearTempLists();
		MergeResultCombine result = new MergeResultCombine();
		BGMergeSettingsEntity.IMergeReceiver mergeReceiver = null;
		BGMergeSettingsEntity.IAddMissingReceiver addMissingReceiver = null;
		bool addMissingReceiverError = false;
		BGMergeSettingsEntity.IRemoveOrphanedReceiver removeOrphanedReceiver = null;
		bool removeOrphanedReceiverError = false;
		BGMergeSettingsEntity.IUpdateMatchingReceiver updateMatchingReceiver = null;
		bool updateMatchingReceiverError = false;
		BGMergeSettingsEntity.IUpdateMatchingFieldReceiver updateMatchingFieldReceiver = null;
		bool updateMatchingFieldReceiverError = false;
		object obj = settings.NewController(logger);
		if (obj != null)
		{
			result.Controller = obj;
			string log = "";
			CheckInterface(obj, ref mergeReceiver, ref log);
			CheckInterface(obj, ref addMissingReceiver, ref log);
			CheckInterface(obj, ref removeOrphanedReceiver, ref log);
			CheckInterface(obj, ref updateMatchingReceiver, ref log);
			CheckInterface(obj, ref updateMatchingFieldReceiver, ref log);
			if (string.IsNullOrEmpty(log))
			{
				AppendLine("Controller was set up $, however controller does not implement any receiver interfaces.", settings.ControllerType);
			}
			else
			{
				AppendLine("Controller was set up $. The following receivers was assigned: $.", settings.ControllerType, log);
			}
		}
		if (mergeReceiver != null)
		{
			bool flag = false;
			try
			{
				flag = mergeReceiver.OnBeforeMerge(From, To);
			}
			catch (Exception ex)
			{
				AppendWarning("Error from mergeReceiver.OnBeforeMerge:" + ex.Message);
			}
			if (flag)
			{
				AppendLine("Attention!!! Merge was cancelled by a controller.");
				return result;
			}
		}
		From.ForEachMeta((BGMetaEntity metaFrom) =>
		{
			SubSection(() =>
			{
				BGId id = metaFrom.Id;
				if (!settings.IsMetaIncluded(id))
				{
					AppendLine("Meta is not included in settings. Skipping..");
				}
				else
				{
					BGMetaEntity meta = To.GetMeta(id);
					if (meta == null)
					{
						AppendLine("Meta is not found in destination Repo. Skipping..");
					}
					else
					{
						bool flag2 = settings.IsAddingMissing(id);
						bool flag3 = settings.IsRemovingOrphaned(id);
						bool flag4 = settings.IsUpdatingMatching(id);
						meta.ForEachEntity((BGEntity myObject) =>
						{
							Id2EntityMine[myObject.Id] = myObject;
						});
						metaFrom.ForEachEntity((BGEntity otherObject) =>
						{
							Id2EntityOther[otherObject.Id] = otherObject;
						});
						foreach (KeyValuePair<BGId, BGEntity> item in Id2EntityMine)
						{
							if (Id2EntityOther.TryGetValue(item.Key, out var value))
							{
								Other2Mine[value] = item.Value;
							}
							else
							{
								Orphaned.Add(item.Value);
							}
						}
						foreach (KeyValuePair<BGId, BGEntity> item2 in Id2EntityOther)
						{
							if (!Id2EntityMine.ContainsKey(item2.Key))
							{
								Missing.Add(item2.Value);
							}
						}
						if (flag2)
						{
							int num = 0;
							foreach (BGEntity item3 in Missing)
							{
								BGEntity entity = item3;
								try
								{
									if (addMissingReceiver != null && addMissingReceiver.OnBeforeAdd(entity))
									{
										continue;
									}
								}
								catch (Exception ex2)
								{
									if (!addMissingReceiverError)
									{
										addMissingReceiverError = true;
										AppendWarning("addMissingReceiver controller raised an error: $ ", ex2.Message);
									}
								}
								num++;
								meta.NewEntity(entity.Id);
								meta.ForEachField((BGField fieldTo) =>
								{
									if (settings.AddMissingFieldFilter == null || settings.AddMissingFieldFilter(fieldTo))
									{
										BGField field = metaFrom.GetField(fieldTo.Id, errorIfNotFound: false);
										if (field != null)
										{
											fieldTo.CopyValue(field, entity.Id, entity.Index, entity.Id);
										}
									}
								});
							}
							result.AddedCount += num;
							result.AddedSkippedCount += Missing.Count - num;
							AppendLine("$ missing entities added. $ skipped. ", num, Missing.Count - num);
						}
						else
						{
							AppendLine("$ missing entities found. No action was taken", Missing.Count);
						}
						if (flag3)
						{
							int num2 = 0;
							foreach (BGEntity item4 in Orphaned)
							{
								try
								{
									if (removeOrphanedReceiver != null && removeOrphanedReceiver.OnBeforeDelete(item4))
									{
										continue;
									}
								}
								catch (Exception ex3)
								{
									if (!removeOrphanedReceiverError)
									{
										removeOrphanedReceiverError = true;
										AppendWarning("removeOrphanedReceiver controller raised an error: $ ", ex3.Message);
									}
								}
								num2++;
								item4.Delete();
							}
							result.RemovedCount += num2;
							result.RemovedSkippedCount += Orphaned.Count - num2;
							AppendLine("$ orphaned entities removed. $ skipped.", num2, Orphaned.Count - num2);
						}
						else
						{
							AppendLine("$ orphaned entities found. No action was taken", Orphaned.Count);
						}
						if (flag4)
						{
							int num3 = 0;
							int updatedFieldsCount = 0;
							int fieldUpdateCancelled = 0;
							foreach (KeyValuePair<BGEntity, BGEntity> item5 in Other2Mine)
							{
								BGEntity fromEntity = item5.Key;
								BGEntity toEntity = item5.Value;
								try
								{
									if (updateMatchingReceiver != null && updateMatchingReceiver.OnBeforeUpdate(fromEntity, toEntity))
									{
										continue;
									}
								}
								catch (Exception ex4)
								{
									if (!updateMatchingReceiverError)
									{
										updateMatchingReceiverError = true;
										AppendWarning("updateMatchingReceiverError controller raised an error: $ ", ex4.Message);
									}
								}
								num3++;
								meta.ForEachField((BGField fieldTo) =>
								{
									BGField field = metaFrom.GetField(fieldTo.Id, errorIfNotFound: false);
									if (field != null && settings.IsFieldIncluded(field))
									{
										int num4;
										try
										{
											if (updateMatchingFieldReceiver != null && updateMatchingFieldReceiver.OnBeforeFieldUpdate(field, fieldTo, fromEntity, toEntity))
											{
												num4 = fieldUpdateCancelled;
												fieldUpdateCancelled = num4 + 1;
												return;
											}
										}
										catch (Exception ex5)
										{
											if (!updateMatchingFieldReceiverError)
											{
												updateMatchingFieldReceiverError = true;
												AppendWarning("updateMatchingFieldReceiverError controller raised an error: $ ", ex5.Message);
											}
										}
										num4 = updatedFieldsCount;
										updatedFieldsCount = num4 + 1;
										fieldTo.CopyValue(field, fromEntity.Id, fromEntity.Index, fromEntity.Id);
									}
								});
							}
							result.UpdatedCount += num3;
							result.UpdatedSkippedCount += Other2Mine.Count - num3;
							result.UpdatedFieldsCount += updatedFieldsCount;
							result.UpdatedFieldsSkippedCount += fieldUpdateCancelled;
							AppendLine("$ matching entities updated. $ skipped. $ field update operations was cancelled.", num3, Other2Mine.Count - num3, fieldUpdateCancelled);
						}
						else
						{
							AppendLine("$ matching entities found. No action was taken", Other2Mine.Count);
						}
						ClearTempLists();
					}
				}
			}, "Processing $ meta.", metaFrom.Name);
		});
		if (mergeReceiver != null)
		{
			BGUtil.Catch(() =>
			{
				mergeReceiver.OnAfterMerge(From, To);
			}, (Exception exception) =>
			{
				AppendWarning("Error from mergeReceiver.OnAfterMerge:" + exception.Message);
			});
		}
		AppendLine("$ metas processed.", From.CountMeta);
		return result;
	}

	private void CheckInterface<T>(object controller, ref T receiverInterface, ref string log) where T : class
	{
		if (controller is T val)
		{
			receiverInterface = val;
			log = log + typeof(T).Name + " ";
		}
	}

	private void ClearTempLists()
	{
		Missing.Clear();
		Orphaned.Clear();
		Other2Mine.Clear();
		Id2EntityMine.Clear();
		Id2EntityOther.Clear();
	}
}
