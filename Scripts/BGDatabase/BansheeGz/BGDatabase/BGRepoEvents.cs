using System;
using UnityEngine;

namespace BansheeGz.BGDatabase;

public class BGRepoEvents
{
	private BGEventArgsBatch batchEvent;

	private bool on;

	private readonly BGRepo repo;

	public bool On
	{
		get
		{
			return on;
		}
		set
		{
			on = value;
		}
	}

	public bool IsInBatch => batchEvent != null;

	public event EventHandler<BGEventArgsAnyChange> OnAnyChange;

	public event EventHandler<BGEventArgsBatch> OnBatchUpdate;

	public event EventHandler<BGEventArgsMeta> OnRepoStructureChange;

	public BGRepoEvents(BGRepo repo)
	{
		this.repo = repo;
	}

	public void FireAnyChange()
	{
		if (!on || batchEvent != null || OnAnyChange == null)
		{
			return;
		}
		using BGEventArgsAnyChange e = BGEventArgsAnyChange.GetInstance(repo);
		try
		{
			OnAnyChange(this, e);
		}
		catch (Exception exception)
		{
			Debug.LogException(exception);
		}
	}

	public void FireAddonChange()
	{
		FireAnyChange();
	}

	[Obsolete("Use BGField.ValueChanged event instead")]
	public void AddFieldListener(BGId fieldId, EventHandler<BGEventArgsField> handler)
	{
		BGField field = repo.GetField(fieldId);
		if (field != null)
		{
			field.ValueChanged += handler;
		}
	}

	[Obsolete("Use BGField.ValueChanged event instead")]
	public void RemoveFieldListener(BGId fieldId, EventHandler<BGEventArgsField> handler)
	{
		BGField field = repo.GetField(fieldId);
		if (field != null)
		{
			field.ValueChanged -= handler;
		}
	}

	[Obsolete("Use BGField.FireValueChanged event instead")]
	public void FieldWasChanged(BGId metaId, BGId fieldId, BGId entityId)
	{
		if (on)
		{
			if (batchEvent != null)
			{
				batchEvent.AddMetaWithUpdatedEntities(metaId);
			}
			else
			{
				FireFieldChanged(metaId, fieldId, entityId);
			}
		}
	}

	[Obsolete("Use BGField.FireValueChanged event instead")]
	public void FireFieldChanged(BGId metaId, BGId fieldId, BGId entityId)
	{
		BGMetaEntity meta = null;
		BGEntity entity = null;
		if (EnsureEntity(metaId, entityId, ref meta, ref entity))
		{
			meta.GetField(fieldId, errorIfNotFound: false)?.FireValueChanged(entity);
		}
		FireAnyChange();
	}

	private bool EnsureEntity(BGId metaId, BGId entityId, ref BGMetaEntity meta, ref BGEntity entity)
	{
		if (entity != null)
		{
			return true;
		}
		meta = repo[metaId];
		if (meta == null)
		{
			return false;
		}
		entity = meta[entityId];
		if (entity == null)
		{
			return false;
		}
		return true;
	}

	internal bool ConsumeOnChange(BGId metaId)
	{
		if (!on)
		{
			return true;
		}
		if (batchEvent != null)
		{
			batchEvent.AddMetaWithUpdatedEntities(metaId);
			return true;
		}
		return false;
	}

	[Obsolete("Use BGMetaEntity.AddEntityUpdatedListener method instead")]
	public void AddEntityUpdatedListener(BGId entityId, EventHandler<BGEventArgsEntityUpdated> handler)
	{
		repo.GetEntity(entityId)?.Meta.AddEntityUpdatedListener(entityId, handler);
	}

	[Obsolete("Use BGMetaEntity.AddEntityDeletedListener method instead")]
	public void AddEntityDeletedListener(BGId entityId, EventHandler<BGEventArgsEntity> handler)
	{
		repo.GetEntity(entityId)?.Meta.AddEntityDeletedListener(entityId, handler);
	}

