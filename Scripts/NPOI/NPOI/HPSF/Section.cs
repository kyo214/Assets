using System;
using System.Collections;
using System.Text;
using NPOI.HPSF.Wellknown;
using NPOI.Util;

namespace NPOI.HPSF;

public class Section
{
	private class PropertyListEntry : IComparable
	{
		public int id;

		public int offset;

		public int Length;

		public int CompareTo(object o)
		{
			if (!(o is PropertyListEntry))
			{
				throw new InvalidCastException(o.ToString());
			}
			int num = ((PropertyListEntry)o).offset;
			if (offset < num)
			{
				return -1;
			}
			if (offset == num)
			{
				return 0;
			}
			return 1;
		}

		public override int GetHashCode()
		{
			int num = 1;
			num = 31 * num + id;
			num = 31 * num + Length;
			return 31 * num + offset;
		}

		public override bool Equals(object obj)
		{
			if (this == obj)
			{
				return true;
			}
			if (obj == null)
			{
				return false;
			}
			if (GetType() != obj.GetType())
			{
				return false;
			}
			PropertyListEntry propertyListEntry = (PropertyListEntry)obj;
			if (id != propertyListEntry.id)
			{
				return false;
			}
			if (Length != propertyListEntry.Length)
			{
				return false;
			}
			if (offset != propertyListEntry.offset)
			{
				return false;
			}
			return true;
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(GetType().Name);
			stringBuilder.Append("[id=");
			stringBuilder.Append(id);
			stringBuilder.Append(", offset=");
			stringBuilder.Append(offset);
			stringBuilder.Append(", Length=");
			stringBuilder.Append(Length);
			stringBuilder.Append(']');
			return stringBuilder.ToString();
		}
	}

	protected IDictionary dictionary;

	protected ClassID formatID;

	protected long offset;

	protected int size;

	protected Property[] properties;

	private bool wasNull;

	public ClassID FormatID => formatID;

	public long OffSet => offset;

	public virtual int Size => size;

	public virtual int PropertyCount => properties.Length;

	public virtual Property[] Properties => properties;

	public virtual bool WasNull => wasNull;

	public virtual IDictionary Dictionary
	{
		get
		{
			if (dictionary == null)
			{
				dictionary = new Hashtable();
			}
			return dictionary;
		}
		set
		{
			dictionary = value;
		}
	}

	public int Codepage
	{
		get
		{
			if (GetProperty(1L) == null)
			{
				return -1;
			}
			return (int)GetProperty(1L);
		}
	}

	protected Section()
	{
	}

	public Section(byte[] src, int offset)
	{
		int num = offset;
		formatID = new ClassID(src, num);
		num += 16;
		this.offset = LittleEndian.GetUInt(src, num);
		num = (int)this.offset;
		size = (int)LittleEndian.GetUInt(src, num);
		num += 4;
		int num2 = (int)LittleEndian.GetUInt(src, num);
		num += 4;
		properties = new Property[num2];
		int num3 = num;
		ArrayList arrayList = new ArrayList(num2);
		for (int i = 0; i < properties.Length; i++)
		{
			PropertyListEntry propertyListEntry = new PropertyListEntry
			{
				id = (int)LittleEndian.GetUInt(src, num3)
			};
			num3 += 4;
			propertyListEntry.offset = (int)LittleEndian.GetUInt(src, num3);
			num3 += 4;
			arrayList.Add(propertyListEntry);
		}
		arrayList.Sort();
		for (int j = 0; j < num2 - 1; j++)
		{
			PropertyListEntry propertyListEntry2 = (PropertyListEntry)arrayList[j];
			PropertyListEntry propertyListEntry3 = (PropertyListEntry)arrayList[j + 1];
			propertyListEntry2.Length = propertyListEntry3.offset - propertyListEntry2.offset;
		}
		if (num2 > 0)
		{
			PropertyListEntry propertyListEntry = (PropertyListEntry)arrayList[num2 - 1];
			propertyListEntry.Length = size - propertyListEntry.offset;
		}
		int num4 = -1;
		IEnumerator enumerator = arrayList.GetEnumerator();
		while (num4 == -1 && enumerator.MoveNext())
		{
			PropertyListEntry propertyListEntry = (PropertyListEntry)enumerator.Current;
			if (propertyListEntry.id == 1)
			{
				int num5 = (int)(this.offset + propertyListEntry.offset);
				long uInt = LittleEndian.GetUInt(src, num5);
				num5 += 4;
				if (uInt != 2)
				{
					throw new HPSFRuntimeException("Value type of property ID 1 is not VT_I2 but " + uInt + ".");
				}
				num4 = LittleEndian.GetUShort(src, num5);
			}
		}
		int num6 = 0;
		IEnumerator enumerator2 = arrayList.GetEnumerator();
		while (enumerator2.MoveNext())
		{
			PropertyListEntry propertyListEntry = (PropertyListEntry)enumerator2.Current;
			Property property = new Property(propertyListEntry.id, src, this.offset + propertyListEntry.offset, propertyListEntry.Length, num4);
			if (property.ID == 1)
			{
				property = new Property(property.ID, property.Type, num4);
			}
			properties[num6++] = property;
		}
		dictionary = (IDictionary)GetProperty(0L);
	}

