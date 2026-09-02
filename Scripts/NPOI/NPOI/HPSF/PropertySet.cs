using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using NPOI.HPSF.Wellknown;
using NPOI.Util;

namespace NPOI.HPSF;

[Serializable]
public class PropertySet
{
	protected static byte[] BYTE_ORDER_ASSERTION = new byte[2] { 254, 255 };

	protected int byteOrder;

	protected static byte[] FORMAT_ASSERTION = new byte[2];

	protected int format;

	protected int osVersion;

	public const int OS_WIN16 = 0;

	public const int OS_MACINTOSH = 1;

	public const int OS_WIN32 = 2;

	[NonSerialized]
	protected ClassID classID;

	protected List<Section> sections;

	public virtual int ByteOrder
	{
		get
		{
			return byteOrder;
		}
		set
		{
			byteOrder = value;
		}
	}

	public virtual int Format
	{
		get
		{
			return format;
		}
		set
		{
			format = value;
		}
	}

	public virtual int OSVersion
	{
		get
		{
			return osVersion;
		}
		set
		{
			osVersion = value;
		}
	}

	public virtual ClassID ClassID
	{
		get
		{
			return classID;
		}
		set
		{
			classID = value;
		}
	}

	public virtual int SectionCount => sections.Count;

	public virtual List<Section> Sections => sections;

	public virtual bool IsSummaryInformation
	{
		get
		{
			if (sections.Count <= 0)
			{
				return false;
			}
			return Arrays.Equals(sections[0].FormatID.Bytes, SectionIDMap.SUMMARY_INFORMATION_ID);
		}
	}

	public virtual bool IsDocumentSummaryInformation
	{
		get
		{
			if (sections.Count <= 0)
			{
				return false;
			}
			return Arrays.Equals(sections[0].FormatID.Bytes, SectionIDMap.DOCUMENT_SUMMARY_INFORMATION_ID1);
		}
	}

	public virtual Property[] Properties => FirstSection.Properties;

	public virtual bool WasNull => FirstSection.WasNull;

	public virtual Section FirstSection
	{
		get
		{
			if (SectionCount < 1)
			{
				throw new MissingSectionException("Property Set does not contain any sections.");
			}
			return sections[0];
		}
	}

	public Section SingleSection
	{
		get
		{
			int sectionCount = SectionCount;
			if (sectionCount != 1)
			{
				throw new NoSingleSectionException("Property Set Contains " + sectionCount + " sections.");
			}
			return sections[0];
		}
	}

	protected PropertySet()
	{
	}

	public PropertySet(Stream stream)
	{
		if (IsPropertySetStream(stream))
		{
			byte[] array = new byte[(stream as ByteArrayInputStream).Available()];
			stream.Read(array, 0, array.Length);
			init(array, 0, array.Length);
			return;
		}
		throw new NoPropertySetStreamException("this stream may not be a valid property set stream");
	}

	public PropertySet(byte[] stream, int offset, int Length)
	{
		if (IsPropertySetStream(stream, offset, Length))
		{
			init(stream, offset, Length);
			return;
		}
		throw new NoPropertySetStreamException();
	}

	public PropertySet(byte[] stream)
		: this(stream, 0, stream.Length)
	{
	}

	public static bool IsPropertySetStream(Stream stream)
	{
		ByteArrayInputStream byteArrayInputStream = stream as ByteArrayInputStream;
		int num = 50;
		if (byteArrayInputStream == null || !byteArrayInputStream.MarkSupported())
		{
			throw new MarkUnsupportedException(stream.GetType().Name);
		}
		byteArrayInputStream.Mark(num);
		byte[] array = new byte[num];
		int length = stream.Read(array, 0, Math.Min(array.Length, byteArrayInputStream.Available()));
		bool result = IsPropertySetStream(array, 0, length);
		byteArrayInputStream.Reset();
		return result;
	}

