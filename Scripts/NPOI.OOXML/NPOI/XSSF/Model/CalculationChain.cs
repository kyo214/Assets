using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using NPOI.OpenXml4Net.OPC;
using NPOI.OpenXmlFormats.Spreadsheet;

namespace NPOI.XSSF.Model;

public class CalculationChain : POIXMLDocumentPart
{
	private CT_CalcChain chain;

	public CalculationChain()
	{
		chain = new CT_CalcChain();
	}

	internal CalculationChain(PackagePart part)
		: base(part)
	{
		XmlDocument xml = POIXMLDocumentPart.ConvertStreamToXml(part.GetInputStream());
		ReadFrom(xml);
	}

	[Obsolete("deprecated in POI 3.14, scheduled for removal in POI 3.16")]
	public CalculationChain(PackagePart part, PackageRelationship rel)
		: this(part)
	{
	}

	public void ReadFrom(XmlDocument xml)
	{
		CalcChainDocument calcChainDocument = CalcChainDocument.Parse(xml, POIXMLDocumentPart.NamespaceManager);
		chain = calcChainDocument.GetCalcChain();
	}

	public void WriteTo(Stream out1)
	{
		CalcChainDocument calcChainDocument = new CalcChainDocument();
		calcChainDocument.SetCalcChain(chain);
		calcChainDocument.Save(out1);
	}

	protected internal override void Commit()
	{
		Stream outputStream = GetPackagePart().GetOutputStream();
		WriteTo(outputStream);
		outputStream.Close();
	}

	public CT_CalcChain GetCTCalcChain()
	{
		return chain;
	}

	public void RemoveItem(int sheetId, string ref1)
	{
		int num = -1;
		List<CT_CalcCell> c = chain.c;
		for (int i = 0; i < c.Count; i++)
		{
			if (c[i].iSpecified)
			{
				num = c[i].i;
			}
			if (num == sheetId && c[i].r.Equals(ref1))
			{
				if (c[i].iSpecified && i < c.Count - 1 && !c[i + 1].iSpecified)
				{
					c[i + 1].i = num;
					c[i + 1].iSpecified = true;
				}
				chain.RemoveC(i);
				break;
			}
		}
	}
}
