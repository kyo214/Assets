using System;
using System.Collections.Generic;
using UnityEngine;

namespace BansheeGz.BGDatabase;

public class BGCalcFlowEvents
{
	private class EventsMetaData
	{
		public readonly BGId MetaId;

		public EventsMetaData(BGId metaId)
		{
			MetaId = metaId;
		}

		protected bool Equals(EventsMetaData other)
		{
			return MetaId.Equals(other.MetaId);
		}

		public override bool Equals(object obj)
		{
			if (obj == null)
			{
				return false;
			}
			if (this == obj)
			{
				return true;
			}
			if (obj.GetType() != GetType())
			{
				return false;
			}
			return Equals((EventsMetaData)obj);
		}

		public override int GetHashCode()
		{
			return MetaId.GetHashCode();
		}
	}

	private class EventsFieldData : EventsMetaData
	{
		public readonly BGId FieldId;

		public EventsFieldData(BGId metaId, BGId fieldId)
			: base(metaId)
		{
			FieldId = fieldId;
		}

		protected bool Equals(EventsFieldData other)
		{
			if (Equals((EventsMetaData)other))
			{
				return FieldId.Equals(other.FieldId);
			}
			return false;
		}

		public override bool Equals(object obj)
		{
			if (obj == null)
			{
				return false;
			}
			if (this == obj)
			{
				return true;
			}
			if (obj.GetType() != GetType())
			{
				return false;
			}
			return Equals((EventsFieldData)obj);
		}

		public override int GetHashCode()
		{
			return (base.GetHashCode() * 397) ^ FieldId.GetHashCode();
		}
	}

	private class EventsRowsData(BGId metaId, BGId fieldId) : EventsFieldData(metaId, fieldId)
	{
		public readonly List<BGId> EntityIds = new List<BGId>();
	}

	private const int MAXRowsCount = 5;

	private readonly Func<string> onChange;

	private readonly List<EventsFieldData> fields = new List<EventsFieldData>();

	private readonly List<EventsRowsData> editRow = new List<EventsRowsData>();

	private readonly List<EventsMetaData> createRow = new List<EventsMetaData>();

	private readonly List<EventsMetaData> deleteRow = new List<EventsMetaData>();

	private bool listenersAdded;

	public bool AddBatchListeners { get; set; }

	public BGCalcFlowEvents(Func<string> onChange)
	{
		this.onChange = onChange;
	}

	public void AddOnCreate(BGMetaEntity meta)
	{
		for (int i = 0; i < createRow.Count; i++)
		{
			EventsMetaData eventsMetaData = createRow[i];
			if (eventsMetaData.MetaId == meta.Id)
			{
				return;
			}
		}
		createRow.Add(new EventsMetaData(meta.Id));
	}

	public void AddOnDelete(BGMetaEntity meta)
	{
		for (int i = 0; i < deleteRow.Count; i++)
		{
			EventsMetaData eventsMetaData = deleteRow[i];
			if (eventsMetaData.MetaId == meta.Id)
			{
				return;
			}
		}
		deleteRow.Add(new EventsMetaData(meta.Id));
	}

	public void AddOnEdit(BGField field, BGEntity entity)
	{
		bool flag = false;
		for (int i = 0; i < fields.Count; i++)
		{
			EventsFieldData eventsFieldData = fields[i];
			if (!(field.Id != eventsFieldData.FieldId))
			{
				flag = true;
			}
		}
		if (flag)
		{
			return;
		}
		for (int j = 0; j < editRow.Count; j++)
		{
			EventsRowsData eventsRowsData = editRow[j];
			if (field.Id != eventsRowsData.FieldId)
			{
				continue;
			}
			flag = eventsRowsData.EntityIds.Contains(entity.Id);
			if (!flag)
			{
				if (eventsRowsData.EntityIds.Count > 5)
				{
					editRow.RemoveAt(j);
					fields.Add(new EventsFieldData(field.MetaId, field.Id));
				}
				else
				{
					eventsRowsData.EntityIds.Add(entity.Id);
				}
				flag = true;
			}
			break;
		}
		if (!flag)
		{
			EventsRowsData eventsRowsData2 = new EventsRowsData(field.MetaId, field.Id);
			eventsRowsData2.EntityIds.Add(entity.Id);
			editRow.Add(eventsRowsData2);
		}
	}

