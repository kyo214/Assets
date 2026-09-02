using System.Collections.Generic;
using System.Text;
using NPOI.OpenXmlFormats.Wordprocessing;

namespace NPOI.XWPF.UserModel;

public class XWPFSDTContent : ISDTContent
{
	private List<XWPFParagraph> paragraphs = new List<XWPFParagraph>();

	private List<XWPFTable> tables = new List<XWPFTable>();

	private List<XWPFRun> runs = new List<XWPFRun>();

	private List<XWPFSDT> contentControls = new List<XWPFSDT>();

	private List<ISDTContents> bodyElements = new List<ISDTContents>();

	public string Text
	{
		get
		{
			StringBuilder stringBuilder = new StringBuilder();
			bool flag = false;
			for (int i = 0; i < bodyElements.Count; i++)
			{
				object obj = bodyElements[i];
				if (obj is XWPFParagraph)
				{
					AppendParagraph((XWPFParagraph)obj, stringBuilder);
					flag = true;
				}
				else if (obj is XWPFTable)
				{
					AppendTable((XWPFTable)obj, stringBuilder);
					flag = true;
				}
				else if (obj is XWPFSDT)
				{
					stringBuilder.Append(((XWPFSDT)obj).Content.Text);
					flag = true;
				}
				else if (obj is XWPFRun)
				{
					stringBuilder.Append(((XWPFRun)obj).ToString());
					flag = false;
				}
				if (flag && i < bodyElements.Count - 1)
				{
					stringBuilder.Append("\n");
				}
			}
			return stringBuilder.ToString();
		}
	}

	public XWPFSDTContent(CT_SdtContentRun sdtRun, IBody part, IRunBody parent)
	{
		foreach (CT_R r in sdtRun.GetRList())
		{
			XWPFRun item = new XWPFRun(r, parent);
			runs.Add(item);
			bodyElements.Add(item);
		}
	}

	public XWPFSDTContent(CT_SdtContentBlock block, IBody part, IRunBody parent)
	{
		foreach (object item5 in block.Items)
		{
			if (item5 is CT_P)
			{
				XWPFParagraph item = new XWPFParagraph((CT_P)item5, part);
				bodyElements.Add(item);
				paragraphs.Add(item);
			}
			else if (item5 is CT_Tbl)
			{
				XWPFTable item2 = new XWPFTable((CT_Tbl)item5, part);
				bodyElements.Add(item2);
				tables.Add(item2);
			}
			else if (item5 is CT_SdtBlock)
			{
				XWPFSDT item3 = new XWPFSDT((CT_SdtBlock)item5, part);
				bodyElements.Add(item3);
				contentControls.Add(item3);
			}
			else if (item5 is CT_R)
			{
				XWPFRun item4 = new XWPFRun((CT_R)item5, parent);
				runs.Add(item4);
				bodyElements.Add(item4);
			}
		}
	}

	private void AppendTable(XWPFTable table, StringBuilder text)
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

	private void AppendParagraph(XWPFParagraph paragraph, StringBuilder text)
	{
		foreach (XWPFRun run in paragraph.Runs)
		{
			text.Append(run.ToString());
		}
	}

	public override string ToString()
	{
		return Text;
	}
}
