using System;

namespace BansheeGz.BGDatabase;

public class BGEventsHolder
{
	private class BGListenersList
	{
		public readonly BGId Id;

		public readonly Delegate[] delegates;

		public BGListenersList(BGId Id, Delegate[] delegates)
		{
			this.Id = Id;
			this.delegates = delegates;
		}
	}

	private readonly BGIdDictionary<Delegate[]> fieldListeners = new BGIdDictionary<Delegate[]>();

	private readonly BGIdDictionary<Delegate[]> fieldBeforeListeners = new BGIdDictionary<Delegate[]>();

	private readonly BGIdDictionary<BGIdDictionary<BGEventsDelegatesHolder<BGEventArgsEntity>>> entityId2DeleteEntityListener = new BGIdDictionary<BGIdDictionary<BGEventsDelegatesHolder<BGEventArgsEntity>>>();

	private readonly BGIdDictionary<BGIdDictionary<BGEventsDelegatesHolder<BGEventArgsEntityUpdated>>> entityId2UpdateEntityListener = new BGIdDictionary<BGIdDictionary<BGEventsDelegatesHolder<BGEventArgsEntityUpdated>>>();

	private readonly BGIdDictionary<Delegate[]> metaId2AddAnyEntityListener = new BGIdDictionary<Delegate[]>();

	private readonly BGIdDictionary<Delegate[]> metaId2BeforeAddAnyEntityListener = new BGIdDictionary<Delegate[]>();

	private readonly BGIdDictionary<Delegate[]> metaId2DeleteAnyEntityListener = new BGIdDictionary<Delegate[]>();

	private readonly BGIdDictionary<Delegate[]> metaId2UpdateAnyEntityListener = new BGIdDictionary<Delegate[]>();

	private readonly BGIdDictionary<Delegate[]> metaId2BeforeUpdateAnyEntityListener = new BGIdDictionary<Delegate[]>();

	private readonly BGIdDictionary<Delegate[]> metaId2EntitiesOrderListener = new BGIdDictionary<Delegate[]>();

	internal void AddOnFieldValueChangedListeners(BGId fieldId, Delegate[] handlers)
	{
		fieldListeners.Add(fieldId, handlers);
	}

	internal void AddOnBeforeFieldValueChangedListeners(BGId fieldId, Delegate[] handlers)
	{
		fieldBeforeListeners.Add(fieldId, handlers);
	}

	public void AddOnAnyEntityAddedListeners(BGId metaId, Delegate[] handlers)
	{
		metaId2AddAnyEntityListener.Add(metaId, handlers);
	}

	public void AddOnAnyEntityBeforeAddedListeners(BGId metaId, Delegate[] handlers)
	{
		metaId2BeforeAddAnyEntityListener.Add(metaId, handlers);
	}

	public void AddOnAnyEntityUpdatedListeners(BGId metaId, Delegate[] handlers)
	{
		metaId2UpdateAnyEntityListener.Add(metaId, handlers);
	}

	public void AddOnAnyEntityBeforeUpdatedListeners(BGId metaId, Delegate[] handlers)
	{
		metaId2BeforeUpdateAnyEntityListener.Add(metaId, handlers);
	}

	public void AddOnAnyEntityDeletedListeners(BGId metaId, Delegate[] handlers)
	{
		metaId2DeleteAnyEntityListener.Add(metaId, handlers);
	}

	public void AddOnEntitiesOrderChangedListeners(BGId metaId, Delegate[] handlers)
	{
		metaId2EntitiesOrderListener.Add(metaId, handlers);
	}

	public void AddOnEntityUpdatedListeners(BGId entityId, BGIdDictionary<BGEventsDelegatesHolder<BGEventArgsEntityUpdated>> handlers)
	{
		entityId2UpdateEntityListener.Add(entityId, handlers);
	}

	public void AddOnEntityDeletedListeners(BGId entityId, BGIdDictionary<BGEventsDelegatesHolder<BGEventArgsEntity>> handlers)
	{
		entityId2DeleteEntityListener.Add(entityId, handlers);
	}

	public void TransferEventsFrom(BGMetaEntity meta)
	{
		meta.TransferEventsTo(this);
	}

	public void TransferEventsFrom(BGField field)
	{
		field.TransferEventsTo(this);
	}

	public void TransferEventsTo(BGMetaEntity meta)
	{
		meta.TransferEventsFrom(this);
	}

	public void TransferEventsTo(BGField field)
	{
		field.TransferEventsFrom(this);
	}

	public Delegate[] GetOnFieldValueChangedListeners(BGId fieldId)
	{
		if (fieldListeners.TryGetValue(fieldId, out var value))
		{
			return value;
		}
		return null;
	}

	public Delegate[] GetOnFieldBeforeValueChangedListeners(BGId fieldId)
	{
		if (fieldBeforeListeners.TryGetValue(fieldId, out var value))
		{
			return value;
		}
		return null;
	}

	public Delegate[] GetOnAnyEntityAddedListeners(BGId metaId)
	{
		if (!metaId2AddAnyEntityListener.TryGetValue(metaId, out var value))
		{
			return null;
		}
		return value;
	}

	public Delegate[] GetOnAnyEntityBeforeAddedListeners(BGId metaId)
	{
		if (!metaId2BeforeAddAnyEntityListener.TryGetValue(metaId, out var value))
		{
			return null;
		}
		return value;
	}

	public Delegate[] GetOnAnyEntityUpdatedListeners(BGId metaId)
	{
		if (!metaId2UpdateAnyEntityListener.TryGetValue(metaId, out var value))
		{
			return null;
		}
		return value;
	}

	public Delegate[] GetOnAnyEntityBeforeUpdatedListeners(BGId metaId)
	{
		if (!metaId2BeforeUpdateAnyEntityListener.TryGetValue(metaId, out var value))
		{
			return null;
		}
		return value;
	}

	public Delegate[] GetOnAnyEntityDeletedListeners(BGId metaId)
	{
		if (!metaId2DeleteAnyEntityListener.TryGetValue(metaId, out var value))
		{
			return null;
		}
		return value;
	}

	public Delegate[] GetOnEntitiesOrderChangedListeners(BGId metaId)
	{
		if (!metaId2EntitiesOrderListener.TryGetValue(metaId, out var value))
		{
			return null;
		}
		return value;
	}

	public BGIdDictionary<BGEventsDelegatesHolder<BGEventArgsEntityUpdated>> GetOnEntityUpdatedListeners(BGId metaId)
	{
		if (entityId2UpdateEntityListener.Count == 0)
		{
			return null;
		}
		if (!entityId2UpdateEntityListener.TryGetValue(metaId, out var value))
		{
			return null;
		}
		return value;
	}

	public BGIdDictionary<BGEventsDelegatesHolder<BGEventArgsEntity>> GetOnEntityDeletedListeners(BGId metaId)
	{
		if (entityId2DeleteEntityListener.Count == 0)
		{
			return null;
		}
		if (!entityId2DeleteEntityListener.TryGetValue(metaId, out var value))
		{
			return null;
		}
		return value;
	}
}
