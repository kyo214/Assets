using System.IO;
using System.Xml;

namespace NPOI.OpenXmlFormats.Wordprocessing;

public class CommentsDocument
{
	private CT_Comments comments;

	public CT_Comments Comments => comments;

	public CommentsDocument()
	{
		comments = new CT_Comments();
	}

	public static CommentsDocument Parse(XmlDocument doc, XmlNamespaceManager NameSpaceManager)
	{
		return new CommentsDocument(CT_Comments.Parse(doc.DocumentElement, NameSpaceManager));
	}

	public CommentsDocument(CT_Comments comments)
	{
		this.comments = comments;
	}

	public void Save(Stream stream)
	{
		using StreamWriter sw = new StreamWriter(stream);
		comments.Write(sw);
	}
}
