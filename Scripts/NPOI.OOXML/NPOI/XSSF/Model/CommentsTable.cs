using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using NPOI.OpenXml4Net.OPC;
using NPOI.OpenXmlFormats.Spreadsheet;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using NPOI.XSSF.UserModel;

namespace NPOI.XSSF.Model;

public class CommentsTable : POIXMLDocumentPart
{
	public static string DEFAULT_AUTHOR = "";

	public static int DEFAULT_AUTHOR_ID = 0;

	private CT_Comments comments;

	private Dictionary<CellAddress, CT_Comment> commentRefs;

	public CommentsTable()
	{
		comments = new CT_Comments();
		comments.AddNewCommentList();
		comments.AddNewAuthors().AddAuthor(DEFAULT_AUTHOR);
	}

	internal CommentsTable(PackagePart part)
		: base(part)
	{
		ReadFrom(part.GetInputStream());
	}

	[Obsolete("deprecated in POI 3.14, scheduled for removal in POI 3.16")]
	public CommentsTable(PackagePart part, PackageRelationship rel)
		: this(part)
	{
	}

	public void ReadFrom(Stream is1)
	{
		try
		{
			CommentsDocument commentsDocument = CommentsDocument.Parse(POIXMLDocumentPart.ConvertStreamToXml(is1), POIXMLDocumentPart.NamespaceManager);
			comments = commentsDocument.GetComments();
		}
		catch (XmlException ex)
		{
			throw new IOException(ex.Message);
		}
	}

	public void WriteTo(Stream out1)
	{
		CommentsDocument commentsDocument = new CommentsDocument();
		commentsDocument.SetComments(comments);
		commentsDocument.Save(out1);
	}

	protected internal override void Commit()
	{
		Stream outputStream = GetPackagePart().GetOutputStream();
		WriteTo(outputStream);
		outputStream.Close();
	}

	[Obsolete("2015-11-23 (circa POI 3.14beta1). Use {@link #referenceUpdated(CellAddress, CTComment)} instead")]
	public void ReferenceUpdated(string oldReference, CT_Comment comment)
	{
		ReferenceUpdated(new CellAddress(oldReference), comment);
	}

	public void ReferenceUpdated(CellAddress oldReference, CT_Comment comment)
	{
		if (commentRefs != null)
		{
			commentRefs.Remove(oldReference);
			commentRefs[new CellAddress(comment.@ref)] = comment;
		}
	}

	public int GetNumberOfComments()
	{
		return comments.commentList.SizeOfCommentArray();
	}

	public int GetNumberOfAuthors()
	{
		return comments.authors.SizeOfAuthorArray();
	}

	public string GetAuthor(long authorId)
	{
		return comments.authors.GetAuthorArray((int)authorId);
	}

	public int FindAuthor(string author)
	{
		for (int i = 0; i < comments.authors.SizeOfAuthorArray(); i++)
		{
			if (comments.authors.GetAuthorArray(i).Equals(author))
			{
				return i;
			}
		}
		return AddNewAuthor(author);
	}

	[Obsolete("deprecated 2015-11-23 (circa POI 3.14beta1). Use {@link #findCellComment(CellAddress)} instead")]
	public XSSFComment FindCellComment(string cellRef)
	{
		return FindCellComment(new CellAddress(cellRef));
	}

	public XSSFComment FindCellComment(CellAddress cellAddress)
	{
		CT_Comment cTComment = GetCTComment(cellAddress);
		if (cTComment != null)
		{
			return new XSSFComment(this, cTComment, null);
		}
		return null;
	}

	[Obsolete("deprecated 2015-11-23 (circa POI 3.14beta1). Use {@link CommentsTable#getCTComment(CellAddress)} instead")]
	public CT_Comment GetCTComment(string ref1)
	{
		return GetCTComment(new CellAddress(ref1));
	}

	public CT_Comment GetCTComment(CellAddress cellRef)
	{
		PrepareCTCommentCache();
		if (commentRefs.ContainsKey(cellRef))
		{
			return commentRefs[cellRef];
		}
		return null;
	}

	public Dictionary<CellAddress, IComment> GetCellComments()
	{
		PrepareCTCommentCache();
		Dictionary<CellAddress, IComment> dictionary = new Dictionary<CellAddress, IComment>();
		foreach (KeyValuePair<CellAddress, CT_Comment> commentRef in commentRefs)
		{
			dictionary.Add(commentRef.Key, new XSSFComment(this, commentRef.Value, null));
		}
		return dictionary;
	}

	private void PrepareCTCommentCache()
	{
		if (commentRefs == null)
		{
			commentRefs = new Dictionary<CellAddress, CT_Comment>();
			CT_Comment[] commentArray = comments.commentList.GetCommentArray();
			foreach (CT_Comment cT_Comment in commentArray)
			{
				commentRefs.Add(new CellAddress(cT_Comment.@ref), cT_Comment);
			}
		}
	}

	[Obsolete("deprecated 2015-11-23 (circa POI 3.14beta1). Use {@link #newComment(CellAddress)} instead")]
	public CT_Comment NewComment(string ref1)
	{
		return NewComment(new CellAddress(ref1));
	}

	public CT_Comment NewComment(CellAddress ref1)
	{
		CT_Comment cT_Comment = comments.commentList.AddNewComment();
		cT_Comment.@ref = ref1.FormatAsString();
		cT_Comment.authorId = (uint)DEFAULT_AUTHOR_ID;
		if (commentRefs != null)
		{
			commentRefs.Add(ref1, cT_Comment);
		}
		return cT_Comment;
	}

	[Obsolete("deprecated 2015-11-23 (circa POI 3.14beta1). Use {@link #removeComment(CellAddress)} instead")]
	public bool RemoveComment(string cellRef)
	{
		return RemoveComment(new CellAddress(cellRef));
	}

	public bool RemoveComment(CellAddress cellRef)
	{
		string text = cellRef.FormatAsString();
		CT_CommentList commentList = comments.commentList;
		if (commentList != null)
		{
			CT_Comment[] commentArray = commentList.GetCommentArray();
			for (int i = 0; i < commentArray.Length; i++)
			{
				CT_Comment cT_Comment = commentArray[i];
				if (text.Equals(cT_Comment.@ref))
				{
					commentList.RemoveComment(i);
					if (commentRefs != null)
					{
						commentRefs.Remove(cellRef);
					}
					return true;
				}
			}
		}
		return false;
	}

	private int AddNewAuthor(string author)
	{
		int num = comments.authors.SizeOfAuthorArray();
		comments.authors.Insert(num, author);
		return num;
	}

	public CT_Comments GetCTComments()
	{
		return comments;
	}
}
