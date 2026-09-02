using System;
using System.Text;
using System.Text.RegularExpressions;

namespace NPOI.Util;

public class ClassID
{
	public static ClassID OLE10_PACKAGE = new ClassID("{0003000C-0000-0000-C000-000000000046}");

	public static ClassID PPT_SHOW = new ClassID("{64818D10-4F9B-11CF-86EA-00AA00B929E8}");

	public static ClassID XLS_WORKBOOK = new ClassID("{00020841-0000-0000-C000-000000000046}");

	public static ClassID TXT_ONLY = new ClassID("{5e941d80-bf96-11cd-b579-08002b30bfeb}");

	public static ClassID EXCEL97 = new ClassID("{00020820-0000-0000-C000-000000000046}");

	public static ClassID EXCEL95 = new ClassID("{00020810-0000-0000-C000-000000000046}");

	public static ClassID WORD97 = new ClassID("{00020906-0000-0000-C000-000000000046}");

	public static ClassID WORD95 = new ClassID("{00020900-0000-0000-C000-000000000046}");

	public static ClassID POWERPOINT97 = new ClassID("{64818D10-4F9B-11CF-86EA-00AA00B929E8}");

	public static ClassID POWERPOINT95 = new ClassID("{EA7BAE70-FB3B-11CD-A903-00AA00510EA3}");

	public static ClassID EQUATION30 = new ClassID("{0002CE02-0000-0000-C000-000000000046}");

	protected byte[] bytes;

	public const int LENGTH = 16;

	public int Length => 16;

	public byte[] Bytes
	{
		get
		{
			return bytes;
		}
		set
		{
			for (int i = 0; i < bytes.Length; i++)
			{
				bytes[i] = value[i];
			}
		}
	}

	public ClassID(byte[] src, int offset)
	{
		Read(src, offset);
	}

	public ClassID()
	{
		bytes = new byte[16];
		for (int i = 0; i < 16; i++)
		{
			bytes[i] = 0;
		}
	}

	public ClassID(string externalForm)
	{
		bytes = new byte[16];
		string text = Regex.Replace(externalForm, "[{}-]", "");
		for (int i = 0; i < text.Length; i += 2)
		{
			bytes[i / 2] = (byte)Convert.ToInt64(text.Substring(i, 2), 16);
		}
	}

	public byte[] Read(byte[] src, int offset)
	{
		bytes = new byte[16];
		bytes[0] = src[3 + offset];
		bytes[1] = src[2 + offset];
		bytes[2] = src[1 + offset];
		bytes[3] = src[offset];
		bytes[4] = src[5 + offset];
		bytes[5] = src[4 + offset];
		bytes[6] = src[7 + offset];
		bytes[7] = src[6 + offset];
		for (int i = 8; i < 16; i++)
		{
			bytes[i] = src[i + offset];
		}
		return bytes;
	}

	public void Write(byte[] dst, int offset)
	{
		if (dst.Length < 16)
		{
			throw new ArrayTypeMismatchException("Destination byte[] must have room for at least 16 bytes, but has a length of only " + dst.Length + ".");
		}
		dst[offset] = bytes[3];
		dst[1 + offset] = bytes[2];
		dst[2 + offset] = bytes[1];
		dst[3 + offset] = bytes[0];
		dst[4 + offset] = bytes[5];
		dst[5 + offset] = bytes[4];
		dst[6 + offset] = bytes[7];
		dst[7 + offset] = bytes[6];
		for (int i = 8; i < 16; i++)
		{
			dst[i + offset] = bytes[i];
		}
	}

	public override bool Equals(object o)
	{
		if (o == null || !(o is ClassID))
		{
			return false;
		}
		ClassID classID = (ClassID)o;
		if (bytes.Length != classID.bytes.Length)
		{
			return false;
		}
		for (int i = 0; i < bytes.Length; i++)
		{
			if (bytes[i] != classID.bytes[i])
			{
				return false;
			}
		}
		return true;
	}

	public override int GetHashCode()
	{
		return Encoding.UTF8.GetString(bytes).GetHashCode();
	}

	public override string ToString()
	{
		StringBuilder stringBuilder = new StringBuilder(38);
		stringBuilder.Append('{');
		for (int i = 0; i < 16; i++)
		{
			stringBuilder.Append(HexDump.ToHex(bytes[i]));
			if (i == 3 || i == 5 || i == 7 || i == 9)
			{
				stringBuilder.Append('-');
			}
		}
		stringBuilder.Append('}');
		return stringBuilder.ToString();
	}
}
