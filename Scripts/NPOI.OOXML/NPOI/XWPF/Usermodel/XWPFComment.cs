using System.Text;
using NPOI.OpenXmlFormats.Wordprocessing;

namespace NPOI.XWPF.UserModel;

public class XWPFComment
{
	protected string id;

	protected string author;

	protected StringBuilder text;

	public string Id => id;

	public string Author => author;

	public string Text => text.ToString();

	public XWPFComment(CT_Comment comment, XWPFDocument document)
	{
		text = new StringBuilder();
		id = comment.id.ToString();
		author = comment.author;
		foreach (CT_P p in comment.GetPList())
		{
			XWPFParagraph xWPFParagraph = new XWPFParagraph(p, document);
			text.Append(xWPFParagraph.Text);
		}
	}
}
