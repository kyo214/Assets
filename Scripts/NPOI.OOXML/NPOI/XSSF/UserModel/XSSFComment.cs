using System;
using NPOI.OpenXmlFormats.Spreadsheet;
using NPOI.OpenXmlFormats.Vml;
using NPOI.OpenXmlFormats.Vml.Spreadsheet;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using NPOI.Util;
using NPOI.XSSF.Model;

namespace NPOI.XSSF.UserModel;

public class XSSFComment : IComment
{
	private CT_Comment _comment;

	private CommentsTable _comments;

	private CT_Shape _vmlShape;

	private XSSFRichTextString _str;

	public string Author
	{
		get
		{
			return _comments.GetAuthor((int)_comment.authorId);
		}
		set
		{
			_comment.authorId = (uint)_comments.FindAuthor(value);
		}
	}

	public CellAddress Address
	{
		get
		{
			return new CellAddress(_comment.@ref);
		}
		set
		{
			CellAddress cellAddress = new CellAddress(_comment.@ref);
			if (!value.Equals(cellAddress))
			{
				_comment.@ref = value.FormatAsString();
				_comments.ReferenceUpdated(cellAddress, _comment);
				if (_vmlShape != null)
				{
					CT_ClientData clientDataArray = _vmlShape.GetClientDataArray(0);
					clientDataArray.SetRowArray(0, value.Row);
					clientDataArray.SetColumnArray(0, value.Column);
				}
			}
		}
	}

	public int Column
	{
		get
		{
			return Address.Column;
		}
		set
		{
			SetAddress(Row, value);
		}
	}

	public int Row
	{
		get
		{
			return Address.Row;
		}
		set
		{
			SetAddress(value, Column);
		}
	}

	public bool Visible
	{
		get
		{
			bool result = false;
			if (_vmlShape != null)
			{
				string style = _vmlShape.style;
				if (style != null)
				{
					result = style.IndexOf("visibility:visible") != -1;
				}
				else
				{
					if (_vmlShape.GetClientDataArray(0) == null)
					{
						return false;
					}
					result = _vmlShape.GetClientDataArray(0).visibleSpecified;
				}
			}
			return result;
		}
		set
		{
			if (_vmlShape != null)
			{
				string style;
				if (value)
				{
					style = "position:absolute;visibility:visible";
					_vmlShape.GetClientDataArray(0).visible = ST_TrueFalseBlank.@true;
					_vmlShape.GetClientDataArray(0).visibleSpecified = true;
				}
				else
				{
					style = "position:absolute;visibility:hidden";
					_vmlShape.GetClientDataArray(0).visible = ST_TrueFalseBlank.@false;
					_vmlShape.GetClientDataArray(0).visibleSpecified = false;
				}
				_vmlShape.style = style;
			}
		}
	}

	public IRichTextString String
	{
		get
		{
			if (_str == null && _comment.text != null)
			{
				_str = new XSSFRichTextString(_comment.text);
			}
			return _str;
		}
		set
		{
			if (!(value is XSSFRichTextString))
			{
				throw new ArgumentException("Only XSSFRichTextString argument is supported");
			}
			_str = (XSSFRichTextString)value;
			_comment.text = _str.GetCTRst();
		}
	}

	public IClientAnchor ClientAnchor
	{
		get
		{
			string anchorArray = _vmlShape.GetClientDataArray(0).GetAnchorArray(0);
			int[] array = new int[8];
			int num = 0;
			string[] array2 = anchorArray.Split(",".ToCharArray());
			foreach (string text in array2)
			{
				array[num++] = int.Parse(text.Trim());
			}
			return new XSSFClientAnchor(array[1] * Units.EMU_PER_PIXEL, array[3] * Units.EMU_PER_PIXEL, array[5] * Units.EMU_PER_PIXEL, array[7] * Units.EMU_PER_PIXEL, array[0], array[2], array[4], array[6]);
		}
	}

	public XSSFComment(CommentsTable comments, CT_Comment comment, CT_Shape vmlShape)
	{
		_comment = comment;
		_comments = comments;
		_vmlShape = vmlShape;
		if (vmlShape != null && vmlShape.SizeOfClientDataArray() > 0)
		{
			CellReference cellReference = new CellReference(comment.@ref);
			CT_ClientData clientDataArray = vmlShape.GetClientDataArray(0);
			clientDataArray.SetRowArray(0, cellReference.Row);
			clientDataArray.SetColumnArray(0, cellReference.Col);
		}
	}

	public void SetAddress(int row, int col)
	{
		Address = new CellAddress(row, col);
	}

	public void SetString(string str)
	{
		String = new XSSFRichTextString(str);
	}

	internal CT_Comment GetCTComment()
	{
		return _comment;
	}

	internal CT_Shape GetCTShape()
	{
		return _vmlShape;
	}

	public override bool Equals(object obj)
	{
		if (!(obj is XSSFComment))
		{
			return false;
		}
		XSSFComment xSSFComment = (XSSFComment)obj;
		if (GetCTComment() == xSSFComment.GetCTComment())
		{
			return GetCTShape() == xSSFComment.GetCTShape();
		}
		return false;
	}

	public override int GetHashCode()
	{
		return (Row * 17 + Column) * 31;
	}
}
