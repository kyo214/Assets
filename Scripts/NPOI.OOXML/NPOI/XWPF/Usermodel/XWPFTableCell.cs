using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using NPOI.OpenXmlFormats.Wordprocessing;

namespace NPOI.XWPF.UserModel;

public class XWPFTableCell : IBody, ICell
{
	public enum XWPFVertAlign
	{
		TOP = 0,
		CENTER = 1,
		BOTH = 2,
		BOTTOM = 3
	}

	private CT_Tc ctTc;

	protected List<XWPFParagraph> paragraphs;

	protected List<XWPFTable> tables;

	protected List<IBodyElement> bodyElements;

	protected IBody part;

	private XWPFTableRow tableRow;

	private static Dictionary<XWPFVertAlign, ST_VerticalJc> alignMap;

	private static Dictionary<ST_VerticalJc, XWPFVertAlign> stVertAlignTypeMap;

	public IList<IBodyElement> BodyElements => bodyElements.AsReadOnly();

	public IList<XWPFParagraph> Paragraphs => paragraphs;

	public POIXMLDocumentPart Part => tableRow.GetTable().Part;

	public BodyType PartType => BodyType.TABLECELL;

	public IList<XWPFTable> Tables => tables.AsReadOnly();

	static XWPFTableCell()
	{
		alignMap = new Dictionary<XWPFVertAlign, ST_VerticalJc>();
		alignMap.Add(XWPFVertAlign.TOP, ST_VerticalJc.top);
		alignMap.Add(XWPFVertAlign.CENTER, ST_VerticalJc.center);
		alignMap.Add(XWPFVertAlign.BOTH, ST_VerticalJc.both);
		alignMap.Add(XWPFVertAlign.BOTTOM, ST_VerticalJc.bottom);
		stVertAlignTypeMap = new Dictionary<ST_VerticalJc, XWPFVertAlign>();
		stVertAlignTypeMap.Add(ST_VerticalJc.top, XWPFVertAlign.TOP);
		stVertAlignTypeMap.Add(ST_VerticalJc.center, XWPFVertAlign.CENTER);
		stVertAlignTypeMap.Add(ST_VerticalJc.both, XWPFVertAlign.BOTH);
		stVertAlignTypeMap.Add(ST_VerticalJc.bottom, XWPFVertAlign.BOTTOM);
	}

	public XWPFTableCell(CT_Tc cell, XWPFTableRow tableRow, IBody part)
	{
		ctTc = cell;
		this.part = part;
		this.tableRow = tableRow;
		if (cell.GetPList().Count < 1)
		{
			cell.AddNewP();
		}
		bodyElements = new List<IBodyElement>();
		paragraphs = new List<XWPFParagraph>();
		tables = new List<XWPFTable>();
		foreach (object item5 in ctTc.Items)
		{
			if (item5 is CT_P)
			{
				XWPFParagraph item = new XWPFParagraph((CT_P)item5, this);
				paragraphs.Add(item);
				bodyElements.Add(item);
			}
			if (item5 is CT_Tbl)
			{
				XWPFTable item2 = new XWPFTable((CT_Tbl)item5, this);
				tables.Add(item2);
				bodyElements.Add(item2);
			}
			if (item5 is CT_SdtBlock)
			{
				XWPFSDT item3 = new XWPFSDT((CT_SdtBlock)item5, this);
				bodyElements.Add(item3);
			}
			if (item5 is CT_SdtRun)
			{
				XWPFSDT item4 = new XWPFSDT((CT_SdtRun)item5, this);
				bodyElements.Add(item4);
			}
		}
	}

	public CT_Tc GetCTTc()
	{
		return ctTc;
	}

	public void SetParagraph(XWPFParagraph p)
	{
		if (ctTc.SizeOfPArray() == 0)
		{
			ctTc.AddNewP();
		}
		ctTc.SetPArray(0, p.GetCTP());
	}

