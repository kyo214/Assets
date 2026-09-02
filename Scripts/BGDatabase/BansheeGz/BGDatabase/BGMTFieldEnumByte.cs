using System;

namespace BansheeGz.BGDatabase;

public class BGMTFieldEnumByte : BGMTFieldEnumA<byte>
{
	protected internal override Enum this[int entityIndex]
	{
		get
		{
			return (Enum)Enum.ToObject(enumType, GetStoredValue(entityIndex));
		}
		set
		{
			SetStoredValue(entityIndex, Convert.ToByte(value));
		}
	}

	internal BGMTFieldEnumByte(BGField field)
		: base(field)
	{
	}

	internal BGMTFieldEnumByte(BGMTMeta meta, BGMTFieldEnumByte otherField)
		: base(meta, (BGMTFieldEnumA<byte>)otherField)
	{
	}

	internal override BGMTField DeepClone(BGMTMeta meta)
	{
		return new BGMTFieldEnumByte(meta, this);
	}
}
