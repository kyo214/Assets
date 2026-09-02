using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using NPOI.HPSF.Wellknown;
using NPOI.POIFS.FileSystem;
using NPOI.Util;

namespace NPOI.HPSF;

[Serializable]
public abstract class SpecialPropertySet : MutablePropertySet
{
	private MutablePropertySet delegate1;

	public abstract PropertyIDMap PropertySetIDMap { get; }

	public override int ByteOrder
	{
		get
		{
			return delegate1.ByteOrder;
		}
		set
		{
			delegate1.ByteOrder = value;
		}
	}

	public override int Format
	{
		get
		{
			return delegate1.Format;
		}
		set
		{
			delegate1.Format = value;
		}
	}

	public override ClassID ClassID
	{
		get
		{
			return delegate1.ClassID;
		}
		set
		{
			delegate1.ClassID = value;
		}
	}

	public override int SectionCount => delegate1.SectionCount;

	public override List<Section> Sections => delegate1.Sections;

	public override bool IsSummaryInformation => delegate1.IsSummaryInformation;

	public override bool IsDocumentSummaryInformation => delegate1.IsDocumentSummaryInformation;

	public override Section FirstSection => delegate1.FirstSection;

	public override int OSVersion
	{
		get
		{
			return delegate1.OSVersion;
		}
		set
		{
			delegate1.OSVersion = value;
		}
	}

	public override Property[] Properties => delegate1.Properties;

	public override bool WasNull => delegate1.WasNull;

	public SpecialPropertySet(PropertySet ps)
	{
		delegate1 = new MutablePropertySet(ps);
	}

	public SpecialPropertySet(MutablePropertySet ps)
	{
		delegate1 = ps;
	}

	public override Stream ToInputStream()
	{
		return delegate1.ToInputStream();
	}

	public override void AddSection(Section section)
	{
		delegate1.AddSection(section);
	}

	public override void ClearSections()
	{
		delegate1.ClearSections();
	}

	public override void Write(DirectoryEntry dir, string name)
	{
		delegate1.Write(dir, name);
	}

	public override void Write(Stream out1)
	{
		delegate1.Write(out1);
	}

	public override bool Equals(object o)
	{
		return delegate1.Equals(o);
	}

	public override object GetProperty(int id)
	{
		return delegate1.GetProperty(id);
	}

	public override bool GetPropertyBooleanValue(int id)
	{
		return delegate1.GetPropertyBooleanValue(id);
	}

	public override int GetPropertyIntValue(int id)
	{
		return delegate1.GetPropertyIntValue(id);
	}

	protected string GetPropertyStringValue(int propertyId)
	{
		return GetPropertyStringValue(GetProperty(propertyId));
	}

	protected static string GetPropertyStringValue(object propertyValue)
	{
		if (propertyValue == null)
		{
			return null;
		}
		if (propertyValue is string)
		{
			return (string)propertyValue;
		}
		if (propertyValue is byte[])
		{
			byte[] array = (byte[])propertyValue;
			if (array.Length == 0)
			{
				return "";
			}
			if (array.Length == 1)
			{
				return array[0].ToString();
			}
			if (array.Length == 2)
			{
				return LittleEndian.GetUShort(array).ToString();
			}
			if (array.Length == 4)
			{
				return LittleEndian.GetUInt(array).ToString();
			}
			return Encoding.UTF8.GetString(array);
		}
		return propertyValue.ToString();
	}

	public override int GetHashCode()
	{
		return delegate1.GetHashCode();
	}

	public override string ToString()
	{
		return delegate1.ToString();
	}
}
