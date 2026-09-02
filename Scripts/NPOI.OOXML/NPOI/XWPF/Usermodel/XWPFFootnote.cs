using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using NPOI.OpenXmlFormats.Wordprocessing;

namespace NPOI.XWPF.UserModel;

public class XWPFFootnote : IEnumerator<XWPFParagraph>, IDisposable, IEnumerator, IBody
{
	private List<XWPFParagraph> paragraphs = new List<XWPFParagraph>();

	private List<XWPFTable> tables = new List<XWPFTable>();

	private List<XWPFPictureData> pictures = new List<XWPFPictureData>();

	private List<IBodyElement> bodyElements = new List<IBodyElement>();

	private CT_FtnEdn ctFtnEdn;

	private XWPFFootnotes footnotes;

	private XWPFDocument document;

	public IList<XWPFParagraph> Paragraphs => paragraphs;

	public IList<XWPFTable> Tables => tables;

	public IList<XWPFPictureData> Pictures => pictures;

	public IList<IBodyElement> BodyElements => bodyElements;

	public POIXMLDocumentPart Owner => footnotes;

	public POIXMLDocumentPart Part => footnotes;

	public BodyType PartType => BodyType.FOOTNOTE;

	public XWPFParagraph Current
	{
		get
		{
			throw new NotImplementedException();
		}
	}

	object IEnumerator.Current
	{
		get
		{
			throw new NotImplementedException();
		}
	}

	public XWPFFootnote(CT_FtnEdn note, XWPFFootnotes xFootnotes)
	{
		footnotes = xFootnotes;
		ctFtnEdn = note;
		document = xFootnotes.GetXWPFDocument();
		Init();
	}

	public XWPFFootnote(XWPFDocument document, CT_FtnEdn body)
	{
		ctFtnEdn = body;
		this.document = document;
		Init();
	}

	private void Init()
	{
		foreach (object item4 in ctFtnEdn.Items)
		{
			if (item4 is CT_P)
			{
				XWPFParagraph item = new XWPFParagraph((CT_P)item4, this);
				bodyElements.Add(item);
				paragraphs.Add(item);
			}
			else if (item4 is CT_Tbl)
			{
				XWPFTable item2 = new XWPFTable((CT_Tbl)item4, this);
				bodyElements.Add(item2);
				tables.Add(item2);
			}
			else if (item4 is CT_SdtBlock)
			{
				XWPFSDT item3 = new XWPFSDT((CT_SdtBlock)item4, this);
				bodyElements.Add(item3);
			}
		}
	}

	public IEnumerator<XWPFParagraph> GetEnumerator()
	{
		return paragraphs.GetEnumerator();
	}

	public CT_FtnEdn GetCTFtnEdn()
	{
		return ctFtnEdn;
	}

	public void SetCTFtnEdn(CT_FtnEdn footnote)
	{
		ctFtnEdn = footnote;
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
		for (i = 0; i < ctFtnEdn.GetTblList().Count && ctFtnEdn.GetTblArray(i) != table.GetCTTbl(); i++)
		{
		}
		tables.Insert(i, table);
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

	public XWPFTableCell GetTableCell(CT_Tc cell)
	{
		object parent = cell.Parent;
		if (!(parent is CT_Row))
		{
			return null;
		}
		CT_Row cT_Row = (CT_Row)parent;
		if (!(cT_Row.Parent is CT_Tbl))
		{
			return null;
		}
		CT_Tbl ctTable = (CT_Tbl)cT_Row.Parent;
		return GetTable(ctTable)?.GetRow(cT_Row)?.GetTableCell(cell);
	}

	private bool IsCursorInFtn(XmlDocument cursor)
	{
		throw new NotImplementedException();
	}

	public XWPFTable InsertNewTbl(XmlDocument cursor)
	{
		throw new NotImplementedException();
	}

	public XWPFParagraph InsertNewParagraph(XmlDocument cursor)
	{
		throw new NotImplementedException();
	}

	public XWPFTable AddNewTbl(CT_Tbl table)
	{
		CT_Tbl cT_Tbl = ctFtnEdn.AddNewTbl();
		cT_Tbl.Set(table);
		XWPFTable xWPFTable = new XWPFTable(cT_Tbl, this);
		tables.Add(xWPFTable);
		return xWPFTable;
	}

	public XWPFParagraph AddNewParagraph(CT_P paragraph)
	{
		XWPFParagraph xWPFParagraph = new XWPFParagraph(ctFtnEdn.AddNewP(paragraph), this);
		paragraphs.Add(xWPFParagraph);
		return xWPFParagraph;
	}

	public XWPFDocument GetXWPFDocument()
	{
		return document;
	}

	public void Dispose()
	{
		throw new NotImplementedException();
	}

	public bool MoveNext()
	{
		throw new NotImplementedException();
	}

	public void Reset()
	{
		throw new NotImplementedException();
	}
}
