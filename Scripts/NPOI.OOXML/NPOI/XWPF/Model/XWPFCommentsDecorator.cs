using System.Text;
using NPOI.OpenXmlFormats.Wordprocessing;
using NPOI.XWPF.UserModel;

namespace NPOI.XWPF.Model;

public class XWPFCommentsDecorator : XWPFParagraphDecorator
{
	private StringBuilder commentText;

	public override string Text => base.Text + commentText;

	public XWPFCommentsDecorator(XWPFParagraphDecorator nextDecorator)
		: this(nextDecorator.paragraph, nextDecorator)
	{
	}

	public XWPFCommentsDecorator(XWPFParagraph paragraph, XWPFParagraphDecorator nextDecorator)
		: base(paragraph, nextDecorator)
	{
		commentText = new StringBuilder();
		foreach (CT_MarkupRange commentRangeStart in paragraph.GetCTP().GetCommentRangeStartList())
		{
			XWPFComment commentByID;
			if ((commentByID = paragraph.Document.GetCommentByID(commentRangeStart.id)) != null)
			{
				commentText.Append("\tComment by " + commentByID.Author + ": " + commentByID.Text);
			}
		}
	}

	public string GetCommentText()
	{
		return commentText.ToString();
	}
}