	[Obsolete("Use BGMetaEntity.RemoveEntityUpdatedListener method instead")]
	public void RemoveEntityUpdatedListener(BGId entityId, EventHandler<BGEventArgsEntityUpdated> handler)
	{
		repo.GetEntity(entityId)?.Meta.RemoveEntityUpdatedListener(entityId, handler);
	}

	[Obsolete("Use BGMetaEntity.RemoveEntityDeletedListener method instead")]
	public void RemoveEntityDeletedListener(BGId entityId, EventHandler<BGEventArgsEntity> handler)
	{
		repo.GetEntity(entityId)?.Meta.RemoveEntityDeletedListener(entityId, handler);
	}

	[Obsolete("Use BGMetaEntity.EntitiesOrderChanged event instead")]
	public void AddEntitiesOrderListener(BGId metaId, EventHandler<BGEventArgsEntitiesOrder> handler)
	{
		BGMetaEntity meta = repo.GetMeta(metaId);
		if (meta != null)
		{
			meta.EntitiesOrderChanged += handler;
		}
	}

	[Obsolete("Use BGMetaEntity.AnyEntityUpdated event instead")]
	public void AddAnyEntityUpdatedListener(BGId metaId, EventHandler<BGEventArgsAnyEntityUpdated> handler)
	{
		BGMetaEntity meta = repo.GetMeta(metaId);
		if (meta != null)
		{
			meta.AnyEntityUpdated += handler;
		}
	}

	[Obsolete("Use BGMetaEntity.AnyEntityAdded event instead")]
	public void AddAnyEntityAddedListener(BGId metaId, EventHandler<BGEventArgsAnyEntity> handler)
	{
		BGMetaEntity meta = repo.GetMeta(metaId);
		if (meta != null)
		{
			meta.AnyEntityAdded += handler;
		}
	}

	[Obsolete("Use BGMetaEntity.AnyEntityDeleted event instead")]
	public void AddAnyEntityDeletedListener(BGId metaId, EventHandler<BGEventArgsAnyEntity> handler)
	{
		BGMetaEntity meta = repo.GetMeta(metaId);
		if (meta != null)
		{
			meta.AnyEntityDeleted += handler;
		}
	}

	[Obsolete("Use BGMetaEntity.EntitiesOrderChanged event instead")]
	public void RemoveEntitiesOrderListener(BGId metaId, EventHandler<BGEventArgsEntitiesOrder> handler)
	{
		BGMetaEntity meta = repo.GetMeta(metaId);
		if (meta != null)
		{
			meta.EntitiesOrderChanged -= handler;
		}
	}

	[Obsolete("Use BGMetaEntity.AnyEntityUpdated event instead")]
	public void RemoveAnyEntityUpdatedListener(BGId metaId, EventHandler<BGEventArgsAnyEntityUpdated> handler)
	{
		BGMetaEntity meta = repo.GetMeta(metaId);
		if (meta != null)
		{
			meta.AnyEntityUpdated -= handler;
		}
	}

	[Obsolete("Use BGMetaEntity.AnyEntityAdded event instead")]
	public void RemoveAnyEntityAddedListener(BGId metaId, EventHandler<BGEventArgsAnyEntity> handler)
	{
		BGMetaEntity meta = repo.GetMeta(metaId);
		if (meta != null)
		{
			meta.AnyEntityAdded -= handler;
		}
	}

	[Obsolete("Use BGMetaEntity.AnyEntityDeleted event instead")]
	public void RemoveAnyEntityDeletedListener(BGId metaId, EventHandler<BGEventArgsAnyEntity> handler)
	{
		BGMetaEntity meta = repo.GetMeta(metaId);
		if (meta != null)
		{
			meta.AnyEntityDeleted -= handler;
		}
	}