	public static bool IsPropertySetStream(byte[] src, int offset, int Length)
	{
		int num = offset;
		int uShort = LittleEndian.GetUShort(src, num);
		num += 2;
		byte[] array = new byte[2];
		LittleEndian.PutShort(array, 0, (short)uShort);
		if (!Arrays.Equals(array, BYTE_ORDER_ASSERTION))
		{
			return false;
		}
		int uShort2 = LittleEndian.GetUShort(src, num);
		num += 2;
		byte[] array2 = new byte[2];
		LittleEndian.PutShort(array2, 0, (short)uShort2);
		if (!Arrays.Equals(array2, FORMAT_ASSERTION))
		{
			return false;
		}
		LittleEndian.GetUInt(src, offset);
		num += 4;
		new ClassID(src, offset);
		num += 16;
		long uInt = LittleEndian.GetUInt(src, num);
		num += 4;
		if (uInt < 0)
		{
			return false;
		}
		return true;
	}

	private void init(byte[] src, int offset, int Length)
	{
		int num = offset;
		byteOrder = LittleEndian.GetUShort(src, num);
		num += 2;
		format = LittleEndian.GetUShort(src, num);
		num += 2;
		osVersion = (int)LittleEndian.GetUInt(src, num);
		num += 4;
		classID = new ClassID(src, num);
		num += 16;
		int num2 = LittleEndian.GetInt(src, num);
		num += 4;
		if (num2 < 0)
		{
			throw new HPSFRuntimeException("Section count " + num2 + " is negative.");
		}
		sections = new List<Section>(num2);
		for (int i = 0; i < num2; i++)
		{
			Section item = new Section(src, num);
			num += ClassID.Length + 4;
			sections.Add(item);
		}
	}

	public virtual object GetProperty(int id)
	{
		return FirstSection.GetProperty(id);
	}

	public virtual bool GetPropertyBooleanValue(int id)
	{
		return FirstSection.GetPropertyBooleanValue(id);
	}

	public virtual int GetPropertyIntValue(int id)
	{
		return FirstSection.GetPropertyIntValue(id);
	}

	public override bool Equals(object o)
	{
		if (o == null || !(o is PropertySet))
		{
			return false;
		}
		PropertySet propertySet = (PropertySet)o;
		int num = propertySet.ByteOrder;
		int num2 = ByteOrder;
		ClassID classID = propertySet.ClassID;
		ClassID obj = ClassID;
		int num3 = propertySet.Format;
		int num4 = Format;
		int oSVersion = propertySet.OSVersion;
		int oSVersion2 = OSVersion;
		int sectionCount = propertySet.SectionCount;
		int sectionCount2 = SectionCount;
		if (num != num2 || !classID.Equals(obj) || num3 != num4 || oSVersion != oSVersion2 || sectionCount != sectionCount2)
		{
			return false;
		}
		return Util.AreEqual(Sections, propertySet.Sections);
	}

	public override int GetHashCode()
	{
		throw new InvalidOperationException("FIXME: Not yet implemented.");
	}

	public override string ToString()
	{
		StringBuilder stringBuilder = new StringBuilder();
		int sectionCount = SectionCount;
		stringBuilder.Append(GetType().Name);
		stringBuilder.Append('[');
		stringBuilder.Append("byteOrder: ");
		stringBuilder.Append(ByteOrder);
		stringBuilder.Append(", classID: ");
		stringBuilder.Append(ClassID);
		stringBuilder.Append(", format: ");
		stringBuilder.Append(Format);
		stringBuilder.Append(", OSVersion: ");
		stringBuilder.Append(OSVersion);
		stringBuilder.Append(", sectionCount: ");
		stringBuilder.Append(sectionCount);
		stringBuilder.Append(", sections: [\n");
		foreach (Section section in Sections)
		{
			stringBuilder.Append(section.ToString());
		}
		stringBuilder.Append(']');
		stringBuilder.Append(']');
		return stringBuilder.ToString();
	}
}
