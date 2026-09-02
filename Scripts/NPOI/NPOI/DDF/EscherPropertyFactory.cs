using System;
using System.Collections;
using System.Collections.Generic;
using NPOI.Util;

namespace NPOI.DDF;

public class EscherPropertyFactory
{
	public List<EscherProperty> CreateProperties(byte[] data, int offset, short numProperties)
	{
		List<EscherProperty> list = new List<EscherProperty>();
		int num = offset;
		for (int i = 0; i < numProperties; i++)
		{
			short num2 = LittleEndian.GetShort(data, num);
			int num3 = LittleEndian.GetInt(data, num + 2);
			short propertyId = (short)(num2 & 0x3FFF);
			bool flag = (num2 & -32768) != 0;
			byte propertyType = EscherProperties.GetPropertyType(propertyId);
			switch (propertyType)
			{
			case 1:
				list.Add(new EscherBoolProperty(num2, num3));
				break;
			case 2:
				list.Add(new EscherRGBProperty(num2, num3));
				break;
			case 3:
				list.Add(new EscherShapePathProperty(num2, num3));
				break;
			default:
				if (!flag)
				{
					list.Add(new EscherSimpleProperty(num2, num3));
				}
				else if (propertyType == 5)
				{
					list.Add(new EscherArrayProperty(num2, new byte[num3]));
				}
				else
				{
					list.Add(new EscherComplexProperty(num2, new byte[num3]));
				}
				break;
			}
			num += 6;
		}
		IEnumerator enumerator = list.GetEnumerator();
		while (enumerator.MoveNext())
		{
			EscherProperty escherProperty = (EscherProperty)enumerator.Current;
			if (!(escherProperty is EscherComplexProperty))
			{
				continue;
			}
			if (escherProperty is EscherArrayProperty)
			{
				num += ((EscherArrayProperty)escherProperty).SetArrayData(data, num);
				continue;
			}
			byte[] complexData = ((EscherComplexProperty)escherProperty).ComplexData;
			int num4 = data.Length - num;
			if (num4 < complexData.Length)
			{
				throw new InvalidOperationException("Could not read complex escher property, lenght was " + complexData.Length + ", but had only " + num4 + " bytes left");
			}
			Array.Copy(data, num, complexData, 0, complexData.Length);
			num += complexData.Length;
		}
		return list;
	}
}
