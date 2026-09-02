using System;

namespace BansheeGz.BGDatabase;

public class BGMTFieldEnum : BGMTFieldEnumA<int>
{
	protected internal override Enum this[int entityIndex]
	{
		get
		{
			return (Enum)Enum.ToObject(enumType, GetStoredValue(entityIndex));
		}
		set
		{
			SetStoredValue(entityIndex, Convert.ToInt32(value));
		}
	}

	internal BGMTFieldEnum(BGField field)
		: base(field)
	{
	}

	internal BGMTFieldEnum(BGMTMeta meta, BGMTFieldEnum otherField)
		: base(meta, (BGMTFieldEnumA<int>)otherField)
	{
	}

	internal override BGMTField DeepClone(BGMTMeta meta)
	{
		return new BGMTFieldEnum(meta, this);
	}
}
