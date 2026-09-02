using System;
using System.Collections;
using System.Collections.Generic;
using NPOI.HSSF.Model;
using NPOI.HSSF.Record;
using NPOI.SS.UserModel;

namespace NPOI.HSSF.UserModel;

[Serializable]
public class HSSFRichTextString : IComparable<HSSFRichTextString>, IRichTextString
{
	public const short NO_FONT = 0;

	[NonSerialized]
	private UnicodeString _string;

	private InternalWorkbook _book;

	private LabelSSTRecord _record;

	public string String => _string.String;

	public UnicodeString RawUnicodeString => _string;

	public UnicodeString UnicodeString
	{
		get
		{
			return CloneStringIfRequired();
		}
		set
		{
			_string = value;
		}
	}

	public int Length => _string.CharCount;

	public int NumFormattingRuns => _string.FormatRunCount;

	public HSSFRichTextString()
		: this("")
	{
	}

	public HSSFRichTextString(string str)
	{
		if (str == null)
		{
			_string = new UnicodeString("");
		}
		else
		{
			_string = new UnicodeString(str);
		}
	}

	public HSSFRichTextString(InternalWorkbook book, LabelSSTRecord record)
	{
		SetWorkbookReferences(book, record);
		_string = book.GetSSTString(record.SSTIndex);
	}

	public void SetWorkbookReferences(InternalWorkbook book, LabelSSTRecord record)
	{
		_book = book;
		_record = record;
	}

	private UnicodeString CloneStringIfRequired()
	{
		if (_book == null)
		{
			return _string;
		}
		return (UnicodeString)_string.Clone();
	}

	private void AddToSSTIfRequired()
	{
		if (_book != null)
		{
			int num = _book.AddSSTString(_string);
			_record.SSTIndex = num;
			_string = _book.GetSSTString(num);
		}
	}

	public void ApplyFont(int startIndex, int endIndex, short fontIndex)
	{
		if (startIndex > endIndex)
		{
			throw new ArgumentException("Start index must be less than end index.");
		}
		if (startIndex < 0 || endIndex > Length)
		{
			throw new ArgumentException("Start and end index not in range.");
		}
		if (startIndex == endIndex)
		{
			return;
		}
		short fontIndex2 = 0;
		if (endIndex != Length)
		{
			fontIndex2 = GetFontAtIndex(endIndex);
		}
		_string = CloneStringIfRequired();
		List<UnicodeString.FormatRun> list = _string.FormatIterator();
		ArrayList arrayList = new ArrayList();
		if (list != null)
		{
			IEnumerator<UnicodeString.FormatRun> enumerator = list.GetEnumerator();
			while (enumerator.MoveNext())
			{
				UnicodeString.FormatRun current = enumerator.Current;
				if (current.CharacterPos >= startIndex && current.CharacterPos < endIndex)
				{
					arrayList.Add(current);
				}
			}
		}
		foreach (UnicodeString.FormatRun item in arrayList)
		{
			_string.RemoveFormatRun(item);
		}
		_string.AddFormatRun(new UnicodeString.FormatRun((short)startIndex, fontIndex));
		if (endIndex != Length)
		{
			_string.AddFormatRun(new UnicodeString.FormatRun((short)endIndex, fontIndex2));
		}
		AddToSSTIfRequired();
	}

	public void ApplyFont(int startIndex, int endIndex, IFont font)
	{
		ApplyFont(startIndex, endIndex, font.Index);
	}

	public void ApplyFont(IFont font)
	{
		ApplyFont(0, _string.CharCount, font);
	}

	public void ClearFormatting()
	{
		_string = CloneStringIfRequired();
		_string.ClearFormatting();
		AddToSSTIfRequired();
	}

	public short GetFontAtIndex(int index)
	{
		int formatRunCount = _string.FormatRunCount;
		UnicodeString.FormatRun formatRun = null;
		for (int i = 0; i < formatRunCount; i++)
		{
			UnicodeString.FormatRun formatRun2 = _string.GetFormatRun(i);
			if (formatRun2.CharacterPos > index)
			{
				break;
			}
			formatRun = formatRun2;
		}
		return formatRun?.FontIndex ?? 0;
	}

	public int GetIndexOfFormattingRun(int index)
	{
		return _string.GetFormatRun(index).CharacterPos;
	}

	public short GetFontOfFormattingRun(int index)
	{
		return _string.GetFormatRun(index).FontIndex;
	}

	public int CompareTo(HSSFRichTextString other)
	{
		return _string.CompareTo(other._string);
	}

	public override bool Equals(object o)
	{
		if (o is HSSFRichTextString)
		{
			return _string.Equals(((HSSFRichTextString)o)._string);
		}
		return false;
	}

	public override int GetHashCode()
	{
		return _string.GetHashCode();
	}

	public override string ToString()
	{
		return _string.ToString();
	}

	public void ApplyFont(short fontIndex)
	{
		ApplyFont(0, _string.CharCount, fontIndex);
	}
}
