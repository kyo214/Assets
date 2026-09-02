using System;

namespace BansheeGz.BGDatabase;

[FieldDescriptor(Name = "byteArray", Folder = "Special", ManagerType = "BansheeGz.BGDatabase.Editor.BGFieldManagerArrayByte")]
public class BGFieldArrayByte : BGFieldCachedStructArrayA<byte>, BGBinaryBulkLoaderClass
{
	public const ushort CodeType = 1;

	public override ushort TypeCode => 1;

	public BGFieldArrayByte(BGMetaEntity meta, string name)
		: base(meta, name)
	{
	}

	internal BGFieldArrayByte(BGMetaEntity meta, BGId id, string name)
		: base(meta, id, name)
	{
	}

	protected override Func<BGMetaEntity, BGId, string, BGField> CreateFieldFactory()
	{
		return (BGMetaEntity meta, BGId id, string name) => new BGFieldArrayByte(meta, id, name);
	}

	public override byte[] ToBytes(int entityIndex)
	{
		byte[] array = this[entityIndex];
		if (array == null || array.Length == 0)
		{
			return null;
		}
		return array;
	}

	public override void FromBytes(int entityIndex, ArraySegment<byte> segment)
	{
		if (segment.Count == 0)
		{
			ClearValueNoEvent(entityIndex);
		}
		else
		{
			StoreItems[entityIndex] = BGUtil.ToArray(segment);
		}
	}

	public virtual void FromBytes(BGBinaryBulkRequestClass request)
	{
		byte[] array = request.Array;
		BGBinaryBulkRequestClass.CellRequest[] cellRequests = request.CellRequests;
		int num = cellRequests.Length;
		for (int i = 0; i < num; i++)
		{
			BGBinaryBulkRequestClass.CellRequest cellRequest = cellRequests[i];
			try
			{
				byte[] array2 = new byte[cellRequest.Count];
				Buffer.BlockCopy(array, cellRequest.Offset, array2, 0, cellRequest.Count);
				StoreItems[cellRequest.EntityIndex] = array2;
			}
			catch (Exception obj)
			{
				request.OnError?.Invoke(obj);
			}
		}
	}

	public override string ToString(int entityIndex)
	{
		byte[] array = this[entityIndex];
		if (array == null || array.Length == 0)
		{
			return null;
		}
		string text = Convert.ToBase64String(array);
		if (text[0] == '+')
		{
			text = "'" + text;
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
		if (value.Length > 1 && value[0] == '\'')
		{
			value = value.Substring(1);
		}
		StoreItems[entityIndex] = Convert.FromBase64String(value);
	}

	protected override bool AreEqual(byte myValue, byte myValue2)
	{
		return myValue == myValue2;
	}
}
