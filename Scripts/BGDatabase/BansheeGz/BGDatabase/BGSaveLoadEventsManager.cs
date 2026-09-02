using System;
using System.Collections.Generic;
using UnityEngine;

namespace BansheeGz.BGDatabase;

internal class BGSaveLoadEventsManager
{
	private class EntityMatchingIteration<T> where T : EventArgs
	{
		internal BGMetaEntity meta;

		internal BGMetaEntity oldMeta;

		internal BGEntity entity;

		internal BGEntity oldEntity;

		internal EventHandler<T> handler;

		internal virtual void Clear()
		{
			meta = null;
			oldMeta = null;
			entity = null;
			oldEntity = null;
		}
	}

	private class CellMatchingIteration : EntityMatchingIteration<BGSaveLoadEventArgsCellChanged>
	{
		internal BGField field;

		internal BGField oldField;

		internal override void Clear()
		{
			base.Clear();
			field = null;
			oldField = null;
		}
	}

	private BGAddonSaveLoad addon;

	private readonly Dictionary<BGEntityPointer, EventHandler<BGSaveLoadEventArgsEntityChanged>> metaPointer2EntityHandlers = new Dictionary<BGEntityPointer, EventHandler<BGSaveLoadEventArgsEntityChanged>>();

	private readonly Dictionary<BGCellPointer, EventHandler<BGSaveLoadEventArgsCellChanged>> metaPointer2CellHandlers = new Dictionary<BGCellPointer, EventHandler<BGSaveLoadEventArgsCellChanged>>();

	private BGRepo oldRepo;

	internal BGAddonSaveLoad Addon
	{
		set
		{
			addon = value;
		}
	}

	internal BGSaveLoadEventsManager(BGAddonSaveLoad addon)
	{
		this.addon = addon;
	}

	internal void Add(BGEntityPointer pointer, EventHandler<BGSaveLoadEventArgsEntityChanged> handler)
	{
		Add(metaPointer2EntityHandlers, pointer, handler);
	}

	internal void Remove(BGEntityPointer pointer, EventHandler<BGSaveLoadEventArgsEntityChanged> handler)
	{
		Remove(metaPointer2EntityHandlers, pointer, handler);
	}

	internal void Add(BGCellPointer pointer, EventHandler<BGSaveLoadEventArgsCellChanged> handler)
	{
		Add(metaPointer2CellHandlers, pointer, handler);
	}

	internal void Remove(BGCellPointer pointer, EventHandler<BGSaveLoadEventArgsCellChanged> handler)
	{
		Remove(metaPointer2CellHandlers, pointer, handler);
	}

	internal void BeforeLoad()
	{
		if (metaPointer2EntityHandlers.Count == 0)
		{
			return;
		}
		oldRepo = new BGRepo();
		ForEachMatchingEntity((BGEntityPointer pointer, EntityMatchingIteration<BGSaveLoadEventArgsEntityChanged> iteration) =>
		{
			iteration.oldMeta = iteration.meta.CloneTo(oldRepo, null, null, copyValues: false);
		}, (BGEntityPointer pointer, EntityMatchingIteration<BGSaveLoadEventArgsEntityChanged> iteration) =>
		{
			iteration.oldEntity = iteration.oldMeta.NewEntity(pointer.EntityId);
		}, (EntityMatchingIteration<BGSaveLoadEventArgsEntityChanged> iteration) =>
		{
			iteration.oldMeta.ForEachField((BGField oldField) =>
			{
				BGField field = iteration.meta.GetField(oldField.Id, errorIfNotFound: false);
				if (field != null)
				{
					BGEntity entity = iteration.entity;
					oldField.CopyValue(field, entity.Id, entity.Index, entity.Id);
				}
			});
		});
		ForEachMatchingCell((BGCellPointer pointer, CellMatchingIteration iteration) =>
		{
			iteration.oldMeta = iteration.meta.CloneTo(oldRepo, null, (BGField field) => field.Id == iteration.field.Id, copyValues: false);
		}, (BGCellPointer pointer, CellMatchingIteration iteration) =>
		{
			iteration.oldEntity = iteration.oldMeta.NewEntity(pointer.EntityId);
		}, (BGCellPointer pointer, CellMatchingIteration iteration) =>
		{
			iteration.oldField = iteration.field.CloneTo(iteration.oldMeta, copyValues: false);
		}, (CellMatchingIteration iteration) =>
		{
			BGEntity entity = iteration.entity;
			iteration.oldField.CopyValue(iteration.field, entity.Id, entity.Index, entity.Id);
		});
	}

