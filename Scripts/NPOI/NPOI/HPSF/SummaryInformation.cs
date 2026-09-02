using System;
using NPOI.HPSF.Wellknown;

namespace NPOI.HPSF;

[Serializable]
public class SummaryInformation : SpecialPropertySet
{
	public const string DEFAULT_STREAM_NAME = "\u0005SummaryInformation";

	public override PropertyIDMap PropertySetIDMap => PropertyIDMap.SummaryInformationProperties;

	public string Title
	{
		get
		{
			return GetPropertyStringValue(2);
		}
		set
		{
			((MutableSection)FirstSection).SetProperty(2, value);
		}
	}

	public string Subject
	{
		get
		{
			return GetPropertyStringValue(3);
		}
		set
		{
			((MutableSection)FirstSection).SetProperty(3, value);
		}
	}

	public string Author
	{
		get
		{
			return GetPropertyStringValue(4);
		}
		set
		{
			((MutableSection)FirstSection).SetProperty(4, value);
		}
	}

	public string Keywords
	{
		get
		{
			return GetPropertyStringValue(5);
		}
		set
		{
			((MutableSection)FirstSection).SetProperty(5, value);
		}
	}

	public string Comments
	{
		get
		{
			return GetPropertyStringValue(6);
		}
		set
		{
			((MutableSection)FirstSection).SetProperty(6, value);
		}
	}

	public string Template
	{
		get
		{
			return GetPropertyStringValue(7);
		}
		set
		{
			((MutableSection)FirstSection).SetProperty(7, value);
		}
	}

	public string LastAuthor
	{
		get
		{
			return GetPropertyStringValue(8);
		}
		set
		{
			((MutableSection)FirstSection).SetProperty(8, value);
		}
	}

	public string RevNumber
	{
		get
		{
			return GetPropertyStringValue(9);
		}
		set
		{
			((MutableSection)FirstSection).SetProperty(9, value);
		}
	}

	public long EditTime
	{
		get
		{
			if (GetProperty(10) == null)
			{
				return 0L;
			}
			return Util.DateToFileTime((DateTime)GetProperty(10));
		}
		set
		{
			DateTime dateTime = Util.FiletimeToDate(value);
			((MutableSection)FirstSection).SetProperty(10, 64L, dateTime);
		}
	}

	public DateTime? LastPrinted
	{
		get
		{
			return (DateTime?)GetProperty(11);
		}
		set
		{
			((MutableSection)FirstSection).SetProperty(11, 64L, value);
		}
	}

	public DateTime? CreateDateTime
	{
		get
		{
			return (DateTime?)GetProperty(12);
		}
		set
		{
			((MutableSection)FirstSection).SetProperty(12, 64L, value);
		}
	}

	public DateTime? LastSaveDateTime
	{
		get
		{
			return (DateTime?)GetProperty(13);
		}
		set
		{
			((MutableSection)FirstSection).SetProperty(13, 64L, value);
		}
	}

	public int PageCount
	{
		get
		{
			return GetPropertyIntValue(14);
		}
		set
		{
			((MutableSection)FirstSection).SetProperty(14, value);
		}
	}

	public int WordCount
	{
		get
		{
			return GetPropertyIntValue(15);
		}
		set
		{
			((MutableSection)FirstSection).SetProperty(15, value);
		}
	}

	public int CharCount
	{
		get
		{
			return GetPropertyIntValue(16);
		}
		set
		{
			((MutableSection)FirstSection).SetProperty(16, value);
		}
	}

	public byte[] Thumbnail
	{
		get
		{
			return (byte[])GetProperty(17);
		}
		set
		{
			((MutableSection)FirstSection).SetProperty(17, 30L, value);
		}
	}

	public Thumbnail ThumbnailThumbnail
	{
		get
		{
			byte[] thumbnail = Thumbnail;
			if (thumbnail == null)
			{
				return null;
			}
			return new Thumbnail(thumbnail);
		}
	}

	public string ApplicationName
	{
		get
		{
			return GetPropertyStringValue(18);
		}
		set
		{
			((MutableSection)FirstSection).SetProperty(18, value);
		}
	}

	public int Security
	{
		get
		{
			return GetPropertyIntValue(19);
		}
		set
		{
			((MutableSection)FirstSection).SetProperty(19, value);
		}
	}

	public SummaryInformation(PropertySet ps)
		: base(ps)
	{
		if (!IsSummaryInformation)
		{
			throw new UnexpectedPropertySetTypeException("Not a " + GetType().Name);
		}
	}

	public void RemoveTitle()
	{
		((MutableSection)FirstSection).RemoveProperty(2L);
	}

	public void RemoveSubject()
	{
		((MutableSection)FirstSection).RemoveProperty(3L);
	}

	public void RemoveAuthor()
	{
		((MutableSection)FirstSection).RemoveProperty(4L);
	}

	public void RemoveKeywords()
	{
		((MutableSection)FirstSection).RemoveProperty(5L);
	}

	public void RemoveComments()
	{
		((MutableSection)FirstSection).RemoveProperty(6L);
	}

	public void RemoveTemplate()
	{
		((MutableSection)FirstSection).RemoveProperty(7L);
	}

	public void RemoveLastAuthor()
	{
		((MutableSection)FirstSection).RemoveProperty(8L);
	}

	public void RemoveRevNumber()
	{
		((MutableSection)FirstSection).RemoveProperty(9L);
	}

	public void RemoveEditTime()
	{
		((MutableSection)FirstSection).RemoveProperty(10L);
	}

	public void RemoveLastPrinted()
	{
		((MutableSection)FirstSection).RemoveProperty(11L);
	}

	public void RemoveCreateDateTime()
	{
		((MutableSection)FirstSection).RemoveProperty(12L);
	}

	public void RemoveLastSaveDateTime()
	{
		((MutableSection)FirstSection).RemoveProperty(13L);
	}

	public void RemovePageCount()
	{
		((MutableSection)FirstSection).RemoveProperty(14L);
	}

	public void RemoveWordCount()
	{
		((MutableSection)FirstSection).RemoveProperty(15L);
	}

	public void RemoveCharCount()
	{
		((MutableSection)FirstSection).RemoveProperty(16L);
	}

	public void RemoveThumbnail()
	{
		((MutableSection)FirstSection).RemoveProperty(17L);
	}

	public void RemoveApplicationName()
	{
		((MutableSection)FirstSection).RemoveProperty(18L);
	}

	public void RemoveSecurity()
	{
		((MutableSection)FirstSection).RemoveProperty(19L);
	}
}
