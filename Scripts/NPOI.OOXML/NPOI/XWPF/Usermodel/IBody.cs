using System.Collections.Generic;
using System.Xml;
using NPOI.OpenXmlFormats.Wordprocessing;

namespace NPOI.XWPF.UserModel;

public interface IBody
{
	POIXMLDocumentPart Part { get; }

	BodyType PartType { get; }

	IList<IBodyElement> BodyElements { get; }

	IList<XWPFParagraph> Paragraphs { get; }

	IList<XWPFTable> Tables { get; }

	XWPFParagraph GetParagraph(CT_P p);

	XWPFTable GetTable(CT_Tbl ctTable);

	XWPFParagraph GetParagraphArray(int pos);

	XWPFTable GetTableArray(int pos);

	XWPFParagraph InsertNewParagraph(XmlDocument cursor);

	XWPFTable InsertNewTbl(XmlDocument cursor);

	void InsertTable(int pos, XWPFTable table);

	XWPFTableCell GetTableCell(CT_Tc cell);

	XWPFDocument GetXWPFDocument();
}
