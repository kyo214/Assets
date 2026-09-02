using System.Collections;

namespace NPOI.HPSF.Wellknown;

public class PropertyIDMap : Hashtable
{
	public const int PID_TITLE = 2;

	public const int PID_SUBJECT = 3;

	public const int PID_AUTHOR = 4;

	public const int PID_KEYWORDS = 5;

	public const int PID_COMMENTS = 6;

	public const int PID_TEMPLATE = 7;

	public const int PID_LASTAUTHOR = 8;

	public const int PID_REVNUMBER = 9;

	public const int PID_EDITTIME = 10;

	public const int PID_LASTPRINTED = 11;

	public const int PID_Create_DTM = 12;

	public const int PID_LASTSAVE_DTM = 13;

	public const int PID_PAGECOUNT = 14;

	public const int PID_WORDCOUNT = 15;

	public const int PID_CHARCOUNT = 16;

	public const int PID_THUMBNAIL = 17;

	public const int PID_APPNAME = 18;

	public const int PID_SECURITY = 19;

	public const int PID_DICTIONARY = 0;

	public const int PID_CODEPAGE = 1;

	public const int PID_CATEGORY = 2;

	public const int PID_PRESFORMAT = 3;

	public const int PID_BYTECOUNT = 4;

	public const int PID_LINECOUNT = 5;

	public const int PID_PARCOUNT = 6;

	public const int PID_SLIDECOUNT = 7;

	public const int PID_NOTECOUNT = 8;

	public const int PID_HIDDENCOUNT = 9;

	public const int PID_MMCLIPCOUNT = 10;

	public const int PID_SCALE = 11;

	public const int PID_HEADINGPAIR = 12;

	public const int PID_DOCPARTS = 13;

	public const int PID_MANAGER = 14;

	public const int PID_COMPANY = 15;

	public const int PID_LINKSDIRTY = 16;

	public static int PID_CCHWITHSPACES = 17;

	public static int PID_HYPERLINKSCHANGED = 22;

	public static int PID_VERSION = 23;

	public static int PID_DIGSIG = 24;

	public static int PID_CONTENTTYPE = 26;

	public static int PID_CONTENTSTATUS = 27;

	public static int PID_LANGUAGE = 28;

	public static int PID_DOCVERSION = 29;

	public const int PID_MAX = 31;

	private static PropertyIDMap summaryInformationProperties;

	private static PropertyIDMap documentSummaryInformationProperties;

	public static PropertyIDMap SummaryInformationProperties
	{
		get
		{
			if (summaryInformationProperties == null)
			{
				PropertyIDMap propertyIDMap = new PropertyIDMap(18, 1f);
				propertyIDMap.Put(2L, "PID_TITLE");
				propertyIDMap.Put(3L, "PID_SUBJECT");
				propertyIDMap.Put(4L, "PID_AUTHOR");
				propertyIDMap.Put(5L, "PID_KEYWORDS");
				propertyIDMap.Put(6L, "PID_COMMENTS");
				propertyIDMap.Put(7L, "PID_TEMPLATE");
				propertyIDMap.Put(8L, "PID_LASTAUTHOR");
				propertyIDMap.Put(9L, "PID_REVNUMBER");
				propertyIDMap.Put(10L, "PID_EDITTIME");
				propertyIDMap.Put(11L, "PID_LASTPRINTED");
				propertyIDMap.Put(12L, "PID_Create_DTM");
				propertyIDMap.Put(13L, "PID_LASTSAVE_DTM");
				propertyIDMap.Put(14L, "PID_PAGECOUNT");
				propertyIDMap.Put(15L, "PID_WORDCOUNT");
				propertyIDMap.Put(16L, "PID_CHARCOUNT");
				propertyIDMap.Put(17L, "PID_THUMBNAIL");
				propertyIDMap.Put(18L, "PID_APPNAME");
				propertyIDMap.Put(19L, "PID_SECURITY");
				summaryInformationProperties = propertyIDMap;
			}
			return summaryInformationProperties;
		}
	}

	public static PropertyIDMap DocumentSummaryInformationProperties
	{
		get
		{
			if (documentSummaryInformationProperties == null)
			{
				PropertyIDMap propertyIDMap = new PropertyIDMap(17, 1f);
				propertyIDMap.Put(0L, "PID_DICTIONARY");
				propertyIDMap.Put(1L, "PID_CODEPAGE");
				propertyIDMap.Put(2L, "PID_CATEGORY");
				propertyIDMap.Put(3L, "PID_PRESFORMAT");
				propertyIDMap.Put(4L, "PID_BYTECOUNT");
				propertyIDMap.Put(5L, "PID_LINECOUNT");
				propertyIDMap.Put(6L, "PID_PARCOUNT");
				propertyIDMap.Put(7L, "PID_SLIDECOUNT");
				propertyIDMap.Put(8L, "PID_NOTECOUNT");
				propertyIDMap.Put(9L, "PID_HIDDENCOUNT");
				propertyIDMap.Put(10L, "PID_MMCLIPCOUNT");
				propertyIDMap.Put(11L, "PID_SCALE");
				propertyIDMap.Put(12L, "PID_HEADINGPAIR");
				propertyIDMap.Put(13L, "PID_DOCPARTS");
				propertyIDMap.Put(14L, "PID_MANAGER");
				propertyIDMap.Put(15L, "PID_COMPANY");
				propertyIDMap.Put(16L, "PID_LINKSDIRTY");
				documentSummaryInformationProperties = propertyIDMap;
			}
			return documentSummaryInformationProperties;
		}
	}

	public PropertyIDMap(int initialCapacity, float loadFactor)
		: base(initialCapacity, loadFactor)
	{
	}

	public PropertyIDMap(IDictionary map)
		: base(map)
	{
	}

	public object Put(long id, string idString)
	{
		return this[id] = idString;
	}

	public object Get(long id)
	{
		return this[id];
	}
}
