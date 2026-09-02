using System;

namespace BansheeGz.BGDatabase;

public class BGMTFieldEnumShort : BGMTFieldEnumA<short>
{
	protected internal override Enum this[int entityIndex]
	{
		get
		{
			return (Enum)Enum.ToObject(enumType, GetStoredValue(entityIndex));
		}
		set
		{
			SetStoredValue(entityIndex, Convert.ToInt16(value));
		}
	}

	internal BGMTFieldEnumShort(BGField field)
		: base(field)
	{
	}

	internal BGMTFieldEnumShort(BGMTMeta meta, BGMTFieldEnumShort otherField)
		: base(meta, (BGMTFieldEnumA<short>)otherField)
	{
	}

	internal override BGMTField DeepClone(BGMTMeta meta)
	{
		return new BGMTFieldEnumShort(meta, this);
	}
}