	public XWPFParagraph AddParagraph()
	{
		XWPFParagraph xWPFParagraph = new XWPFParagraph(ctTc.AddNewP(), this);
		AddParagraph(xWPFParagraph);
		return xWPFParagraph;
	}

	public void AddParagraph(XWPFParagraph p)
	{
		paragraphs.Add(p);
	}

	public void RemoveParagraph(int pos)
	{
		paragraphs.RemoveAt(pos);
		ctTc.RemoveP(pos);
	}

	public XWPFParagraph GetParagraph(CT_P p)
	{
		foreach (XWPFParagraph paragraph in paragraphs)
		{
			if (p.Equals(paragraph.GetCTP()))
			{
				return paragraph;
			}
		}
		return null;
	}

	public void SetBorderBottom(XWPFTable.XWPFBorderType type, int size, int space, string rgbColor)
	{
		CT_TcPr cT_TcPr = (GetCTTc().IsSetTcPr() ? GetCTTc().tcPr : GetCTTc().AddNewTcPr());
		((cT_TcPr.tcBorders == null) ? cT_TcPr.AddNewTcBorders() : cT_TcPr.tcBorders).bottom = CreateBorder(type, size, space, rgbColor);
	}

	public void SetBorderTop(XWPFTable.XWPFBorderType type, int size, int space, string rgbColor)
	{
		CT_TcPr cT_TcPr = (GetCTTc().IsSetTcPr() ? GetCTTc().tcPr : GetCTTc().AddNewTcPr());
		((cT_TcPr.tcBorders == null) ? cT_TcPr.AddNewTcBorders() : cT_TcPr.tcBorders).top = CreateBorder(type, size, space, rgbColor);
	}

	public void SetBorderLeft(XWPFTable.XWPFBorderType type, int size, int space, string rgbColor)
	{
		CT_TcPr cT_TcPr = (GetCTTc().IsSetTcPr() ? GetCTTc().tcPr : GetCTTc().AddNewTcPr());
		((cT_TcPr.tcBorders == null) ? cT_TcPr.AddNewTcBorders() : cT_TcPr.tcBorders).left = CreateBorder(type, size, space, rgbColor);
	}

	public void SetBorderRight(XWPFTable.XWPFBorderType type, int size, int space, string rgbColor)
	{
		CT_TcPr cT_TcPr = (GetCTTc().IsSetTcPr() ? GetCTTc().tcPr : GetCTTc().AddNewTcPr());
		((cT_TcPr.tcBorders == null) ? cT_TcPr.AddNewTcBorders() : cT_TcPr.tcBorders).right = CreateBorder(type, size, space, rgbColor);
	}

	private static CT_Border CreateBorder(XWPFTable.XWPFBorderType type, int size, int space, string rgbColor)
	{
		return new CT_Border
		{
			val = XWPFTable.xwpfBorderTypeMap[type],
			sz = (ulong)size,
			space = (ulong)space,
			color = rgbColor
		};
	}

	public void SetText(string text)
	{
		new XWPFParagraph((ctTc.SizeOfPArray() == 0) ? ctTc.AddNewP() : ctTc.GetPArray(0), this).CreateRun().AppendText(text);
	}

	public XWPFTableRow GetTableRow()
	{
		return tableRow;
	}

	public void SetColor(string rgbStr)
	{
		CT_TcPr cT_TcPr = (ctTc.IsSetTcPr() ? ctTc.tcPr : ctTc.AddNewTcPr());
		CT_Shd obj = (cT_TcPr.IsSetShd() ? cT_TcPr.shd : cT_TcPr.AddNewShd());
		obj.color = "auto";
		obj.val = ST_Shd.clear;
		obj.fill = rgbStr;
	}

	public string GetColor()
	{
		string result = null;
		CT_TcPr tcPr = ctTc.tcPr;
		if (tcPr != null)
		{
			CT_Shd shd = tcPr.shd;
			if (shd != null)
			{
				result = shd.fill;
			}
		}
		return result;
	}

