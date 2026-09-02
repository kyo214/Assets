using System;
using System.Collections;
using System.Globalization;
using System.IO;
using System.Text;
using NPOI.Util;

namespace NPOI.HPSF;

public class MutableSection : Section
{
	private class PropertyComparer : IComparer
	{
		int IComparer.Compare(object o1, object o2)
		{
			Property property = (Property)o1;
			Property property2 = (Property)o2;
			if (property.ID < property2.ID)
			{
				return -1;
			}
			if (property.ID == property2.ID)
			{
				return 0;
			}
			return 1;
		}
	}

	private bool dirty = true;

	private ArrayList preprops;

	private byte[] sectionBytes;

	public override int Size
	{
		get
		{
			if (dirty)
			{
				try
				{
					size = CalcSize();
					dirty = false;
				}
				catch (Exception)
				{
					throw;
				}
			}
			return size;
		}
	}

	public override int PropertyCount => preprops.Count;

	public override Property[] Properties
	{
		get
		{
			EnsureProperties();
			return properties;
		}
	}

	public override IDictionary Dictionary
	{
		get
		{
			return dictionary;
		}
		set
		{
			if (value != null)
			{
				IEnumerator enumerator = value.Keys.GetEnumerator();
				while (enumerator.MoveNext())
				{
					if (!(enumerator.Current is long) && !(enumerator.Current is int))
					{
						throw new IllegalPropertySetDataException("Dictionary keys must be of type long. but it's " + enumerator.Current?.ToString() + "," + enumerator.Current.GetType().Name + " now");
					}
				}
				dictionary = value;
				SetProperty(0, -1L, value);
				if (GetProperty(1L) == null)
				{
					SetProperty(1, 2L, 1200);
				}
			}
			else
			{
				RemoveProperty(0L);
			}
		}
	}

	public new int Codepage
	{
		get
		{
			return base.Codepage;
		}
		set
		{
			SetProperty(1, 2L, value);
		}
	}

	public MutableSection()
	{
		dirty = true;
		formatID = null;
		offset = -1L;
		preprops = new ArrayList();
	}

	public MutableSection(Section s)
	{
		SetFormatID(s.FormatID);
		Property[] array = s.Properties;
		MutableProperty[] array2 = new MutableProperty[array.Length];
		for (int i = 0; i < array.Length; i++)
		{
			array2[i] = new MutableProperty(array[i]);
		}
		Property[] array3 = array2;
		SetProperties(array3);
		Dictionary = s.Dictionary;
	}

	public void SetFormatID(ClassID formatID)
	{
		base.formatID = formatID;
	}

	public void SetFormatID(byte[] formatID)
	{
		ClassID classID = base.FormatID;
		if (classID == null)
		{
			classID = new ClassID();
			SetFormatID(classID);
		}
		classID.Bytes = formatID;
	}

	public void SetProperties(Property[] properties)
	{
		base.properties = properties;
		preprops = new ArrayList();
		for (int i = 0; i < properties.Length; i++)
		{
			preprops.Add(properties[i]);
		}
		dirty = true;
	}

	public void SetProperty(int id, string value)
	{
		SetProperty(id, 31L, value);
		dirty = true;
	}

	public void SetProperty(int id, int value)
	{
		SetProperty(id, 3L, value);
		dirty = true;
	}

	public void SetProperty(int id, long value)
	{
		SetProperty(id, 20L, value);
		dirty = true;
	}

	public void SetProperty(int id, bool value)
	{
		SetProperty(id, 11L, value);
		dirty = true;
	}

	public void SetProperty(int id, long variantType, object value)
	{
		MutableProperty mutableProperty = new MutableProperty();
		mutableProperty.ID = id;
		mutableProperty.Type = variantType;
		mutableProperty.Value = value;
		SetProperty(mutableProperty);
		dirty = true;
	}

	public void SetProperty(Property p)
	{
		long iD = p.ID;
		RemoveProperty(iD);
		preprops.Add(p);
		dirty = true;
	}

	public void RemoveProperty(long id)
	{
		IEnumerator enumerator = preprops.GetEnumerator();
		while (enumerator.MoveNext())
		{
			if (((Property)enumerator.Current).ID == id)
			{
				preprops.Remove(enumerator.Current);
				break;
			}
		}
		dirty = true;
	}

	protected void SetPropertyBooleanValue(int id, bool value)
	{
		SetProperty(id, 11L, value);
	}

	private int CalcSize()
	{
		using MemoryStream memoryStream = new MemoryStream();
		Write(memoryStream);
		sectionBytes = Util.Pad4(memoryStream.ToArray());
		return sectionBytes.Length;
	}

