using System;
using NPOI.HSSF.Record;
using NPOI.SS.UserModel;

namespace NPOI.HSSF.UserModel;

public class HSSFHyperlink : IHyperlink
{
	public HyperlinkRecord record;

	protected HyperlinkType link_type;

	public int FirstRow
	{
		get
		{
			return record.FirstRow;
		}
		set
		{
			record.FirstRow = value;
		}
	}

	public int LastRow
	{
		get
		{
			return record.LastRow;
		}
		set
		{
			record.LastRow = value;
		}
	}

	public int FirstColumn
	{
		get
		{
			return record.FirstColumn;
		}
		set
		{
			record.FirstColumn = value;
		}
	}

	public int LastColumn
	{
		get
		{
			return record.LastColumn;
		}
		set
		{
			record.LastColumn = value;
		}
	}

	public string Address
	{
		get
		{
			return record.Address;
		}
		set
		{
			record.Address = value;
		}
	}

	public string TextMark
	{
		get
		{
			return record.TextMark;
		}
		set
		{
			record.TextMark = value;
		}
	}

	public string ShortFilename
	{
		get
		{
			return record.ShortFilename;
		}
		set
		{
			record.ShortFilename = value;
		}
	}

	public string Label
	{
		get
		{
			return record.Label;
		}
		set
		{
			record.Label = value;
		}
	}

	public HyperlinkType Type => link_type;

	public HSSFHyperlink(HyperlinkType type)
	{
		link_type = type;
		record = new HyperlinkRecord();
		switch (type)
		{
		case HyperlinkType.Url:
		case HyperlinkType.Email:
			record.CreateUrlLink();
			break;
		case HyperlinkType.File:
			record.CreateFileLink();
			break;
		case HyperlinkType.Document:
			record.CreateDocumentLink();
			break;
		default:
			throw new ArgumentException("Invalid type: " + type);
		}
	}

	public HSSFHyperlink(HyperlinkRecord record)
	{
		this.record = record;
		link_type = getType(record);
	}

	private HyperlinkType getType(HyperlinkRecord record)
	{
		if (record.IsFileLink)
		{
			return HyperlinkType.File;
		}
		if (record.IsDocumentLink)
		{
			return HyperlinkType.Document;
		}
		if (record.Address != null && record.Address.StartsWith("mailto:"))
		{
			return HyperlinkType.Email;
		}
		return HyperlinkType.Url;
	}

	public HSSFHyperlink(IHyperlink other)
	{
		if (other is HSSFHyperlink)
		{
			HSSFHyperlink hSSFHyperlink = (HSSFHyperlink)other;
			record = hSSFHyperlink.record.Clone() as HyperlinkRecord;
			link_type = getType(record);
			return;
		}
		link_type = other.Type;
		record = new HyperlinkRecord();
		FirstRow = other.FirstRow;
		FirstColumn = other.FirstColumn;
		LastRow = other.LastRow;
		LastColumn = other.LastColumn;
	}

	public override bool Equals(object other)
	{
		if (this == other)
		{
			return true;
		}
		if (!(other is HSSFHyperlink))
		{
			return false;
		}
		HSSFHyperlink hSSFHyperlink = (HSSFHyperlink)other;
		return record == hSSFHyperlink.record;
	}

	public override int GetHashCode()
	{
		return record.GetHashCode();
	}
}