	internal void AfterLoad()
	{
		List<BGSaveLoadEventArgsEntityChanged.FieldChangedData> fieldsData = new List<BGSaveLoadEventArgsEntityChanged.FieldChangedData>();
		ForEachMatchingEntity(null, null, (EntityMatchingIteration<BGSaveLoadEventArgsEntityChanged> iteration) =>
		{
			fieldsData.Clear();
			iteration.oldMeta.ForEachField((BGField oldField) =>
			{
				BGField field = iteration.meta.GetField(oldField.Id, errorIfNotFound: false);
				if (field != null)
				{
					BGEntity entity = iteration.entity;
					BGEntity oldEntity = iteration.oldEntity;
					if (!field.AreStoredValuesEqual(oldField, entity.Index, oldEntity.Index))
					{
						fieldsData.Add(new BGSaveLoadEventArgsEntityChanged.FieldChangedData(field, oldField.GetValue(oldEntity.Index), field.GetValue(entity.Index)));
					}
				}
			});
			if (fieldsData.Count == 0)
			{
				return;
			}
			using BGSaveLoadEventArgsEntityChanged e = BGSaveLoadEventArgsEntityChanged.Get(iteration.meta, iteration.entity, fieldsData);
			try
			{
				iteration.handler(addon, e);
			}
			catch (Exception exception)
			{
				Debug.LogException(exception);
			}
		});
		ForEachMatchingCell(null, null, null, (CellMatchingIteration iteration) =>
		{
			if (iteration.field.AreStoredValuesEqual(iteration.oldField, iteration.entity.Index, iteration.oldEntity.Index))
			{
				return;
			}
			using BGSaveLoadEventArgsCellChanged e = BGSaveLoadEventArgsCellChanged.Get(iteration.meta, iteration.field, iteration.entity, iteration.oldField.GetValue(iteration.oldEntity.Index), iteration.field.GetValue(iteration.entity.Index));
			try
			{
				iteration.handler(addon, e);
			}
			catch (Exception exception)
			{
				Debug.LogException(exception);
			}
		});
		oldRepo = null;
	}

	private void ForEachMatchingEntity(Action<BGEntityPointer, EntityMatchingIteration<BGSaveLoadEventArgsEntityChanged>> metaProvider, Action<BGEntityPointer, EntityMatchingIteration<BGSaveLoadEventArgsEntityChanged>> entityProvider, Action<EntityMatchingIteration<BGSaveLoadEventArgsEntityChanged>> action)
	{
		if (metaPointer2EntityHandlers.Count == 0)
		{
			return;
		}
		EntityMatchingIteration<BGSaveLoadEventArgsEntityChanged> entityMatchingIteration = new EntityMatchingIteration<BGSaveLoadEventArgsEntityChanged>();
		BGRepo repo = addon.Repo;
		foreach (KeyValuePair<BGEntityPointer, EventHandler<BGSaveLoadEventArgsEntityChanged>> metaPointer2EntityHandler in metaPointer2EntityHandlers)
		{
			EventHandler<BGSaveLoadEventArgsEntityChanged> value = metaPointer2EntityHandler.Value;
			if (value == null)
			{
				continue;
			}
			entityMatchingIteration.Clear();
			entityMatchingIteration.handler = value;
			BGEntityPointer key = metaPointer2EntityHandler.Key;
			BGId entityId = key.EntityId;
			entityMatchingIteration.meta = key.GetMeta(repo);
			if (entityMatchingIteration.meta == null)
			{
				continue;
			}
			entityMatchingIteration.oldMeta = key.GetMeta(oldRepo);
			if (entityMatchingIteration.oldMeta == null)
			{
				if (metaProvider == null)
				{
					continue;
				}
				metaProvider(key, entityMatchingIteration);
			}
			if (entityMatchingIteration.oldMeta == null)
			{
				continue;
			}
			entityMatchingIteration.entity = entityMatchingIteration.meta.GetEntity(entityId);
			if (entityMatchingIteration.entity == null)
			{
				continue;
			}
			entityMatchingIteration.oldEntity = entityMatchingIteration.oldMeta.GetEntity(entityId);
			if (entityMatchingIteration.oldEntity == null)
			{
				if (entityProvider == null)
				{
					continue;
				}
				entityProvider(key, entityMatchingIteration);
			}
			if (entityMatchingIteration.oldEntity != null)
			{
				action(entityMatchingIteration);
			}
		}
	}

