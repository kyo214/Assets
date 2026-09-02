using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using NPOI.OpenXml4Net.OPC;
using NPOI.OpenXmlFormats.Wordprocessing;
using NPOI.Util;

namespace NPOI.XWPF.UserModel;

public abstract class XWPFHeaderFooter : POIXMLDocumentPart, IBody
{
	protected List<XWPFParagraph> paragraphs = new List<XWPFParagraph>(1);

	protected List<XWPFTable> tables = new List<XWPFTable>(1);

	protected List<XWPFPictureData> pictures = new List<XWPFPictureData>();

	protected List<IBodyElement> bodyElements = new List<IBodyElement>(1);

	protected CT_HdrFtr headerFooter;

	protected XWPFDocument document;

	public IList<IBodyElement> BodyElements => bodyElements.AsReadOnly();

	public IList<XWPFParagraph> Paragraphs => paragraphs.AsReadOnly();

	public IList<XWPFTable> Tables => tables.AsReadOnly();

	public string Text
	{
		get
		{
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < paragraphs.Count; i++)
			{
				if (!paragraphs[i].IsEmpty)
				{
					string text = paragraphs[i].Text;
					if (text != null && text.Length > 0)
					{
						stringBuilder.Append(text);
						stringBuilder.Append('\n');
					}
				}
			}
			IList<XWPFTable> list = Tables;
			for (int j = 0; j < list.Count; j++)
			{
				string text2 = list[j].Text;
				if (text2 != null && text2.Length > 0)
				{
					stringBuilder.Append(text2);
					stringBuilder.Append('\n');
				}
			}
			foreach (IBodyElement bodyElement in BodyElements)
			{
				if (bodyElement is XWPFSDT)
				{
					stringBuilder.Append(((XWPFSDT)bodyElement).Content.Text + "\n");
				}
			}
			return stringBuilder.ToString();
		}
	}

	public IList<XWPFPictureData> AllPictures => pictures.AsReadOnly();

	public IList<XWPFPictureData> AllPackagePictures => document.AllPackagePictures;

	public POIXMLDocumentPart Owner => this;

	public POIXMLDocumentPart Part => this;

	public virtual BodyType PartType
	{
		get
		{
			throw new NotImplementedException();
		}
	}

	public XWPFHeaderFooter(XWPFDocument doc, CT_HdrFtr hdrFtr)
	{
		if (doc == null)
		{
			throw new NullReferenceException();
		}
		document = doc;
		headerFooter = hdrFtr;
		ReadHdrFtr();
	}

	protected XWPFHeaderFooter()
	{
		headerFooter = new CT_HdrFtr();
		ReadHdrFtr();
	}

	public XWPFHeaderFooter(POIXMLDocumentPart parent, PackagePart part)
		: base(parent, part)
	{
		document = (XWPFDocument)GetParent();
		if (document == null)
		{
			throw new NullReferenceException();
		}
	}

	[Obsolete("deprecated in POI 3.14, scheduled for removal in POI 3.16")]
	public XWPFHeaderFooter(POIXMLDocumentPart parent, PackagePart part, PackageRelationship rel)
		: this(parent, part)
	{
	}

	internal override void OnDocumentRead()
	{
		foreach (POIXMLDocumentPart relation in GetRelations())
		{
			if (relation is XWPFPictureData)
			{
				XWPFPictureData xWPFPictureData = (XWPFPictureData)relation;
				pictures.Add(xWPFPictureData);
				document.RegisterPackagePictureData(xWPFPictureData);
			}
		}
	}

	public CT_HdrFtr _getHdrFtr()
	{
		return headerFooter;
	}

	public void SetHeaderFooter(CT_HdrFtr headerFooter)
	{
		this.headerFooter = headerFooter;
		ReadHdrFtr();
	}

	public XWPFTable GetTable(CT_Tbl ctTable)
	{
		foreach (XWPFTable table in tables)
		{
			if (table == null)
			{
				return null;
			}
			if (table.GetCTTbl().Equals(ctTable))
			{
				return table;
			}
		}
		return null;
	}

	public XWPFParagraph GetParagraph(CT_P p)
	{
		foreach (XWPFParagraph paragraph in paragraphs)
		{
			if (paragraph.GetCTP().Equals(p))
			{
				return paragraph;
			}
		}
		return null;
	}

	public XWPFParagraph GetParagraphArray(int pos)
	{
		if (pos >= 0 && pos < paragraphs.Count)
		{
			return paragraphs[pos];
		}
		return null;
	}

	public List<XWPFParagraph> GetListParagraph()
	{
		return paragraphs;
	}

	public string AddPictureData(byte[] pictureData, int format)
	{
		XWPFPictureData xWPFPictureData = document.FindPackagePictureData(pictureData, format);
		POIXMLRelation descriptor = XWPFPictureData.RELATIONS[format];
		if (xWPFPictureData == null)
		{
			int nextPicNameNumber = document.GetNextPicNameNumber(format);
			xWPFPictureData = (XWPFPictureData)CreateRelationship(descriptor, XWPFFactory.GetInstance(), nextPicNameNumber);
			PackagePart packagePart = xWPFPictureData.GetPackagePart();
			Stream stream = null;
			try
			{
				stream = packagePart.GetOutputStream();
				stream.Write(pictureData, 0, pictureData.Length);
			}
			catch (IOException ex)
			{
				throw new POIXMLException(ex);
			}
			finally
			{
				try
				{
					stream?.Close();
				}
				catch (IOException)
				{
				}
			}
			document.RegisterPackagePictureData(xWPFPictureData);
			pictures.Add(xWPFPictureData);
			return GetRelationId(xWPFPictureData);
		}
		if (!GetRelations().Contains(xWPFPictureData))
		{
			xWPFPictureData.GetPackagePart();
			RelationPart relationPart = AddRelation(null, XWPFRelation.IMAGES, xWPFPictureData);
			pictures.Add(xWPFPictureData);
			return relationPart.Relationship.Id;
		}
		return GetRelationId(xWPFPictureData);
	}

	public string AddPictureData(Stream is1, int format)
	{
		byte[] pictureData = IOUtils.ToByteArray(is1);
		return AddPictureData(pictureData, format);
	}

	public XWPFPictureData GetPictureDataByID(string blipID)
	{
		POIXMLDocumentPart relationById = GetRelationById(blipID);
		if (relationById != null && relationById is XWPFPictureData)
		{
			return (XWPFPictureData)relationById;
		}
		return null;
	}

	public XWPFTable GetTableArray(int pos)
	{
		if (pos >= 0 && pos < tables.Count)
		{
			return tables[pos];
		}
		return null;
	}

	public void InsertTable(int pos, XWPFTable table)
	{
		bodyElements.Insert(pos, table);
		int i;
		for (i = 0; i < headerFooter.GetTblList().Count && headerFooter.GetTblArray(i) != table.GetCTTbl(); i++)
		{
		}
		tables.Insert(i, table);
	}

	public void ReadHdrFtr()
	{
		bodyElements = new List<IBodyElement>();
		paragraphs = new List<XWPFParagraph>();
		tables = new List<XWPFTable>();
		foreach (object item3 in headerFooter.Items)
		{
			if (item3 is CT_P)
			{
				XWPFParagraph item = new XWPFParagraph((CT_P)item3, this);
				paragraphs.Add(item);
				bodyElements.Add(item);
			}
			if (item3 is CT_Tbl)
			{
				XWPFTable item2 = new XWPFTable((CT_Tbl)item3, this);
				tables.Add(item2);
				bodyElements.Add(item2);
			}
		}
	}

	public XWPFTableCell GetTableCell(CT_Tc cell)
	{
		throw new NotImplementedException();
	}

	public void SetXWPFDocument(XWPFDocument doc)
	{
		document = doc;
	}

	public XWPFDocument GetXWPFDocument()
	{
		if (document != null)
		{
			return document;
		}
		return (XWPFDocument)GetParent();
	}

	public XWPFParagraph CreateParagraph()
	{
		XWPFParagraph xWPFParagraph = new XWPFParagraph(headerFooter.AddNewP(), this);
		paragraphs.Add(xWPFParagraph);
		return xWPFParagraph;
	}

	public XWPFParagraph InsertNewParagraph(XmlDocument cursor)
	{
		throw new NotImplementedException();
	}

	public XWPFTable InsertNewTbl(XmlDocument cursor)
	{
		throw new NotImplementedException();
	}
}
