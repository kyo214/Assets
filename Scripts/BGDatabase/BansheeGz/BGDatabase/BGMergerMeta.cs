using System;
using System.Collections.Generic;

namespace BansheeGz.BGDatabase;

public class BGMergerMeta : BGMergerA
{
	private readonly BGMergeSettingsMeta settings;

	public BGMergerMeta(BGLogger logger, BGRepo from, BGRepo to, BGMergeSettingsMeta settings)
		: base(logger, from, to)
	{
		this.settings = settings;
	}

	public void Merge()
	{
		Section("Merging Metas", () =>
		{
			To.Events.Batch(() =>
			{
				switch (settings?.Mode ?? BGMergeModeEnum.Transfer)
				{
				case BGMergeModeEnum.Transfer:
					Transfer();
					break;
				case BGMergeModeEnum.Merge:
					Combine();
					break;
				default:
					throw new ArgumentOutOfRangeException();
				}
			});
		});
	}

	private void Transfer()
	{
		To.Clear();
		From.ForEachMeta((BGMetaEntity meta) =>
		{
			meta.SwitchTo(To);
		});
		From.Clear();
		AppendLine("$ metas was transfered.", To.CountMeta);
		To.Addons.Clear();
		To.Addons.AddFrom(From.Addons);
		From.Addons.Clear();
	}

	private void Combine()
	{
		if (settings.AddMissing || settings.UpdateMatching)
		{
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
						BGMetaEntity metaTo = To.GetMeta(id);
						if (metaTo == null)
						{
							if (settings.AddMissing)
							{
								metaTo = metaFrom.CloneTo(To, null, (BGField field) => false, copyValues: false);
								AppendLine("Meta is not found in destination Repo and was added.");
							}
							else
							{
								AppendLine("Meta is not found in destination Repo and was skipped due to settings");
							}
						}
						else if (settings.UpdateMatching)
						{
							metaTo.Name = metaFrom.Name;
							metaTo.Singleton = metaFrom.Singleton;
							metaTo.EmptyName = metaFrom.EmptyName;
							metaTo.UniqueName = metaFrom.UniqueName;
							metaTo.Comment = metaFrom.Comment;
							metaTo.ControllerType = metaFrom.ControllerType;
							metaTo.UserDefinedReadonly = metaFrom.UserDefinedReadonly;
							AppendLine("Meta was found and updated.");
						}
						else
						{
							AppendLine("Meta was found but no action was taken due to settings.");
						}
						if (metaTo != null)
						{
							int added = 0;
							int updated = 0;
							int removed = 0;
							int skipped = 0;
							metaFrom.ForEachField((BGField fieldFrom) =>
							{
								if (!settings.IsFieldIncluded(fieldFrom))
								{
									skipped++;
									AppendLine("field $ is skipped due to settings.", fieldFrom.Name);
								}
								else
								{
									BGField field = metaTo.GetField(fieldFrom.Id, errorIfNotFound: false);
									if (field == null)
									{
										if (settings.AddMissing)
										{
											added++;
											AppendLine("field $ is added", fieldFrom.Name);
											fieldFrom.CloneTo(metaTo, copyValues: false);
										}
										else
										{
											skipped++;
											AppendLine("field $ is not found in destination Repo and was skipped due to settings", fieldFrom.Name);
										}
									}
									else if (settings.UpdateMatching)
									{
										field.Name = fieldFrom.Name;
										field.Required = fieldFrom.Required;
										field.UserDefinedReadonly = fieldFrom.UserDefinedReadonly;
										field.DefaultValue = fieldFrom.DefaultValue;
										field.Comment = fieldFrom.Comment;
										field.ControllerType = fieldFrom.ControllerType;
										updated++;
										AppendLine("field $ was updated", fieldFrom.Name);
									}
									else
									{
										skipped++;
										AppendLine("field $ is skipped due to settings", fieldFrom.Name);
									}
								}
							});
							if (settings.RemoveOrphaned)
							{
								metaTo.ForEachField((BGField field) =>
								{
									if (!metaFrom.HasField(field.Id))
									{
										removed++;
										AppendLine("field $ is removed.", field.Name);
										field.Delete();
									}
								});
							}
							AppendLine("$ fields was added. $ fields was updated. $ fields was removed.. $ fields was skipped.", added, updated, removed, skipped);
						}
					}
				}, "Processing meta $", metaFrom.Name);
			});
			From.Addons.ForEachAddon((BGAddon addonFrom) =>
			{
				if (To.Addons.Has(addonFrom.GetType()))
				{
					if (settings.UpdateMatching)
					{
						BGAddon bGAddon = To.Addons.Get(addonFrom.GetType());
						bGAddon.ConfigFromString(addonFrom.ConfigToString());
					}
				}
				else if (settings.AddMissing)
				{
					addonFrom.CloneAndAddTo(To);
				}
			});
		}
		if (!settings.RemoveOrphaned)
		{
			return;
		}
		To.ForEachMeta((BGMetaEntity metaTo) =>
		{
			if (!From.HasMeta(metaTo.Id))
			{
				metaTo.Delete();
			}
			else
			{
				List<BGField> fields = new List<BGField>();
				BGMetaEntity metaFrom = From.GetMeta(metaTo.Id);
				metaTo.ForEachField((BGField field) =>
				{
					fields.Add(field);
				}, (BGField field) => !metaFrom.HasField(field.Id));
				if (fields.Count > 0)
				{
					foreach (BGField item in fields)
					{
						item.Delete();
					}
				}
			}
		});
		List<Type> toRemove = new List<Type>();
		To.Addons.ForEachAddon((BGAddon addonTo) =>
		{
			Type type = addonTo.GetType();
			if (!From.Addons.Has(type))
			{
				toRemove.Add(type);
			}
		});
		if (toRemove.Count <= 0)
		{
			return;
		}
		foreach (Type item2 in toRemove)
		{
			To.Addons.Remove(item2);
		}
	}
}
