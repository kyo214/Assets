using System;

namespace BansheeGz.BGDatabase;

[FieldDescriptor(ManagerType = "BansheeGz.BGDatabase.Editor.BGFieldManagerEntityName")]
public class BGFieldEntityName : BGFieldString
{
	public new const ushort CodeType = 47;

	public const string NameFieldName = "name";

	private bool nameEmpty;

	public override ushort TypeCode => 47;

	public override string Description => "Entity's name";

	public bool NameEmpty
	{
		get
		{
			return nameEmpty;
		}
		internal set
		{
			if (nameEmpty != value)
			{
				nameEmpty = value;
				if (nameEmpty)
				{
					StoreClear();
				}
				else
				{
					base.OnCreate();
				}
			}
		}
	}

	public override string this[int entityIndex]
	{
		get
		{
			if (nameEmpty)
			{
				return null;
			}
			return ((BGFieldCachedA<string>)this)[entityIndex];
		}
		set
		{
			if (nameEmpty)
			{
				return;
			}
			string text = this[entityIndex];
			if (!string.Equals(value, text))
			{
				base[entityIndex] = value;
				base.Meta.OnEntityNameChange(entityIndex, text, value);
				base.Meta.ForEachField((BGField field) =>
				{
					field.OnNameChange(entityIndex);
				});
			}
		}
	}

	public override string this[BGId entityId]
	{
		get
		{
			if (nameEmpty)
			{
				return null;
			}
			return base[entityId];
		}
		set
		{
			if (!nameEmpty)
			{
				base[entityId] = value;
			}
		}
	}

	public static bool IsName(BGField field)
	{
		return "name".Equals(field.Name);
	}

	public BGFieldEntityName(BGMetaEntity meta, string name)
		: base(meta, "name")
	{
		nameEmpty = meta.EmptyName;
	}

	internal BGFieldEntityName(BGMetaEntity meta, BGId id, string name)
		: base(meta, id, "name")
	{
		nameEmpty = meta.EmptyName;
	}

	internal void SetEntityValue(int entityIndex, string value)
	{
		if (!nameEmpty)
		{
			StoreItems[entityIndex] = value;
		}
	}

	public override void CopyValue(BGField fromField, BGId fromEntityId, int fromEntityIndex, BGId toEntityId)
	{
		if (!nameEmpty)
		{
			base.Meta.InvalidateNameCache();
			base.CopyValue(fromField, fromEntityId, fromEntityIndex, toEntityId);
		}
	}

	public override void ForEachValue(Action<int> action)
	{
		if (!nameEmpty)
		{
			base.ForEachValue(action);
		}
	}

	public override byte[] ToBytes(int entityIndex)
	{
		if (nameEmpty)
		{
			return null;
		}
		return base.ToBytes(entityIndex);
	}

	public override void FromBytes(int entityIndex, ArraySegment<byte> segment)
	{
		if (!nameEmpty)
		{
			base.FromBytes(entityIndex, segment);
		}
	}

	public override void FromBytes(BGBinaryBulkRequestClass request)
	{
		if (!nameEmpty)
		{
			base.FromBytes(request);
		}
	}

	public override string ToString(int entityIndex)
	{
		if (nameEmpty)
		{
			return null;
		}
		return base.ToString(entityIndex);
	}

	public override void FromString(int entityIndex, string value)
	{
		if (!nameEmpty)
		{
			base.FromString(entityIndex, value);
		}
	}

	public override void ClearValues()
	{
		if (!nameEmpty)
		{
			base.ClearValues();
		}
	}

	public override void ClearValue(int entityIndex)
	{
		if (!nameEmpty)
		{
			base.ClearValue(entityIndex);
		}
	}

	public override void SetStoredValue(int entityIndex, string value)
	{
		if (!nameEmpty)
		{
			base.SetStoredValue(entityIndex, value);
		}
	}

	public override bool AreStoredValuesEqual(BGField field, int myEntityIndex, int otherEntityIndex)
	{
		if (nameEmpty)
		{
			if (!(field is BGField<string> bGField))
			{
				return false;
			}
			if (field is BGFieldEntityName { nameEmpty: not false })
			{
				return true;
			}
			return string.IsNullOrEmpty(bGField[otherEntityIndex]);
		}
		return base.AreStoredValuesEqual(field, myEntityIndex, otherEntityIndex);
	}

	public override void MoveEntitiesValues(int fromIndex, int toIndex, int numberOfValues)
	{
		if (!nameEmpty)
		{
			base.MoveEntitiesValues(fromIndex, toIndex, numberOfValues);
		}
	}

	public override void Swap(int entityIndex1, int entityIndex2)
	{
		if (!nameEmpty)
		{
			base.Swap(entityIndex1, entityIndex2);
		}
	}

	public override void OnEntityAdd(BGEntity entity)
	{
		if (!nameEmpty)
		{
			base.OnEntityAdd(entity);
		}
	}

	public override void OnEntityDelete(BGEntity entity)
	{
		if (!nameEmpty)
		{
			base.OnEntityDelete(entity);
		}
	}

	public override void OnCreate()
	{
		if (!nameEmpty)
		{
			base.OnCreate();
		}
	}

	protected override Func<BGMetaEntity, BGId, string, BGField> CreateFieldFactory()
	{
		return (BGMetaEntity meta, BGId id, string name) => new BGFieldEntityName(meta, id, name);
	}
}
