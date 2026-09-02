using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using NPOI.OpenXml4Net.OPC;
using NPOI.OpenXmlFormats.Wordprocessing;
using NPOI.XWPF.Model;
using NPOI.XWPF.UserModel;

namespace NPOI.XWPF.Extractor;

public class XWPFWordExtractor : POIXMLTextExtractor
{
	public static XWPFRelation[] SUPPORTED_TYPES = new XWPFRelation[4]
	{
		XWPFRelation.DOCUMENT,
		XWPFRelation.TEMPLATE,
		XWPFRelation.MACRO_DOCUMENT,
		XWPFRelation.MACRO_TEMPLATE_DOCUMENT
	};

	private XWPFDocument document;

	private bool fetchHyperlinks;

	public override string Text
	{
		get
		{
			StringBuilder stringBuilder = new StringBuilder();
			XWPFHeaderFooterPolicy headerFooterPolicy = document.GetHeaderFooterPolicy();
			ExtractHeaders(stringBuilder, headerFooterPolicy);
			foreach (IBodyElement bodyElement in document.BodyElements)
			{
				AppendBodyElementText(stringBuilder, bodyElement);
				stringBuilder.Append('\n');
			}
			ExtractFooters(stringBuilder, headerFooterPolicy);
			return stringBuilder.ToString();
		}
	}

	public XWPFWordExtractor(OPCPackage Container)
		: this(new XWPFDocument(Container))
	{
	}

	public XWPFWordExtractor(XWPFDocument document)
		: base(document)
	{
		this.document = document;
	}

	public void SetFetchHyperlinks(bool fetch)
	{
		fetchHyperlinks = fetch;
	}

	public void AppendBodyElementText(StringBuilder text, IBodyElement e)
	{
		if (e is XWPFParagraph)
		{
			AppendParagraphText(text, (XWPFParagraph)e);
		}
		else if (e is XWPFTable)
		{
			AppendTableText(text, (XWPFTable)e);
		}
		else if (e is XWPFSDT)
		{
			text.Append(((XWPFSDT)e).Content.Text);
		}
	}

	public void AppendParagraphText(StringBuilder text, XWPFParagraph paragraph)
	{
		try
		{
			CT_SectPr cT_SectPr = null;
			if (paragraph.GetCTP().pPr != null)
			{
				cT_SectPr = paragraph.GetCTP().pPr.sectPr;
			}
			XWPFHeaderFooterPolicy hfPolicy = null;
			if (cT_SectPr != null)
			{
				hfPolicy = new XWPFHeaderFooterPolicy(document, cT_SectPr);
				ExtractHeaders(text, hfPolicy);
			}
			foreach (XWPFRun run in paragraph.Runs)
			{
				text.Append(run.ToString());
				if (run is XWPFHyperlinkRun && fetchHyperlinks)
				{
					XWPFHyperlink hyperlink = ((XWPFHyperlinkRun)run).GetHyperlink(document);
					if (hyperlink != null)
					{
						text.Append(" <" + hyperlink.URL + ">");
					}
				}
			}
			string commentText = new XWPFCommentsDecorator(paragraph, null).GetCommentText();
			if (commentText.Length > 0)
			{
				text.Append(commentText).Append('\n');
			}
			string footnoteText = paragraph.FootnoteText;
			if (footnoteText != null && footnoteText.Length > 0)
			{
				text.Append(footnoteText + "\n");
			}
			if (cT_SectPr != null)
			{
				ExtractFooters(text, hfPolicy);
			}
		}
		catch (IOException ex)
		{
			throw new POIXMLException(ex);
		}
		catch (XmlException ex2)
		{
			throw new POIXMLException(ex2);
		}
	}

	private void AppendTableText(StringBuilder text, XWPFTable table)
	{
		foreach (XWPFTableRow row in table.Rows)
		{
			List<ICell> tableICells = row.GetTableICells();
			for (int i = 0; i < tableICells.Count; i++)
			{
				ICell cell = tableICells[i];
				if (cell is XWPFTableCell)
				{
					text.Append(((XWPFTableCell)cell).GetTextRecursively());
				}
				else if (cell is XWPFSDTCell)
				{
					text.Append(((XWPFSDTCell)cell).Content.Text);
				}
				if (i < tableICells.Count - 1)
				{
					text.Append("\t");
				}
			}
			text.Append('\n');
		}
	}

	private void ExtractFooters(StringBuilder text, XWPFHeaderFooterPolicy hfPolicy)
	{
		if (hfPolicy != null)
		{
			if (hfPolicy.GetFirstPageFooter() != null)
			{
				text.Append(hfPolicy.GetFirstPageFooter().Text);
			}
			if (hfPolicy.GetEvenPageFooter() != null)
			{
				text.Append(hfPolicy.GetEvenPageFooter().Text);
			}
			if (hfPolicy.GetDefaultFooter() != null)
			{
				text.Append(hfPolicy.GetDefaultFooter().Text);
			}
		}
	}

	private void ExtractHeaders(StringBuilder text, XWPFHeaderFooterPolicy hfPolicy)
	{
		if (hfPolicy != null)
		{
			if (hfPolicy.GetFirstPageHeader() != null)
			{
				text.Append(hfPolicy.GetFirstPageHeader().Text);
			}
			if (hfPolicy.GetEvenPageHeader() != null)
			{
				text.Append(hfPolicy.GetEvenPageHeader().Text);
			}
			if (hfPolicy.GetDefaultHeader() != null)
			{
				text.Append(hfPolicy.GetDefaultHeader().Text);
			}
		}
	}
}
