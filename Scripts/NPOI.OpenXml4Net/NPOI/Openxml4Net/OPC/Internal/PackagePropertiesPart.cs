using System;
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using NPOI.OpenXml4Net.Exceptions;
using NPOI.SS.Util;

namespace NPOI.OpenXml4Net.OPC.Internal;

public class PackagePropertiesPart(OPCPackage pack, PackagePartName partName) : PackagePart(pack, partName, ContentTypes.CORE_PROPERTIES_PART), PackageProperties
{
	public static string NAMESPACE_DC = "http://purl.org/dc/elements/1.1/";

	public static string NAMESPACE_DC_URI = "http://purl.org/dc/elements/1.1/";

	public static string NAMESPACE_CP_URI = "http://schemas.openxmlformats.org/package/2006/metadata/core-properties";

	public static string NAMESPACE_DCTERMS_URI = "http://purl.org/dc/terms/";

	public static string NAMESPACE_XSI_URI = "http://www.w3.org/2001/XMLSchema-instance";

	private static string DEFAULT_DATEFORMAT = "yyyy-MM-dd'T'HH:mm:ss'Z'";

	private static string[] DATE_FORMATS = new string[2] { DEFAULT_DATEFORMAT, "yyyy-MM-dd'T'HH:mm:ss.ff'Z'" };

	private string[] TZ_DATE_FORMATS = new string[4] { "yyyy-MM-dd'T'HH:mm:ssz", "yyyy-MM-dd'T'HH:mm:ss.fz", "yyyy-MM-dd'T'HH:mm:ss.ffz", "yyyy-MM-dd'T'HH:mm:ss.fffz" };

	private Regex TIME_ZONE_PAT = new Regex("([-+]\\d\\d):?(\\d\\d)");

	protected string category;

	protected string contentStatus;

	protected string contentType;

	protected DateTime? created;

	protected string creator;

	protected string description;

	protected string identifier;

	protected string keywords;

	protected string language;

	protected string lastModifiedBy;

	protected DateTime? lastPrinted;

	protected DateTime? modified;

	protected string revision;

	protected string subject;

	protected string title;

	protected string version;

	public string GetCategoryProperty()
	{
		return category;
	}

	public string GetContentStatusProperty()
	{
		return contentStatus;
	}

	public string GetContentTypeProperty()
	{
		return contentType;
	}

	public DateTime? GetCreatedProperty()
	{
		return created;
	}

	public string GetCreatedPropertyString()
	{
		return GetDateValue(created);
	}

	public string GetCreatorProperty()
	{
		return creator;
	}

	public string GetDescriptionProperty()
	{
		return description;
	}

	public string GetIdentifierProperty()
	{
		return identifier;
	}

	public string GetKeywordsProperty()
	{
		return keywords;
	}

	public string GetLanguageProperty()
	{
		return language;
	}

	public string GetLastModifiedByProperty()
	{
		return lastModifiedBy;
	}

	public DateTime? GetLastPrintedProperty()
	{
		return lastPrinted;
	}

	public string GetLastPrintedPropertyString()
	{
		return GetDateValue(lastPrinted);
	}

	public DateTime? GetModifiedProperty()
	{
		return modified;
	}

	public string GetModifiedPropertyString()
	{
		_ = modified.Value;
		return GetDateValue(modified);
	}

	public string GetRevisionProperty()
	{
		return revision;
	}

	public string GetSubjectProperty()
	{
		return subject;
	}

	public string GetTitleProperty()
	{
		return title;
	}

	public string GetVersionProperty()
	{
		return version;
	}

	public void SetCategoryProperty(string category)
	{
		this.category = SetStringValue(category);
	}

	public void SetContentStatusProperty(string contentStatus)
	{
		this.contentStatus = SetStringValue(contentStatus);
	}

	public void SetContentTypeProperty(string contentType)
	{
		this.contentType = SetStringValue(contentType);
	}

	public void SetCreatedProperty(string created)
	{
		try
		{
			this.created = SetDateValue(created);
		}
		catch (InvalidFormatException innerException)
		{
			throw new ArgumentException("Date for created could not be parsed: " + created, innerException);
		}
	}

	public void SetCreatedProperty(DateTime? created)
	{
		if (created.HasValue)
		{
			this.created = created;
		}
	}

	public void SetCreatorProperty(string creator)
	{
		this.creator = SetStringValue(creator);
	}

	public void SetDescriptionProperty(string description)
	{
		this.description = SetStringValue(description);
	}

	public void SetIdentifierProperty(string identifier)
	{
		this.identifier = SetStringValue(identifier);
	}