	public void AddListeners()
	{
		if ((!Application.isPlaying && !BGUtil.TestIsRunning) || listenersAdded)
		{
			return;
		}
		listenersAdded = true;
		foreach (EventsMetaData item in createRow)
		{
			BGMetaEntity meta = BGRepo.I.GetMeta(item.MetaId);
			if (meta != null)
			{
				meta.AnyEntityAdded += EntityAdded;
			}
		}
		foreach (EventsMetaData item2 in deleteRow)
		{
			BGMetaEntity meta2 = BGRepo.I.GetMeta(item2.MetaId);
			if (meta2 != null)
			{
				meta2.AnyEntityDeleted += EntityDeleted;
			}
		}
		foreach (EventsFieldData field in fields)
		{
			BGField bGField = BGRepo.I.GetMeta(field.MetaId)?.GetField(field.FieldId, errorIfNotFound: false);
			if (bGField != null)
			{
				bGField.ValueChanged += FieldHandler;
			}
		}
		foreach (EventsRowsData item3 in editRow)
		{
			BGField bGField2 = BGRepo.I.GetMeta(item3.MetaId)?.GetField(item3.FieldId, errorIfNotFound: false);
			if (bGField2 != null)
			{
				bGField2.ValueChanged += FieldHandlerByEntities;
			}
		}
		if (AddBatchListeners)
		{
			BGRepo.I.Events.OnBatchUpdate += OnBatch;
			BGRepo.OnLoad += OnLoad;
		}
	}

	public void Clear()
	{
		DisposeListeners();
		fields.Clear();
		editRow.Clear();
		createRow.Clear();
		deleteRow.Clear();
		listenersAdded = false;
	}

	private void DisposeListeners()
	{
		foreach (EventsFieldData field in fields)
		{
			BGField bGField = BGRepo.I.GetMeta(field.MetaId)?.GetField(field.FieldId, errorIfNotFound: false);
			if (bGField != null)
			{
				bGField.ValueChanged -= FieldHandler;
			}
		}
		foreach (EventsRowsData item in editRow)
		{
			BGField bGField2 = BGRepo.I.GetMeta(item.MetaId)?.GetField(item.FieldId, errorIfNotFound: false);
			if (bGField2 != null)
			{
				bGField2.ValueChanged -= FieldHandlerByEntities;
			}
		}
		foreach (EventsMetaData item2 in createRow)
		{
			BGMetaEntity meta = BGRepo.I.GetMeta(item2.MetaId);
			if (meta != null)
			{
				meta.AnyEntityAdded -= EntityAdded;
			}
		}
		foreach (EventsMetaData item3 in deleteRow)
		{
			BGMetaEntity meta2 = BGRepo.I.GetMeta(item3.MetaId);
			if (meta2 != null)
			{
				meta2.AnyEntityDeleted -= EntityDeleted;
			}
		}
		if (AddBatchListeners)
		{
			BGRepo.I.Events.OnBatchUpdate -= OnBatch;
			BGRepo.OnLoad -= OnLoad;
		}
	}

	private void OnLoad(bool success)
	{
		if (success)
		{
			onChange();
		}
	}

	private void OnBatch(object sender, BGEventArgsBatch e)
	{
		bool flag = false;
		if (e.EverythingChanged)
		{
			flag = true;
		}
		else
		{
			for (int i = 0; i < editRow.Count; i++)
			{
				EventsRowsData eventsRowsData = editRow[i];
				if (e.WasEntitiesUpdated(eventsRowsData.MetaId))
				{
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				for (int j = 0; j < createRow.Count; j++)
				{
					EventsMetaData eventsMetaData = createRow[j];
					if (e.WasEntitiesAdded(eventsMetaData.MetaId))
					{
						flag = true;
						break;
					}
				}
			}
			if (!flag)
			{
				for (int k = 0; k < deleteRow.Count; k++)
				{
					EventsMetaData eventsMetaData2 = deleteRow[k];
					if (e.WasEntitiesDeleted(eventsMetaData2.MetaId))
					{
						flag = true;
						break;
					}
				}
			}
		}
		if (flag)
		{
			onChange();
		}
	}

	private void FieldHandler(object sender, BGEventArgsField e)
	{
		onChange();
	}

	private void FieldHandlerByEntities(object sender, BGEventArgsField e)
	{
		bool flag = false;
		for (int i = 0; i < editRow.Count; i++)
		{
			EventsRowsData eventsRowsData = editRow[i];
			if (eventsRowsData.EntityIds.Contains(e.Entity.Id))
			{
				flag = true;
				break;
			}
		}
		if (flag)
		{
			onChange();
		}
	}

	private void EntityDeleted(object sender, BGEventArgsAnyEntity e)
	{
		onChange();
	}

	private void EntityAdded(object sender, BGEventArgsAnyEntity e)
	{
		onChange();
	}
}
