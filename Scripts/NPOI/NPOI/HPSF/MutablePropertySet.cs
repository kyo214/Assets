using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using NPOI.POIFS.FileSystem;
using NPOI.Util;

namespace NPOI.HPSF;

[Serializable]
public class MutablePropertySet : PropertySet
{
	private int OFFSET_HEADER = PropertySet.BYTE_ORDER_ASSERTION.Length + PropertySet.FORMAT_ASSERTION.Length + 4 + 16 + 4;

	public override int ByteOrder
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

	public override int Format
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

	public override int OSVersion
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

	public override ClassID ClassID
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

	public MutablePropertySet()
	{
		byteOrder = LittleEndian.GetUShort(PropertySet.BYTE_ORDER_ASSERTION);
		format = LittleEndian.GetUShort(PropertySet.FORMAT_ASSERTION);
		osVersion = 133636;
		classID = new ClassID();
		sections = new List<Section>();
		sections.Add(new MutableSection());
	}

	public MutablePropertySet(PropertySet ps)
	{
		byteOrder = ps.ByteOrder;
		format = ps.Format;
		osVersion = ps.OSVersion;
		ClassID = ps.ClassID;
		ClearSections();
		if (sections == null)
		{
			sections = new List<Section>();
		}
		foreach (Section section2 in ps.Sections)
		{
			MutableSection section = new MutableSection(section2);
			AddSection(section);
		}
	}

	public virtual void ClearSections()
	{
		sections = null;
	}

	public virtual void AddSection(Section section)
	{
		if (sections == null)
		{
			sections = new List<Section>();
		}
		sections.Add(section);
	}

	public virtual void Write(Stream out1)
	{
		int count = sections.Count;
		int num = 0;
		num += TypeWriter.WriteToStream(out1, (short)ByteOrder);
		num += TypeWriter.WriteToStream(out1, (short)Format);
		num += TypeWriter.WriteToStream(out1, OSVersion);
		num += TypeWriter.WriteToStream(out1, ClassID);
		num += TypeWriter.WriteToStream(out1, count);
		int oFFSET_HEADER = OFFSET_HEADER;
		oFFSET_HEADER += count * (ClassID.Length + 4);
		int num2 = oFFSET_HEADER;
		IEnumerator enumerator = sections.GetEnumerator();
		while (enumerator.MoveNext())
		{
			MutableSection mutableSection = (MutableSection)enumerator.Current;
			if (mutableSection.FormatID == null)
			{
				throw new NoFormatIDException();
			}
			num += TypeWriter.WriteToStream(out1, mutableSection.FormatID);
			num += TypeWriter.WriteUIntToStream(out1, (uint)oFFSET_HEADER);
			oFFSET_HEADER += mutableSection.Size;
		}
		oFFSET_HEADER = num2;
		IEnumerator enumerator2 = sections.GetEnumerator();
		while (enumerator2.MoveNext())
		{
			MutableSection mutableSection2 = (MutableSection)enumerator2.Current;
			oFFSET_HEADER += mutableSection2.Write(out1);
		}
		out1.Close();
	}

	public virtual Stream ToInputStream()
	{
		using MemoryStream memoryStream = new MemoryStream();
		try
		{
			Write(memoryStream);
			memoryStream.Flush();
		}
		finally
		{
			memoryStream.Close();
		}
		return new MemoryStream(memoryStream.ToArray());
	}

	public virtual void Write(DirectoryEntry dir, string name)
	{
		try
		{
			dir.GetEntry(name).Delete();
		}
		catch (FileNotFoundException)
		{
		}
		dir.CreateDocument(name, ToInputStream());
	}
}
