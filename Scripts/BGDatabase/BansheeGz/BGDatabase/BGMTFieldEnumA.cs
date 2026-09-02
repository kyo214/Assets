using System;

namespace BansheeGz.BGDatabase;

public abstract class BGMTFieldEnumA<T> : BGMTFieldCached<Enum, T> where T : struct, IComparable, IConvertible, IFormattable
{
	protected Type enumType;

	protected BGMTFieldEnumA(BGField field)
		: base(field)
	{
		enumType = ((BGFieldEnumI)field).EnumType;
	}

	protected BGMTFieldEnumA(BGMTMeta meta, BGMTFieldEnumA<T> otherField)
		: base(meta, (BGMTFieldCached<Enum, T>)otherField)
	{
		enumType = otherField.enumType;
	}

	public override void CopyTo(BGField field, BGEntity entity, BGMTEntity fromEntity)
	{
		BGFieldEnumA<T> bGFieldEnumA = (BGFieldEnumA<T>)field;
		bGFieldEnumA.SetStoredValue(entity.Index, GetStoredValue(fromEntity.Index));
	}
}