	public void SetVerticalAlignment(XWPFVertAlign vAlign)
	{
		(ctTc.IsSetTcPr() ? ctTc.tcPr : ctTc.AddNewTcPr()).AddNewVAlign().val = alignMap[vAlign];
	}

	public XWPFVertAlign? GetVerticalAlignment()
	{
		XWPFVertAlign? result = null;
		CT_TcPr tcPr = ctTc.tcPr;
		if (tcPr != null)
		{
			CT_VerticalJc vAlign = tcPr.vAlign;
			result = ((vAlign == null) ? new XWPFVertAlign?(XWPFVertAlign.TOP) : new XWPFVertAlign?(stVertAlignTypeMap[vAlign.val.Value]));
			if (vAlign != null && vAlign.val.HasValue)
			{
				result = stVertAlignTypeMap[vAlign.val.Value];
			}
		}
		return result;
	}

	public XWPFParagraph InsertNewParagraph(XmlDocument cursor)
	{
		throw new NotImplementedException();
	}

	public XWPFTable InsertNewTbl(XmlDocument cursor)
	{
		throw new NotImplementedException();
	}

	private bool IsCursorInTableCell(XmlDocument cursor)
	{
		throw new NotImplementedException();
	}

	public XWPFParagraph GetParagraphArray(int pos)
	{
		if (pos >= 0 && pos < paragraphs.Count)
		{
			return paragraphs[pos];
		}
		return null;
	}

	public XWPFTable GetTable(CT_Tbl ctTable)
	{
		for (int i = 0; i < tables.Count; i++)
		{
			if (Tables[i].GetCTTbl() == ctTable)
			{
				return Tables[i];
			}
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
		for (i = 0; i < ctTc.GetTblList().Count && ctTc.GetTblArray(i) != table.GetCTTbl(); i++)
		{
		}
		tables.Insert(i, table);
	}

	public string GetText()
	{
		StringBuilder stringBuilder = new StringBuilder();
		foreach (XWPFParagraph paragraph in paragraphs)
		{
			stringBuilder.Append(paragraph.Text);
		}
		return stringBuilder.ToString();
	}

	public string GetTextRecursively()
	{
		StringBuilder stringBuilder = new StringBuilder();
		for (int i = 0; i < bodyElements.Count; i++)
		{
			bool isLast = ((i == bodyElements.Count - 1) ? true : false);
			AppendBodyElementText(stringBuilder, bodyElements[i], isLast);
		}
		return stringBuilder.ToString();
	}

	private void AppendBodyElementText(StringBuilder text, IBodyElement e, bool isLast)
	{
		if (e is XWPFParagraph)
		{
			text.Append(((XWPFParagraph)e).Text);
			if (!isLast)
			{
				text.Append('\t');
			}
		}
		else if (e is XWPFTable)
		{
			foreach (XWPFTableRow row in ((XWPFTable)e).Rows)
			{
				foreach (XWPFTableCell tableCell in row.GetTableCells())
				{
					IList<IBodyElement> list = tableCell.BodyElements;
					for (int i = 0; i < list.Count; i++)
					{
						bool isLast2 = ((i == list.Count - 1) ? true : false);
						AppendBodyElementText(text, list[i], isLast2);
					}
				}
			}
			if (!isLast)
			{
				text.Append('\n');
			}
		}
		else if (e is XWPFSDT)
		{
			text.Append(((XWPFSDT)e).Content.Text);
			if (!isLast)
			{
				text.Append('\t');
			}
		}
	}

	public XWPFTableCell GetTableCell(CT_Tc cell)
	{
		if (!(cell.Parent is CT_Row))
		{
			return null;
		}
		CT_Row cT_Row = (CT_Row)cell.Parent;
		if (!(cT_Row.Parent is CT_Tbl))
		{
			return null;
		}
		CT_Tbl ctTable = (CT_Tbl)cT_Row.Parent;
		return GetTable(ctTable)?.GetRow(cT_Row)?.GetTableCell(cell);
	}

	public XWPFDocument GetXWPFDocument()
	{
		return part.GetXWPFDocument();
	}
}
