using System;
using System.Collections.Generic;

namespace BansheeGz.BGDatabase;

[FieldDescriptor(Name = "enumList", Folder = "Enum", ManagerType = "BansheeGz.BGDatabase.Editor.BGFieldManagerEnumList")]
public class BGFieldEnumList : BGFieldEnumListA<int>, BGBinaryBulkLoaderClass
{
	public const ushort CodeType = 11;

	private static readonly List<byte> TempList = new List<byte>();

	private static readonly List<string> TempStringList = new List<string>();

	public override ushort TypeCode => 11;

	public BGFieldEnumList(BGMetaEntity meta, string name, Type enumType)
		: base(meta, name, enumType)
	{
	}

	internal BGFieldEnumList(BGMetaEntity meta, BGId id, string name)
		: base(meta, id, name)
	{
	}

	protected override Func<BGMetaEntity, BGId, string, BGField> CreateFieldFactory()
	{
		return (BGMetaEntity meta, BGId id, string name) => new BGFieldEnumList(meta, id, name);
	}

	protected override bool AreEqual(Enum myValue, Enum myValue2)
	{
		return object.Equals(myValue, myValue2);
	}

	public override byte[] ToBytes(int entityIndex)
	{
		List<Enum> list = this[entityIndex];
		int num = list?.Count ?? 0;
		if (num == 0)
		{
			return null;
		}
		TempList.Clear();
		TempList.AddRange(BGFieldInt.ValueToBytes(num));
		for (int i = 0; i < list.Count; i++)
		{
			Enum value = list[i];
			TempList.AddRange(BGFieldInt.ValueToBytes(Convert.ToInt32(value)));
		}
		byte[] result = TempList.ToArray();
		TempList.Clear();
		return result;
	}

	public override void FromBytes(int entityIndex, ArraySegment<byte> segment)
	{
		if (segment.Count < 4)
		{
			ClearValueNoEvent(entityIndex);
			return;
		}
		int num = BGFieldInt.ValueFromBytes(new ArraySegment<byte>(segment.Array, segment.Offset, 4));
		if (num == 0)
		{
			ClearValueNoEvent(entityIndex);
			return;
		}
		List<Enum> list = this[entityIndex] ?? new List<Enum>();
		list.Clear();
		int num2 = 4;
		for (int i = 0; i < num; i++)
		{
			int value = BGFieldInt.ValueFromBytes(new ArraySegment<byte>(segment.Array, segment.Offset + num2, 4));
			num2 += 4;
			list.Add((Enum)Enum.ToObject(base.EnumType, value));
		}
		this[entityIndex] = list;
	}

	public void FromBytes(BGBinaryBulkRequestClass request)
	{
		byte[] array = request.Array;
		BGBinaryBulkRequestClass.CellRequest[] cellRequests = request.CellRequests;
		int num = cellRequests.Length;
		for (int i = 0; i < num; i++)
		{
			BGBinaryBulkRequestClass.CellRequest cellRequest = cellRequests[i];
			int entityIndex = cellRequest.EntityIndex;
			int offset = cellRequest.Offset;
			try
			{
				int num2 = (array[offset + 3] << 24) | (array[offset + 2] << 16) | (array[offset + 1] << 8) | array[offset];
				if (num2 == 0)
				{
					StoreItems[entityIndex] = null;
					continue;
				}
				List<Enum> list = StoreItems[entityIndex];
				if (list == null)
				{
					list = new List<Enum>(num2);
				}
				else
				{
					list.Clear();
					if (list.Capacity < num2)
					{
						list.Capacity = num2;
					}
				}
				StoreItems[entityIndex] = list;
				int num3 = 4;
				for (int j = 0; j < num2; j++)
				{
					int num4 = offset + num3;
					int value = (array[num4 + 3] << 24) | (array[num4 + 2] << 16) | (array[num4 + 1] << 8) | array[num4];
					num3 += 4;
					list.Add((Enum)Enum.ToObject(base.EnumType, value));
				}
			}
			catch (Exception obj)
			{
				request.OnError?.Invoke(obj);
			}
		}
	}

	public override string ToString(int entityIndex)
	{
		List<Enum> list = this[entityIndex];
		if (BGUtil.IsEmpty(list))
		{
			return null;
		}
		string text = "";
		char c = StringValueSeparator[0];
		for (int i = 0; i < list.Count; i++)
		{
			Enum value = list[i];
			string text2 = Enum.GetName(base.EnumType, value);
			if (!string.IsNullOrEmpty(text2))
			{
				if (text.Length > 0)
				{
					text += c;
				}
				text += text2;
			}
		}
		return text;
	}

	public override void FromString(int entityIndex, string value)
	{
		if (string.IsNullOrEmpty(value))
		{
			ClearValueNoEvent(entityIndex);
			return;
		}
		char delimiter = StringValueSeparator[0];
		List<Enum> list = this[entityIndex] ?? new List<Enum>();
		list.Clear();
		TempStringList.Clear();
		BGFieldListString.Split(TempStringList, value, delimiter, '\\');
		foreach (string tempString in TempStringList)
		{
			if (!Enum.IsDefined(base.EnumType, tempString))
			{
				throw new BGException("Invalid enum value $ for enum $, field=$, entity index=$", tempString, base.EnumType.FullName, base.FullName, entityIndex);
			}
			list.Add((Enum)Enum.Parse(base.EnumType, tempString));
		}
		this[entityIndex] = list;
	}
}
