using System;
using System.Collections.Generic;
using System.Text;

namespace BansheeGz.BGDatabase;

[FieldDescriptor(Name = "listString", Folder = "List/Primitive", ManagerType = "BansheeGz.BGDatabase.Editor.BGFieldManagerListString")]
public class BGFieldListString : BGFieldCachedClassListA<string>, BGBinaryBulkLoaderClass
{
	public const ushort CodeType = 19;

	private static readonly List<byte> TempList = new List<byte>();

	public override ushort TypeCode => 19;

	public BGFieldListString(BGMetaEntity meta, string name)
		: base(meta, name)
	{
	}

	internal BGFieldListString(BGMetaEntity meta, BGId id, string name)
		: base(meta, id, name)
	{
	}

	public override byte[] ToBytes(int entityIndex)
	{
		List<string> list = this[entityIndex];
		int num = list?.Count ?? 0;
		if (num == 0)
		{
			return null;
		}
		TempList.Clear();
		TempList.AddRange(BGFieldInt.ValueToBytes(num));
		for (int i = 0; i < list.Count; i++)
		{
			string value = list[i];
			byte[] array = BGFieldStringA.ValueToBytes(value);
			if (array == null)
			{
				TempList.AddRange(BGFieldInt.ValueToBytes(0));
				continue;
			}
			TempList.AddRange(BGFieldInt.ValueToBytes(array.Length));
			TempList.AddRange(array);
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
		List<string> list = this[entityIndex] ?? new List<string>();
		list.Clear();
		int num2 = 4;
		for (int i = 0; i < num; i++)
		{
			int num3 = BGFieldInt.ValueFromBytes(new ArraySegment<byte>(segment.Array, segment.Offset + num2, 4));
			num2 += 4;
			if (num3 == 0)
			{
				list.Add("");
				continue;
			}
			list.Add(Encoding.UTF8.GetString(segment.Array, segment.Offset + num2, num3));
			num2 += num3;
		}
		this[entityIndex] = list;
	}

	public void FromBytes(BGBinaryBulkRequestClass request)
	{
		byte[] array = request.Array;
		BGBinaryBulkRequestClass.CellRequest[] cellRequests = request.CellRequests;
		int num = cellRequests.Length;
		Encoding uTF = Encoding.UTF8;
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
				List<string> list = StoreItems[entityIndex];
				if (list == null)
				{
					list = new List<string>(num2);
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
				int num3 = offset + 4;
				for (int j = 0; j < num2; j++)
				{
					int num4 = (array[num3 + 3] << 24) | (array[num3 + 2] << 16) | (array[num3 + 1] << 8) | array[num3];
					num3 += 4;
					if (num4 == 0)
					{
						list.Add("");
						continue;
					}
					list.Add(uTF.GetString(array, num3, num4));
					num3 += num4;
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
		List<string> list = this[entityIndex];
		if (BGUtil.IsEmpty(list))
		{
			return null;
		}
		string text = "";
		char c = StringValueSeparator[0];
		for (int i = 0; i < list.Count; i++)
		{
			string text2 = list[i];
			if (!string.IsNullOrEmpty(text2))
			{
				if (text.Length > 0)
				{
					text += c;
				}
				text += text2.Replace("\\", "\\\\").Replace(c.ToString() ?? "", "\\" + c);
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
		List<string> list = this[entityIndex] ?? new List<string>();
		list.Clear();
		Split(list, value, delimiter, '\\');
		this[entityIndex] = list;
	}

	public static void Split(List<string> list, string text, char delimiter, char escapeChar, bool keepEscape = false)
	{
		bool flag = false;
		StringBuilder stringBuilder = new StringBuilder();
		foreach (char c in text)
		{
			if (flag)
			{
				if (keepEscape)
				{
					stringBuilder.Append(escapeChar);
				}
				stringBuilder.Append(c);
				flag = false;
			}
			else if (c == delimiter)
			{
				if (stringBuilder.Length > 0)
				{
					list.Add(stringBuilder.ToString());
					stringBuilder.Remove(0, stringBuilder.Length);
				}
			}
			else if (c == escapeChar)
			{
				flag = true;
			}
			else
			{
				stringBuilder.Append(c);
			}
		}
		if (stringBuilder.Length > 0)
		{
			list.Add(stringBuilder.ToString());
		}
	}

	protected override Func<BGMetaEntity, BGId, string, BGField> CreateFieldFactory()
	{
		return (BGMetaEntity meta, BGId id, string name) => new BGFieldListString(meta, id, name);
	}

	protected override bool AreEqual(string myValue, string myValue2)
	{
		return string.Equals(myValue, myValue2);
	}
}
