using System;
using System.Collections.Generic;

namespace BansheeGz.BGDatabase;

public class BGEntity : BGObject, BGAbstractEntityI, BGObjectWithNameI, BGObjectI, BGIndexableI, IComparable<BGEntity>, IEquatable<BGEntity>
{
	public interface EntityFactory
	{
		BGEntity NewEntity(BGMetaEntity meta);

		BGEntity NewEntity(BGMetaEntity meta, BGId id);
	}

	public virtual string Name
	{
		get
		{
			return Meta.NameField?[Index];
		}
		set
		{
			BGFieldEntityName nameField = Meta.NameField;
			if (nameField != null)
			{
				nameField[Index] = value;
			}
		}
	}

	public int Index { get; internal set; }

	public BGMetaEntity Meta { get; private set; }

	public BGId MetaId => Meta.Id;

	public string MetaName => Meta.Name;

	public string FullName => MetaName + "." + Name;

	public BGRepo Repo => Meta.Repo;

	protected internal BGEntity(BGMetaEntity meta)
		: this(meta, meta.NewEntityId)
	{
		Meta.OnEntityCreate(this);
	}

	protected internal BGEntity(BGMetaEntity meta, BGId id)
		: base(id)
	{
		Meta = meta ?? throw new BGException("Meta can not be null");
		Meta.Register(this);
	}

	public override void Delete()
	{
		if (!base.IsDeleted)
		{
			Exception exception = null;
			BGUtil.Catch(ref exception, () =>
			{
				Meta.FireEntityBeforeDelete(this);
			});
			base.Delete();
			BGUtil.Catch(ref exception, () =>
			{
				Meta.Unregister(this);
			});
			BGUtil.Catch(ref exception, () =>
			{
				Meta.FireEntityDeleted(this);
			});
			BGUtil.Catch(ref exception, Unload, () =>
			{
				Meta = null;
			});
			if (exception != null)
			{
				throw exception;
			}
		}
	}

	protected internal override void Unload()
	{
		Index = -1;
		base.Unload();
	}

	public BGEntity Duplicate()
	{
		if (Meta.IsManagingItsOwnEntities)
		{
			throw new Exception("This meta does not support entity duplication.");
		}
		return Meta.NewEntity(new BGMetaEntity.NewEntityContext((BGEntity e) =>
		{
			e.CopyFieldsValuesFromNoEvents(this);
		}));
	}

	public void CopyFieldsValuesFrom(BGEntity source, Predicate<BGField> fieldFilter = null)
	{
		if (source == null)
		{
			throw new Exception("Can not copy fields values: the source entity is null");
		}
		if (source.Id == base.Id)
		{
			throw new Exception("Can not copy fields values: the source entity is the same as target entity");
		}
		if (!Meta.Equals(source.Meta))
		{
			throw new Exception("Can not copy fields values: the source entity belongs to a different meta! Required meta=" + MetaName + ", source entity meta=" + source.MetaName);
		}
		List<BGField> changedFields = null;
		if (Meta.Repo.Events.On)
		{
			changedFields = new List<BGField>();
			Meta.ForEachField((BGField field) =>
			{
				if (!field.AreStoredValuesEqual(field, Index, source.Index))
				{
					changedFields.Add(field);
				}
			}, fieldFilter);
		}
		CopyFieldsValuesFromNoEvents(source);
		if (changedFields == null || changedFields.Count <= 0)
		{
			return;
		}
		foreach (BGField item in changedFields)
		{
			item.FireValueChanged(this);
		}
	}

	private void CopyFieldsValuesFromNoEvents(BGEntity source, Predicate<BGField> fieldFilter = null)
	{
		Meta.ForEachField((BGField field) =>
		{
			if (field is BGFieldNested bGFieldNested)
			{
				List<BGEntity> list = bGFieldNested[Index];
				if (list != null && list.Count > 0)
				{
					bGFieldNested.NestedMeta.DeleteEntities(list);
				}
			}
			field.DuplicateValue(source.Id, source.Index, base.Id);
		}, fieldFilter);
	}

	public int CompareTo(BGEntity other)
	{
		if (other != null)
		{
			return Index.CompareTo(other.Index);
		}
		return 0;
	}

	public bool Equals(BGEntity other)
	{
		if (other != null)
		{
			return base.Id == other.Id;
		}
		return false;
	}

	public void ClearFieldValue(BGId fieldId)
	{
		Meta.GetField(fieldId).ClearValue(Index);
	}

	public T Get<T>(BGField field)
	{
		if (field == null)
		{
			throw new BGException("Provided field is null");
		}
		if (field.MetaId != MetaId)
		{
			throw new BGException("Field does not belong to entity's Meta. Entity's meta ($), field's meta ($)", Meta.Name, field.Meta.Name);
		}
		if (field.Meta.Repo != Meta.Repo)
		{
			throw new BGException("Field does not belong to entity's Repo.");
		}
		if (!(field is BGField<T> bGField))
		{
			throw new BGException("Can not get a value! Field ($) has type ($), but provided generic parameter has ($) type", field.FullName, field.ValueType.FullName, typeof(T));
		}
		return bGField[Index];
	}

	public T Get<T>(BGId fieldId)
	{
		return Get<T>(Meta.GetField(fieldId));
	}

	public T Get<T>(string fieldName)
	{
		return Get<T>(Meta.GetField(fieldName));
	}

	public void Set<T>(BGField field, T value)
	{
		if (field == null)
		{
			throw new BGException("Provided field is null");
		}
		if (field.MetaId != MetaId)
		{
			throw new BGException("Field does not belong to entity's Meta. Entity's meta ($), field's meta ($)", Meta.Name, field.Meta.Name);
		}
		if (field.Meta.Repo != Meta.Repo)
		{
			throw new BGException("Field does not belong to entity's Repo.");
		}
		if (!(field is BGField<T> bGField))
		{
			throw new BGException("Can not set a value: value type mismatch! Field ($) has type ($), but provided value has ($) type", field.FullName, field.ValueType.FullName, typeof(T));
		}
		bGField[Index] = value;
	}

	public void Set<T>(string fieldName, T value)
	{
		Set(Meta.GetField(fieldName), value);
	}

	public void Set<T>(BGId fieldId, T value)
	{
		Set(Meta.GetField(fieldId), value);
	}

	public T MetaAs<T>() where T : BGMetaEntity
	{
		return (T)Meta;
	}

	public override string ToString()
	{
		return "Entity [id:" + base.Id.ToString() + (base.IsDeleted ? ", [deleted]" : (", name:" + Name)) + ((Meta == null) ? "" : (", meta=" + Meta.Name)) + "]";
	}
}