	[Obsolete("Use BGMetaEntity.FireEntityAdded method instead")]
	public void EntityWasAdded(BGEntity entity)
	{
		if (on)
		{
			if (batchEvent != null)
			{
				batchEvent.AddMetaWithAddedEntities(entity.MetaId);
			}
			else
			{
				FireEntityAdded(entity);
			}
		}
	}

	[Obsolete("Use BGMetaEntity.FireEntityDeleted method instead")]
	public void EntityWasDeleted(BGEntity entity)
	{
		if (on)
		{
			if (batchEvent != null)
			{
				batchEvent.AddMetaWithDeletedEntities(entity.MetaId);
			}
			else
			{
				FireEntityDeleted(entity);
			}
		}
	}

	[Obsolete("Use BGMetaEntity.FireEntityDeleted method instead")]
	public void FireEntityDeleted(BGEntity entity)
	{
		entity.Meta.FireEntityDeleted(entity);
	}

	[Obsolete("Use BGMetaEntity.FireEntityAdded method instead")]
	public void FireEntityAdded(BGEntity entity)
	{
		entity.Meta.FireEntityAdded(entity);
	}

	public void AddRepoStructureListener(EventHandler<BGEventArgsMeta> handler)
	{
		OnRepoStructureChange += handler;
	}

	public void RemoveRepoStructureListener(BGId metaId, EventHandler<BGEventArgsMeta> handler)
	{
		OnRepoStructureChange -= handler;
	}

	public void MetaWasChanged(BGMetaEntity meta)
	{
		FireRepoEvent(meta, BGEventArgsMeta.OperationEnum.Update);
	}

	public void MetaWasAdded(BGMetaEntity meta)
	{
		FireRepoEvent(meta, BGEventArgsMeta.OperationEnum.Add);
	}

	public void MetaWasDeleted(BGMetaEntity meta)
	{
		FireRepoEvent(meta, BGEventArgsMeta.OperationEnum.Delete);
	}

	public void FireRepoEvent(BGMetaEntity meta, BGEventArgsMeta.OperationEnum operation)
	{
		if (!on)
		{
			return;
		}
		if (batchEvent != null)
		{
			batchEvent.StructureChange = true;
			return;
		}
		if (OnRepoStructureChange != null)
		{
			using BGEventArgsMeta e = BGEventArgsMeta.GetInstance(operation, meta);
			Fire(OnRepoStructureChange, e);
		}
		FireAnyChange();
	}

	internal bool ConsumeOnEntityDelete(BGId metaId)
	{
		if (!on)
		{
			return true;
		}
		if (batchEvent != null)
		{
			batchEvent.AddMetaWithDeletedEntities(metaId);
			return true;
		}
		return false;
	}

	internal bool ConsumeOnEntityAdded(BGId metaId)
	{
		if (!on)
		{
			return true;
		}
		if (batchEvent != null)
		{
			batchEvent.AddMetaWithAddedEntities(metaId);
			return true;
		}
		return false;
	}

	internal bool ConsumeOnEntitiesOrderChanged(BGId metaId)
	{
		if (!on)
		{
			return true;
		}
		if (batchEvent != null)
		{
			batchEvent.AddMetaEntitiesOrderChanged(metaId);
			return true;
		}
		return false;
	}

	[Obsolete("Use BGMetaEntity.FireEntitiesOrderChanged event instead")]
	public void EntitiesOrderWasChanged(BGMetaEntity meta)
	{
		if (on)
		{
			if (batchEvent != null)
			{
				batchEvent.AddMetaEntitiesOrderChanged(meta.Id);
				return;
			}
			meta.FireEntitiesOrderChanged();
			FireAnyChange();
		}
	}

	public BGEventArgsBatch EnsureBatch()
	{
		if (batchEvent == null)
		{
			batchEvent = BGEventArgsBatch.GetInstance(repo);
		}
		return batchEvent;
	}

	public void ClearBatch()
	{
		batchEvent?.Dispose();
		batchEvent = null;
	}