	public void SetKeywordsProperty(string keywords)
	{
		this.keywords = SetStringValue(keywords);
	}

	public void SetLanguageProperty(string language)
	{
		this.language = SetStringValue(language);
	}

	public void SetLastModifiedByProperty(string lastModifiedBy)
	{
		this.lastModifiedBy = SetStringValue(lastModifiedBy);
	}

	public void SetLastPrintedProperty(string lastPrinted)
	{
		try
		{
			this.lastPrinted = SetDateValue(lastPrinted);
		}
		catch (InvalidFormatException ex)
		{
			new ArgumentException("lastPrinted  : " + ex.Message, ex);
		}
	}

	public void SetLastPrintedProperty(DateTime? lastPrinted)
	{
		if (lastPrinted.HasValue)
		{
			this.lastPrinted = lastPrinted;
		}
	}

	public void SetModifiedProperty(string modified)
	{
		try
		{
			this.modified = SetDateValue(modified);
		}
		catch (InvalidFormatException ex)
		{
			new ArgumentException("modified  : " + ex.Message, ex);
		}
	}

	public void SetModifiedProperty(DateTime? modified)
	{
		if (modified.HasValue)
		{
			this.modified = modified;
		}
	}

	public void SetRevisionProperty(string revision)
	{
		this.revision = SetStringValue(revision);
	}

	public void SetSubjectProperty(string subject)
	{
		this.subject = SetStringValue(subject);
	}

	public void SetTitleProperty(string title)
	{
		this.title = SetStringValue(title);
	}

	public void SetVersionProperty(string version)
	{
		this.version = SetStringValue(version);
	}

	private string SetStringValue(string s)
	{
		if (s == null || s.Equals(""))
		{
			return null;
		}
		return s;
	}

	private DateTime? SetDateValue(string dateStr)
	{
		if (dateStr == null || dateStr.Equals(""))
		{
			return null;
		}
		Match match = TIME_ZONE_PAT.Match(dateStr);
		string source;
		int num;
		string[] tZ_DATE_FORMATS;
		if (match.Success)
		{
			source = dateStr.Substring(0, match.Index) + match.Groups[1].Value + match.Groups[2].Value;
			tZ_DATE_FORMATS = TZ_DATE_FORMATS;
			num = 0;
			if (num < tZ_DATE_FORMATS.Length)
			{
				return new SimpleDateFormat(tZ_DATE_FORMATS[num])
				{
					TimeZone = TimeZoneInfo.Utc
				}.Parse(source);
			}
		}
		source = (dateStr.EndsWith("Z") ? dateStr : (dateStr + "Z"));
		tZ_DATE_FORMATS = DATE_FORMATS;
		num = 0;
		if (num < tZ_DATE_FORMATS.Length)
		{
			return new SimpleDateFormat(tZ_DATE_FORMATS[num])
			{
				TimeZone = TimeZoneInfo.Utc
			}.Parse(source).ToUniversalTime();
		}
		StringBuilder stringBuilder = new StringBuilder();
		int num2 = 0;
		tZ_DATE_FORMATS = TZ_DATE_FORMATS;
		foreach (string value in tZ_DATE_FORMATS)
		{
			if (num2++ > 0)
			{
				stringBuilder.Append(", ");
			}
			stringBuilder.Append(value);
		}
		tZ_DATE_FORMATS = DATE_FORMATS;
		foreach (string value2 in tZ_DATE_FORMATS)
		{
			stringBuilder.Append(", ").Append(value2);
		}
		throw new InvalidFormatException("Date " + dateStr + " not well formatted, expected format in: " + stringBuilder.ToString());
	}

	private string GetDateValue(DateTime? d)
	{
		if (!d.HasValue || !d.HasValue || d.Equals(""))
		{
			return "";
		}
		return new SimpleDateFormat(DEFAULT_DATEFORMAT)
		{
			TimeZone = TimeZoneInfo.Utc
		}.Format(d.Value, CultureInfo.InvariantCulture);
	}

	protected override Stream GetInputStreamImpl()
	{
		throw new InvalidOperationException("Operation not authorized");
	}

	protected override Stream GetOutputStreamImpl()
	{
		throw new InvalidOperationException("Operation not authorized");
	}

	public override bool Save(Stream zos)
	{
		throw new InvalidOperationException("Operation not authorized");
	}

	public override bool Load(Stream ios)
	{
		throw new InvalidOperationException("Operation not authorized");
	}

	public override void Close()
	{
	}

	public override void Flush()
	{
	}
}