	public virtual object GetProperty(long id)
	{
		wasNull = false;
		for (int i = 0; i < properties.Length; i++)
		{
			if (id == properties[i].ID)
			{
				return properties[i].Value;
			}
		}
		wasNull = true;
		return null;
	}

	public virtual int GetPropertyIntValue(long id)
	{
		object property = GetProperty(id);
		if (property == null)
		{
			return 0;
		}
		if (!(property is long) && !(property is int))
		{
			throw new HPSFRuntimeException("This property is not an integer type, but " + property.GetType().Name + ".");
		}
		return (int)property;
	}

	public virtual bool GetPropertyBooleanValue(int id)
	{
		if (GetProperty(id) != null)
		{
			return (bool)GetProperty(id);
		}
		return false;
	}

	public string GetPIDString(long pid)
	{
		string text = null;
		if (dictionary != null)
		{
			text = (string)dictionary[pid];
		}
		if (text == null)
		{
			text = SectionIDMap.GetPIDString(FormatID.Bytes, pid);
		}
		return text;
	}

	public override bool Equals(object o)
	{
		if (o == null || !(o is Section))
		{
			return false;
		}
		Section section = (Section)o;
		if (!section.FormatID.Equals(FormatID))
		{
			return false;
		}
		Property[] array = new Property[Properties.Length];
		Property[] array2 = new Property[section.Properties.Length];
		System.Array.Copy(Properties, 0, array, 0, array.Length);
		System.Array.Copy(section.Properties, 0, array2, 0, array2.Length);
		Property property = null;
		Property property2 = null;
		for (int i = 0; i < array.Length; i++)
		{
			long iD = array[i].ID;
			if (iD == 0L)
			{
				property = array[i];
				array = Remove(array, i);
				i--;
			}
			if (iD == 1)
			{
				array = Remove(array, i);
				i--;
			}
		}
		for (int j = 0; j < array2.Length; j++)
		{
			long iD2 = array2[j].ID;
			if (iD2 == 0L)
			{
				property2 = array2[j];
				array2 = Remove(array2, j);
				j--;
			}
			if (iD2 == 1)
			{
				array2 = Remove(array2, j);
				j--;
			}
		}
		if (array.Length != array2.Length)
		{
			return false;
		}
		bool flag = true;
		if (property != null && property2 != null)
		{
			Hashtable obj = (Hashtable)property.Value;
			Hashtable hashtable = (Hashtable)property2.Value;
			flag = obj.Count == hashtable.Count;
		}
		else if (property != null || property2 != null)
		{
			flag = false;
		}
		if (!flag)
		{
			return false;
		}
		return Util.AreEqual(array, array2);
	}

	private Property[] Remove(Property[] pa, int i)
	{
		Property[] array = new Property[pa.Length - 1];
		if (i > 0)
		{
			System.Array.Copy(pa, 0, array, 0, i);
		}
		System.Array.Copy(pa, i + 1, array, i, array.Length - i);
		return array;
	}

	public override int GetHashCode()
	{
		long num = 0L;
		num += FormatID.GetHashCode();
		Property[] array = Properties;
		for (int i = 0; i < array.Length; i++)
		{
			num += array[i].GetHashCode();
		}
		return (int)(num & 0xFFFFFFFFu);
	}

	public override string ToString()
	{
		StringBuilder stringBuilder = new StringBuilder();
		Property[] array = Properties;
		stringBuilder.Append(GetType().Name);
		stringBuilder.Append('[');
		stringBuilder.Append("formatID: ");
		stringBuilder.Append(FormatID);
		stringBuilder.Append(", offset: ");
		stringBuilder.Append(OffSet);
		stringBuilder.Append(", propertyCount: ");
		stringBuilder.Append(PropertyCount);
		stringBuilder.Append(", size: ");
		stringBuilder.Append(Size);
		stringBuilder.Append(", properties: [\n");
		for (int i = 0; i < array.Length; i++)
		{
			stringBuilder.Append(array[i].ToString());
			stringBuilder.Append(",\n");
		}
		stringBuilder.Append(']');
		stringBuilder.Append(']');
		return stringBuilder.ToString();
	}
}