	public int Write(Stream out1)
	{
		if (!dirty && sectionBytes != null)
		{
			out1.Write(sectionBytes, 0, sectionBytes.Length);
			return sectionBytes.Length;
		}
		using MemoryStream memoryStream = new MemoryStream();
		using MemoryStream memoryStream2 = new MemoryStream();
		int num = 0;
		num += 8 + PropertyCount * 2 * 4;
		int num2 = -1;
		if (GetProperty(0L) != null)
		{
			object property = GetProperty(1L);
			if (property != null)
			{
				if (!(property is int))
				{
					throw new IllegalPropertySetDataException("The codepage property (ID = 1) must be an Integer object.");
				}
			}
			else
			{
				SetProperty(1, 2L, 1200);
			}
			num2 = Codepage;
		}
		preprops.Sort(new PropertyComparer());
		for (int i = 0; i < preprops.Count; i++)
		{
			MutableProperty mutableProperty = (MutableProperty)preprops[i];
			long iD = mutableProperty.ID;
			TypeWriter.WriteUIntToStream(memoryStream2, (uint)mutableProperty.ID);
			TypeWriter.WriteUIntToStream(memoryStream2, (uint)num);
			if (iD != 0L)
			{
				num += mutableProperty.Write(memoryStream, Codepage);
				continue;
			}
			if (num2 == -1)
			{
				throw new IllegalPropertySetDataException("Codepage (property 1) is undefined.");
			}
			num += WriteDictionary(memoryStream, dictionary, num2);
		}
		memoryStream.Flush();
		memoryStream2.Flush();
		byte[] array = memoryStream2.ToArray();
		byte[] array2 = memoryStream.ToArray();
		TypeWriter.WriteToStream(out1, 8 + array.Length + array2.Length);
		TypeWriter.WriteToStream(out1, PropertyCount);
		out1.Write(array, 0, array.Length);
		out1.Write(array2, 0, array2.Length);
		return 8 + array.Length + array2.Length;
	}

	private static int WriteDictionary(Stream out1, IDictionary dictionary, int codepage)
	{
		int num = TypeWriter.WriteUIntToStream(out1, (uint)dictionary.Count);
		IEnumerator enumerator = dictionary.Keys.GetEnumerator();
		while (enumerator.MoveNext())
		{
			long num2 = Convert.ToInt64(enumerator.Current, CultureInfo.InvariantCulture);
			string text = (string)dictionary[num2];
			if (text == null)
			{
				text = (string)dictionary[(int)num2];
			}
			if (codepage == 1200)
			{
				int num3 = text.Length + 1;
				if ((num3 & 1) == 1)
				{
					num3++;
				}
				num += TypeWriter.WriteUIntToStream(out1, (uint)num2);
				num += TypeWriter.WriteUIntToStream(out1, (uint)num3);
				byte[] bytes = Encoding.GetEncoding(codepage).GetBytes(text);
				for (int i = 0; i < bytes.Length; i++)
				{
					out1.WriteByte(bytes[i]);
					num++;
				}
				for (num3 -= text.Length; num3 > 0; num3--)
				{
					out1.WriteByte(0);
					out1.WriteByte(0);
					num += 2;
				}
				continue;
			}
			num += TypeWriter.WriteUIntToStream(out1, (uint)num2);
			num += TypeWriter.WriteUIntToStream(out1, (uint)(text.Length + 1));
			try
			{
				byte[] bytes2 = Encoding.GetEncoding(codepage).GetBytes(text);
				for (int j = 0; j < bytes2.Length; j++)
				{
					out1.WriteByte(bytes2[j]);
					num++;
				}
			}
			catch (Exception reason)
			{
				throw new IllegalPropertySetDataException(reason);
			}
			out1.WriteByte(0);
			num++;
		}
		return num;
	}

	public void EnsureProperties()
	{
		properties = (Property[])preprops.ToArray(typeof(Property));
	}

	public override object GetProperty(long id)
	{
		EnsureProperties();
		return base.GetProperty(id);
	}

	public void SetProperty(int id, object value)
	{
		if (value is string)
		{
			SetProperty(id, (string)value);
			return;
		}
		if (value is long)
		{
			SetProperty(id, (long)value);
			return;
		}
		if (value is int)
		{
			SetProperty(id, value);
			return;
		}
		if (value is short)
		{
			SetProperty(id, (short)value);
			return;
		}
		if (value is bool)
		{
			SetProperty(id, (bool)value);
			return;
		}
		if (value is DateTime)
		{
			SetProperty(id, 64L, value);
			return;
		}
		throw new HPSFRuntimeException("HPSF does not support properties of type " + value.GetType().Name + ".");
	}

	public void Clear()
	{
		Property[] array = Properties;
		foreach (Property property in array)
		{
			RemoveProperty(property.ID);
		}
	}
}
