using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using NPOI.OpenXml4Net.OPC;
using NPOI.OpenXmlFormats.Spreadsheet;
using NPOI.XSSF.UserModel;
using NPOI.XSSF.UserModel.Helpers;

namespace NPOI.XSSF.Model;

public class SingleXmlCells : POIXMLDocumentPart
{
	private CT_SingleXmlCells SingleXMLCells;

	public SingleXmlCells()
	{
		SingleXMLCells = new CT_SingleXmlCells();
	}

	public SingleXmlCells(PackagePart part)
		: base(part)
	{
		ReadFrom(part.GetInputStream());
	}

	[Obsolete("deprecated in POI 3.14, scheduled for removal in POI 3.16")]
	public SingleXmlCells(PackagePart part, PackageRelationship rel)
		: this(part)
	{
	}

	public void ReadFrom(Stream is1)
	{
		try
		{
			SingleXmlCellsDocument singleXmlCellsDocument = SingleXmlCellsDocument.Parse(is1);
			SingleXMLCells = singleXmlCellsDocument.GetSingleXmlCells();
		}
		catch (XmlException ex)
		{
			throw new IOException(ex.Message);
		}
	}

	public XSSFSheet GetXSSFSheet()
	{
		return (XSSFSheet)GetParent();
	}

	protected void WriteTo(Stream out1)
	{
		SingleXmlCellsDocument singleXmlCellsDocument = new SingleXmlCellsDocument();
		singleXmlCellsDocument.SetSingleXmlCells(SingleXMLCells);
		singleXmlCellsDocument.Save(out1);
	}

	protected internal override void Commit()
	{
		Stream outputStream = GetPackagePart().GetOutputStream();
		WriteTo(outputStream);
		outputStream.Close();
	}

	public CT_SingleXmlCells GetCTSingleXMLCells()
	{
		return SingleXMLCells;
	}

	public List<XSSFSingleXmlCell> GetAllSimpleXmlCell()
	{
		List<XSSFSingleXmlCell> list = new List<XSSFSingleXmlCell>();
		foreach (CT_SingleXmlCell item in SingleXMLCells.singleXmlCell)
		{
			list.Add(new XSSFSingleXmlCell(item, this));
		}
		return list;
	}
}
