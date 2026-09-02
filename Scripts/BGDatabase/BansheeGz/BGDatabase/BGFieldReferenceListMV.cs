using System;
using System.Collections.Generic;
using System.Text;

namespace BansheeGz.BGDatabase;

[FieldDescriptor(Name = "objectListMultiValueReference", Folder = "Unity Scene", ManagerType = "BansheeGz.BGDatabase.Editor.BGFieldManagerReferenceListMV")]
public class BGFieldReferenceListMV : BGFieldCachedA<List<BGWithId>, List<BGId>>, BGSceneObjectReferenceI
{
	public const ushort CodeType = 99;

	public override bool ReadOnly => true;

	public override ushort TypeCode => 99;

	public override List<BGWithId> this[int entityIndex]
	{
		get
		{
			List<BGId> storedValue = GetStoredValue(entityIndex);
			if (storedValue == null || storedValue.Count == 0)
			{
				return null;
			}
			List<BGWithId> list = new List<BGWithId>();
			foreach (BGId item in storedValue)
			{
				List<BGWithId> all = BGWithId.GetAll(item);
				if (all != null && all.Count != 0)
				{
					list.AddRange(all);
				}
			}
			return list;
		}
		set
		{
		}
	}

	public BGFieldReferenceListMV(BGMetaEntity meta, string name)
		: base(meta, name)
	{
	}

	internal BGFieldReferenceListMV(BGMetaEntity meta, BGId id, string name)
		: base(meta, id, name)
	{
	}

	protected override Func<BGMetaEntity, BGId, string, BGField> CreateFieldFactory()
	{
		return (BGMetaEntity meta, BGId id, string name) => new BGFieldReferenceListMV(meta, id, name);
	}

	public override void CopyValue(BGField fromField, BGId fromEntityId, int fromEntityIndex, BGId toEntityId)
	{
		if (fromEntityIndex == -1 || fromField.IsDeleted)
		{
			return;
		}
		int num = base.Meta.FindEntityIndex(toEntityId);
		if (num != -1)
		{
			BGFieldReferenceListMV bGFieldReferenceListMV = (BGFieldReferenceListMV)fromField;
			List<BGId> storedValue = bGFieldReferenceListMV.GetStoredValue(fromEntityIndex);
			if (storedValue == null || storedValue.Count == 0)
			{
				ClearValueNoEvent(num);
			}
			else
			{
				SetStoredValue(num, new List<BGId>(storedValue));
			}
		}
	}

	public override bool AreStoredValuesEqual(BGField field, int myEntityIndex, int otherEntityIndex)
	{
		if (!(field is BGFieldReferenceListMV bGFieldReferenceListMV))
		{
			return false;
		}
		List<BGId> storedValue = GetStoredValue(myEntityIndex);
		List<BGId> storedValue2 = bGFieldReferenceListMV.GetStoredValue(otherEntityIndex);
		bool flag = BGUtil.IsEmpty(storedValue);
		bool flag2 = BGUtil.IsEmpty(storedValue2);
		if (flag & flag2)
		{
			return true;
		}
		if (flag | flag2)
		{
			return false;
		}
		if (storedValue.Count != storedValue2.Count)
		{
			return false;
		}
		for (int i = 0; i < storedValue.Count; i++)
		{
			BGId bGId = storedValue[i];
			BGId bGId2 = storedValue2[i];
			if (bGId != bGId2)
			{
				return false;
			}
		}
		return true;
	}

	public override byte[] ToBytes(int entityIndex)
	{
		List<BGId> list = GetStoredValue(entityIndex);
		bool flag = list != null && list.Count > 0;
		BGBinaryWriter writer = new BGBinaryWriter(flag ? (4 + list.Count * 16) : 4);
		writer.AddArray(() =>
		{
			foreach (BGId item in list)
			{
				writer.AddId(item);
			}
		}, flag ? list.Count : 0);
		return writer.ToArray();
	}

	public override void FromBytes(int entityIndex, ArraySegment<byte> segment)
	{
		int num = BGFieldInt.ValueFromBytes(new ArraySegment<byte>(segment.Array, segment.Offset, 4));
		List<BGId> list;
		if (num > 0)
		{
			int num2 = segment.Offset + 4;
			list = new List<BGId>(num);
			for (int i = 0; i < num; i++)
			{
				list.Add(new BGId(segment.Array, num2 + i * 16));
			}
		}
		else
		{
			list = null;
		}
		StoreItems[entityIndex] = list;
	}

	public override string ToString(int entityIndex)
	{
		List<BGId> storedValue = GetStoredValue(entityIndex);
		if (storedValue == null || storedValue.Count == 0)
		{
			return null;
		}
		StringBuilder stringBuilder = new StringBuilder();
		foreach (BGId item in storedValue)
		{
			if (stringBuilder.Length != 0)
			{
				stringBuilder.Append('|');
			}
			stringBuilder.Append(item.ToString());
		}
		return stringBuilder.ToString();
	}

	public override void FromString(int entityIndex, string value)
	{
		List<BGId> list;
		if (!string.IsNullOrEmpty(value))
		{
			list = new List<BGId>();
			string[] array = value.Split(BGField<List<BGWithId>>.AA);
			string[] array2 = array;
			foreach (string value2 in array2)
			{
				if (BGId.TryParse(value2, out var item))
				{
					list.Add(item);
				}
			}
		}
		else
		{
			list = null;
		}
		StoreItems[entityIndex] = list;
	}

	public int CountValues(int entityIndex)
	{
		return this[entityIndex]?.Count ?? 0;
	}
}