	public void Batch(Action action)
	{
		bool flag = batchEvent == null && on;
		if (flag)
		{
			batchEvent = BGEventArgsBatch.GetInstance(repo);
		}
		try
		{
			action();
		}
		finally
		{
			if (flag)
			{
				FireBatchEvent();
			}
		}
	}

	public void FireBatchEvent()
	{
		if (batchEvent == null)
		{
			return;
		}
		try
		{
			if (OnBatchUpdate != null && !batchEvent.IsEmpty)
			{
				OnBatchUpdate(this, batchEvent);
			}
		}
		catch (Exception exception)
		{
			Debug.LogException(exception);
		}
		finally
		{
			ClearBatch();
			FireAnyChange();
		}
	}

	public void FireFullChange()
	{
		batchEvent = BGEventArgsBatch.GetInstance(repo);
		batchEvent.EverythingChanged = true;
		FireBatchEvent();
	}

	public void WithEventsDisabled(Action action)
	{
		bool flag = On;
		try
		{
			On = false;
			action();
		}
		finally
		{
			On = flag;
		}
	}

	public void Clear()
	{
		OnRepoStructureChange = null;
		OnBatchUpdate = null;
		OnAnyChange = null;
	}

	private void Fire<T>(BGEventsDelegatesHolder<T> holder, T e) where T : BGEventArgsA
	{
		EventHandler<T> handler = holder?.Handler;
		Fire(handler, e);
	}

	private void Fire<T>(EventHandler<T> handler, T e) where T : BGEventArgsA
	{
		if (handler == null)
		{
			return;
		}
		try
		{
			handler(this, e);
		}
		catch (Exception exception)
		{
			Debug.LogException(exception);
		}
	}

	private static void RemoveListener<T>(BGIdDictionary<BGEventsDelegatesHolder<T>> dictionary, BGId id, EventHandler<T> handler) where T : EventArgs
	{
		if (dictionary.TryGetValue(id, out var value))
		{
			BGEventsDelegatesHolder<T> bGEventsDelegatesHolder = value;
			bGEventsDelegatesHolder.Handler = (EventHandler<T>)Delegate.Remove(bGEventsDelegatesHolder.Handler, handler);
		}
	}

	private static void AddListener<T>(BGIdDictionary<BGEventsDelegatesHolder<T>> dictionary, BGId id, EventHandler<T> handler) where T : EventArgs
	{
		if (dictionary.TryGetValue(id, out var value))
		{
			BGEventsDelegatesHolder<T> bGEventsDelegatesHolder = value;
			bGEventsDelegatesHolder.Handler = (EventHandler<T>)Delegate.Combine(bGEventsDelegatesHolder.Handler, handler);
			return;
		}
		value = new BGEventsDelegatesHolder<T>();
		BGEventsDelegatesHolder<T> bGEventsDelegatesHolder2 = value;
		bGEventsDelegatesHolder2.Handler = (EventHandler<T>)Delegate.Combine(bGEventsDelegatesHolder2.Handler, handler);
		dictionary.Add(id, value);
	}

	public void ViewWasAdded(BGMetaView view)
	{
		FireViewEvent(view, BGEventArgsMeta.OperationEnum.Add);
	}

	public void ViewWasDeleted(BGMetaView view)
	{
		FireViewEvent(view, BGEventArgsMeta.OperationEnum.Delete);
	}

	public void ViewWasChanged(BGMetaView view)
	{
		FireViewEvent(view, BGEventArgsMeta.OperationEnum.Update);
	}

	public void FireViewEvent(BGMetaView view, BGEventArgsMeta.OperationEnum operation)
	{
		if (!on)
		{
			return;
		}
		if (batchEvent != null)
		{
			batchEvent.StructureChange = true;
			return;
		}
		if (OnRepoStructureChange != null)
		{
			using BGEventArgsMeta e = BGEventArgsMeta.GetInstance(operation, view);
			Fire(OnRepoStructureChange, e);
		}
		FireAnyChange();
	}
}
