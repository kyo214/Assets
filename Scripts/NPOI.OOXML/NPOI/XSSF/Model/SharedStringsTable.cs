using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using NPOI.OpenXml4Net.OPC;
using NPOI.OpenXmlFormats.Spreadsheet;

namespace NPOI.XSSF.Model;

public class SharedStringsTable : POIXMLDocumentPart
{
	private List<CT_Rst> strings = new List<CT_Rst>();

	private Dictionary<string, int> stmap = new Dictionary<string, int>();

	private int count;

	private int uniqueCount;

	private SstDocument _sstDoc;

	public int Count => count;

	public int UniqueCount => uniqueCount;

	public IList<CT_Rst> Items => strings.AsReadOnly();

	public SharedStringsTable()
	{
		_sstDoc = new SstDocument();
		_sstDoc.AddNewSst();
	}

	public SharedStringsTable(PackagePart part)
		: base(part)
	{
		ReadFrom(part.GetInputStream());
	}

	[Obsolete("deprecated in POI 3.14, scheduled for removal in POI 3.16")]
	public SharedStringsTable(PackagePart part, PackageRelationship rel)
		: this(part)
	{
	}

	public void ReadFrom(Stream is1)
	{
		try
		{
			int num = 0;
			XmlDocument xml = POIXMLDocumentPart.ConvertStreamToXml(is1);
			_sstDoc = SstDocument.Parse(xml, POIXMLDocumentPart.NamespaceManager);
			CT_Sst sst = _sstDoc.GetSst();
			count = sst.count;
			uniqueCount = sst.uniqueCount;
			foreach (CT_Rst item in sst.si)
			{
				string key = GetKey(item);
				if (key != null && !stmap.ContainsKey(key))
				{
					stmap.Add(key, num);
				}
				strings.Add(item);
				num++;
			}
		}
		catch (XmlException innerException)
		{
			throw new IOException("unable to parse shared strings table", innerException);
		}
	}

	private string GetKey(CT_Rst st)
	{
		return st.XmlText;
	}

	public CT_Rst GetEntryAt(int idx)
	{
		return strings[idx];
	}

	public int AddEntry(CT_Rst st)
	{
		string key = GetKey(st);
		count++;
		if (stmap.ContainsKey(key))
		{
			return stmap[key];
		}
		uniqueCount++;
		CT_Rst cT_Rst = new CT_Rst();
		_sstDoc.GetSst().si.Add(cT_Rst);
		cT_Rst.Set(st);
		int num = strings.Count;
		stmap[key] = num;
		strings.Add(cT_Rst);
		return num;
	}

	public void WriteTo(Stream out1)
	{
		CT_Sst sst = _sstDoc.GetSst();
		sst.count = count;
		sst.uniqueCount = uniqueCount;
		_sstDoc.Save(out1);
	}

	protected internal override void Commit()
	{
		Stream outputStream = GetPackagePart().GetOutputStream();
		WriteTo(outputStream);
		outputStream.Close();
	}
}