	private void ForEachMatchingCell(Action<BGCellPointer, CellMatchingIteration> metaProvider, Action<BGCellPointer, CellMatchingIteration> entityProvider, Action<BGCellPointer, CellMatchingIteration> fieldProvider, Action<CellMatchingIteration> action)
	{
		if (metaPointer2CellHandlers.Count == 0)
		{
			return;
		}
		CellMatchingIteration cellMatchingIteration = new CellMatchingIteration();
		BGRepo repo = addon.Repo;
		foreach (KeyValuePair<BGCellPointer, EventHandler<BGSaveLoadEventArgsCellChanged>> metaPointer2CellHandler in metaPointer2CellHandlers)
		{
			EventHandler<BGSaveLoadEventArgsCellChanged> value = metaPointer2CellHandler.Value;
			if (value == null)
			{
				continue;
			}
			cellMatchingIteration.Clear();
			cellMatchingIteration.handler = value;
			BGCellPointer key = metaPointer2CellHandler.Key;
			BGId entityId = key.EntityId;
			BGId fieldId = key.FieldId;
			cellMatchingIteration.meta = key.GetMeta(repo);
			if (cellMatchingIteration.meta == null)
			{
				continue;
			}
			cellMatchingIteration.oldMeta = key.GetMeta(oldRepo);
			if (cellMatchingIteration.oldMeta == null)
			{
				if (metaProvider == null)
				{
					continue;
				}
				metaProvider(key, cellMatchingIteration);
			}
			if (cellMatchingIteration.oldMeta == null)
			{
				continue;
			}
			cellMatchingIteration.field = cellMatchingIteration.meta.GetField(fieldId, errorIfNotFound: false);
			if (cellMatchingIteration.field == null)
			{
				continue;
			}
			cellMatchingIteration.oldField = cellMatchingIteration.oldMeta.GetField(fieldId, errorIfNotFound: false);
			if (cellMatchingIteration.oldField == null)
			{
				if (fieldProvider == null)
				{
					continue;
				}
				fieldProvider(key, cellMatchingIteration);
			}
			if (cellMatchingIteration.oldField == null)
			{
				continue;
			}
			cellMatchingIteration.entity = cellMatchingIteration.meta.GetEntity(entityId);
			if (cellMatchingIteration.entity == null)
			{
				continue;
			}
			cellMatchingIteration.oldEntity = cellMatchingIteration.oldMeta.GetEntity(entityId);
			if (cellMatchingIteration.oldEntity == null)
			{
				if (entityProvider == null)
				{
					continue;
				}
				entityProvider(key, cellMatchingIteration);
			}
			if (cellMatchingIteration.oldEntity != null)
			{
				action(cellMatchingIteration);
			}
		}
	}

	private static void Add<T, TK>(Dictionary<TK, EventHandler<T>> dict, TK pointer, EventHandler<T> handler) where T : BGEventArgsA where TK : BGMetaPointer
	{
		if (!dict.TryGetValue(pointer, out var value))
		{
			value = handler;
			dict.Add(pointer, value);
		}
		else
		{
			value = (EventHandler<T>)Delegate.Combine(value, handler);
			dict[pointer] = value;
		}
	}

	private static void Remove<T, TK>(Dictionary<TK, EventHandler<T>> dict, TK pointer, EventHandler<T> handler) where T : BGEventArgsA where TK : BGMetaPointer
	{
		if (dict.TryGetValue(pointer, out var value))
		{
			value = (EventHandler<T>)Delegate.Remove(value, handler);
			if (value == null)
			{
				dict.Remove(pointer);
			}
			else
			{
				dict[pointer] = value;
			}
		}
	}
}
