using System;

namespace BansheeGz.BGDatabase;

public class BGMetaPartitionModelDefault : BGMetaPartitionModelA, BGMetaPartitionModelA.FieldOwner, BGMetaPartitionModelI
{
	private readonly BGPartitionFieldTypeEnum fieldType;

	private readonly BGField field;

	public BGPartitionFieldTypeEnum FieldType => fieldType;

	public BGField Field => this.field;

	public BGMetaPartitionModelDefault(BGField field)
		: base(field.Meta)
	{
		this.field = field;
		if (!(field is BGFieldByte))
		{
			if (!(field is BGFieldShort))
			{
				if (!(field is BGFieldInt))
				{
					if (!(field is BGFieldByteNullable))
					{
						if (!(field is BGFieldShortNullable))
						{
							if (!(field is BGFieldIntNullable))
							{
								if (field is BGFieldRelationSingle)
								{
									fieldType = BGPartitionFieldTypeEnum.Relation;
								}
							}
							else
							{
								fieldType = BGPartitionFieldTypeEnum.NullableInt;
							}
						}
						else
						{
							fieldType = BGPartitionFieldTypeEnum.NullableShort;
						}
					}
					else
					{
						fieldType = BGPartitionFieldTypeEnum.NullableByte;
					}
				}
				else
				{
					fieldType = BGPartitionFieldTypeEnum.Int;
				}
			}
			else
			{
				fieldType = BGPartitionFieldTypeEnum.Short;
			}
		}
		else
		{
			fieldType = BGPartitionFieldTypeEnum.Byte;
		}
	}

	public override int? GetPartitionIndex(BGEntity entity)
	{
		switch (fieldType)
		{
		case BGPartitionFieldTypeEnum.Relation:
			return ((BGFieldRelationSingle)field)[entity.Index]?.Index;
		case BGPartitionFieldTypeEnum.Byte:
		{
			byte b = ((BGFieldByte)field)[entity.Index];
			if (b == 0)
			{
				return null;
			}
			return b - 1;
		}
		case BGPartitionFieldTypeEnum.Short:
		{
			short num = ((BGFieldShort)field)[entity.Index];
			if (num == 0)
			{
				return null;
			}
			return num - 1;
		}
		case BGPartitionFieldTypeEnum.Int:
		{
			int num3 = ((BGFieldInt)field)[entity.Index];
			if (num3 == 0)
			{
				return null;
			}
			return num3 - 1;
		}
		case BGPartitionFieldTypeEnum.NullableByte:
		{
			byte? b2 = ((BGFieldByteNullable)field)[entity.Index];
			if (!b2.HasValue)
			{
				return null;
			}
			return b2.Value;
		}
		case BGPartitionFieldTypeEnum.NullableShort:
		{
			short? num2 = ((BGFieldShortNullable)field)[entity.Index];
			if (!num2.HasValue)
			{
				return null;
			}
			return num2.Value;
		}
		case BGPartitionFieldTypeEnum.NullableInt:
			return ((BGFieldIntNullable)field)[entity.Index] ?? ((int?)null);
		default:
			throw new ArgumentOutOfRangeException("fieldType");
		}
	}
}
