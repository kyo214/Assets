using System;
using System.Collections;
using System.Text;
using NPOI.Util;

namespace NPOI.HPSF;

public class Property
{
	protected long id;

	protected long type;

	protected object value;

	public virtual long ID
	{
		get
		{
			return id;
		}
		set
		{
			id = value;
		}
	}

	public virtual long Type
	{
		get
		{
			return type;
		}
		set
		{
			type = value;
		}
	}

	public virtual object Value
	{
		get
		{
			return value;
		}
		set
		{
			this.value = value;
		}
	}

	public int Count
	{
		get
		{
			int num = Variant.GetVariantLength(type);
			if (num >= 0)
			{
				return num;
			}
			if (num == -2)
			{
				throw new WritingNotSupportedException(type, null);
			}
			int num2 = 4;
			switch ((int)type)
			{
			case 30:
			{
				int num3 = ((string)value).Length + 1;
				int num4 = num3 % num2;
				if (num4 > 0)
				{
					num3 += num2 - num4;
				}
				num += num3;
				break;
			}
			default:
				throw new WritingNotSupportedException(type, value);
			case 0:
				break;
			}
			return num;
		}
	}

	public Property(long id, long type, object value)
	{
		this.id = id;
		this.type = type;
		this.value = value;
	}

	public Property(long id, byte[] src, long offset, int Length, int codepage)
	{
		this.id = id;
		if (id == 0L)
		{
			value = ReadDictionary(src, offset, Length, codepage);
			return;
		}
		int num = (int)offset;
		type = LittleEndian.GetUInt(src, num);
		num += 4;
		try
		{
			value = VariantSupport.Read(src, num, Length, (int)type, codepage);
		}
		catch (UnsupportedVariantTypeException ex)
		{
			VariantSupport.WriteUnsupportedTypeMessage(ex);
			value = ex.Value;
		}
	}

	protected Property()
	{
	}

	protected IDictionary ReadDictionary(byte[] src, long offset, int Length, int codepage)
	{
		if (offset < 0 || offset > src.Length)
		{
			throw new HPSFRuntimeException("Illegal offset " + offset + " while HPSF stream Contains " + Length + " bytes.");
		}
		int num = (int)offset;
		long uInt = LittleEndian.GetUInt(src, num);
		num += 4;
		Hashtable hashtable = new Hashtable((int)uInt, 1f);
		try
		{
			for (int i = 0; i < uInt; i++)
			{
				long uInt2 = LittleEndian.GetUInt(src, num);
				num += 4;
				long num2 = LittleEndian.GetUInt(src, num);
				num += 4;
				StringBuilder stringBuilder = new StringBuilder();
				switch (codepage)
				{
				case -1:
					stringBuilder.Append(Encoding.UTF8.GetString(src, num, (int)num2));
					break;
				case 1200:
				{
					int num3 = (int)(num2 * 2);
					byte[] array = new byte[num3];
					for (int j = 0; j < num3; j++)
					{
						array[j] = src[num + j];
					}
					stringBuilder.Append(Encoding.GetEncoding(codepage).GetString(array, 0, num3 - 2));
					break;
				}
				default:
					stringBuilder.Append(Encoding.GetEncoding(codepage).GetString(src, num, (int)num2));
					break;
				}
				while (stringBuilder.Length > 0 && stringBuilder[stringBuilder.Length - 1] == '\0')
				{
					stringBuilder.Length--;
				}
				if (codepage == 1200)
				{
					if (num2 % 2 == 1)
					{
						num2++;
					}
					num += (int)(num2 + num2);
				}
				else
				{
					num += (int)num2;
				}
				hashtable[uInt2] = stringBuilder.ToString();
			}
		}
		catch (Exception exception)
		{
			POILogFactory.GetLogger(typeof(Property)).Log(5, "The property Set's dictionary Contains bogus data. All dictionary entries starting with the one with ID " + id + " will be ignored.", exception);
		}
		return hashtable;
	}

	public override bool Equals(object o)
	{
		if (!(o is Property))
		{
			return false;
		}
		Property property = (Property)o;
		object obj = property.Value;
		long iD = property.ID;
		if (id != iD || (id != 0L && !TypesAreEqual(this.type, property.Type)))
		{
			return false;
		}
		if (value == null && obj == null)
		{
			return true;
		}
		if (value == null || obj == null)
		{
			return false;
		}
		Type type = value.GetType();
		Type type2 = obj.GetType();
		if (!type.IsAssignableFrom(type2) && !type2.IsAssignableFrom(type))
		{
			return false;
		}
		if (value is byte[])
		{
			return Arrays.Equals((byte[])value, (byte[])obj);
		}
		return value.Equals(obj);
	}

	private bool TypesAreEqual(long t1, long t2)
	{
		if (t1 == t2 || (t1 == 30 && t2 == 31) || (t2 == 30 && t1 == 31))
		{
			return true;
		}
		return false;
	}

	public override int GetHashCode()
	{
		long num = 0L;
		num += id;
		num += type;
		if (value != null)
		{
			num += value.GetHashCode();
		}
		return (int)(num & 0xFFFFFFFFu);
	}

	public override string ToString()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append(GetType().Name);
		stringBuilder.Append('[');
		stringBuilder.Append("id: ");
		stringBuilder.Append(ID);
		stringBuilder.Append(", type: ");
		stringBuilder.Append(GetType());
		object obj = Value;
		stringBuilder.Append(", value: ");
		if (obj is string)
		{
			stringBuilder.Append(obj.ToString());
			string text = obj.ToString();
			int length = text.Length;
			byte[] array = new byte[length * 2];
			for (int i = 0; i < length; i++)
			{
				char num = text[i];
				byte b = (byte)((num & 0xFF00) >> 8);
				byte b2 = (byte)(num & 0xFF);
				array[i * 2] = b;
				array[i * 2 + 1] = b2;
			}
			stringBuilder.Append(" [");
			if (array.Length != 0)
			{
				string text2 = HexDump.Dump(array, 0L, 0);
				stringBuilder.Append(text2);
			}
			stringBuilder.Append("]");
		}
		else if (obj is byte[])
		{
			byte[] array2 = (byte[])obj;
			if (array2.Length != 0)
			{
				string text3 = HexDump.Dump(array2, 0L, 0);
				stringBuilder.Append(text3);
			}
		}
		else
		{
			stringBuilder.Append(obj.ToString());
		}
		stringBuilder.Append(']');
		return stringBuilder.ToString();
	}
}
