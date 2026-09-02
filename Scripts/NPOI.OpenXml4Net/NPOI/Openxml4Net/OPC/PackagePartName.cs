using System;
using System.Text.RegularExpressions;
using NPOI.OpenXml4Net.Exceptions;

namespace NPOI.OpenXml4Net.OPC;

public class PackagePartName : IComparable<PackagePartName>
{
	private Uri partNameURI;

	private static string[] RFC3986_PCHAR_SUB_DELIMS = new string[11]
	{
		"!", "$", "&", "'", "(", ")", "*", "+", ",", ";",
		"="
	};

	private static string[] RFC3986_PCHAR_UNRESERVED_SUP = new string[4] { "-", ".", "_", "~" };

	private static string[] RFC3986_PCHAR_AUTHORIZED_SUP = new string[2] { ":", "@" };

	private bool isRelationship;

	public string Extension
	{
		get
		{
			string originalString = partNameURI.OriginalString;
			if (originalString.Length > 0)
			{
				int num = originalString.LastIndexOf(".");
				if (num > -1)
				{
					return originalString.Substring(num + 1);
				}
			}
			return "";
		}
	}

	public string Name => partNameURI.OriginalString;

	public Uri URI => partNameURI;

	public PackagePartName(Uri uri, bool checkConformance)
	{
		if (checkConformance)
		{
			ThrowExceptionIfInvalidPartUri(uri);
		}
		else if (!PackagingUriHelper.PACKAGE_ROOT_URI.Equals(uri))
		{
			throw new OpenXml4NetException("OCP conformance must be check for ALL part name except special cases : ['/']");
		}
		partNameURI = uri;
		isRelationship = IsRelationshipPartURI(partNameURI);
	}

	internal PackagePartName(string partName, bool checkConformance)
	{
		Uri uri;
		try
		{
			uri = PackagingUriHelper.ParseUri(partName, UriKind.RelativeOrAbsolute);
		}
		catch (UriFormatException)
		{
			throw new ArgumentException("partName argmument is not a valid OPC part name !");
		}
		if (checkConformance)
		{
			ThrowExceptionIfInvalidPartUri(uri);
		}
		else if (!PackagingUriHelper.PACKAGE_ROOT_URI.Equals(uri))
		{
			throw new OpenXml4NetException("OCP conformance must be check for ALL part name except special cases : ['/']");
		}
		partNameURI = uri;
		isRelationship = IsRelationshipPartURI(partNameURI);
	}

	private bool IsRelationshipPartURI(Uri partUri)
	{
		if (partUri == null)
		{
			throw new ArgumentException("partUri");
		}
		return Regex.IsMatch(partUri.OriginalString, "^.*/" + PackagingUriHelper.RELATIONSHIP_PART_SEGMENT_NAME + "/.*\\" + PackagingUriHelper.RELATIONSHIP_PART_EXTENSION_NAME + "$");
	}

	public bool IsRelationshipPartURI()
	{
		return isRelationship;
	}

	private static void ThrowExceptionIfInvalidPartUri(Uri partUri)
	{
		if (partUri == null)
		{
			throw new ArgumentException("partUri");
		}
		ThrowExceptionIfEmptyURI(partUri);
		ThrowExceptionIfAbsoluteUri(partUri);
		ThrowExceptionIfPartNameNotStartsWithForwardSlashChar(partUri);
		ThrowExceptionIfPartNameEndsWithForwardSlashChar(partUri);
		ThrowExceptionIfPartNameHaveInvalidSegments(partUri);
	}

	private static void ThrowExceptionIfEmptyURI(Uri partURI)
	{
		if (partURI == null)
		{
			throw new ArgumentException("partURI");
		}
		string originalString = partURI.OriginalString;
		if (originalString.Length == 0 || (originalString.Length == 1 && originalString[0] == PackagingUriHelper.FORWARD_SLASH_CHAR))
		{
			throw new InvalidFormatException("A part name shall not be empty [M1.1]: " + partURI.OriginalString);
		}
	}

	private static void ThrowExceptionIfPartNameHaveInvalidSegments(Uri partUri)
	{
		if (partUri == null || "".Equals(partUri))
		{
			throw new ArgumentException("partUri");
		}
		string[] array = partUri.OriginalString.Split(new char[1] { '/' });
		if (array.Length <= 1 || !array[0].Equals(""))
		{
			throw new InvalidFormatException("A part name shall not have empty segments [M1.3]: " + partUri.OriginalString);
		}
		for (int i = 1; i < array.Length; i++)
		{
			string text = array[i];
			if (text == null || "".Equals(text))
			{
				throw new InvalidFormatException("A part name shall not have empty segments [M1.3]: " + partUri.OriginalString);
			}
			if (text.EndsWith("."))
			{
				throw new InvalidFormatException("A segment shall not end with a dot ('.') character [M1.9]: " + partUri.OriginalString);
			}
			if ("".Equals(text.Replace("\\\\.", "")))
			{
				throw new InvalidFormatException("A segment shall include at least one non-dot character. [M1.10]: " + partUri.OriginalString);
			}
			CheckPCharCompliance(text);
		}
	}

	private static void CheckPCharCompliance(string segment)
	{
		int length = segment.Length;
		for (int i = 0; i < length; i++)
		{
			char c = segment[i];
			bool flag = true;
			if ((c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z') || (c >= '0' && c <= '9'))
			{
				flag = false;
			}
			else
			{
				for (int j = 0; j < RFC3986_PCHAR_UNRESERVED_SUP.Length; j++)
				{
					if (c == RFC3986_PCHAR_UNRESERVED_SUP[j][0])
					{
						flag = false;
						break;
					}
				}
				int num = 0;
				while (flag && num < RFC3986_PCHAR_AUTHORIZED_SUP.Length)
				{
					if (c == RFC3986_PCHAR_AUTHORIZED_SUP[num][0])
					{
						flag = false;
					}
					num++;
				}
				int num2 = 0;
				while (flag && num2 < RFC3986_PCHAR_SUB_DELIMS.Length)
				{
					if (c == RFC3986_PCHAR_SUB_DELIMS[num2][0])
					{
						flag = false;
					}
					num2++;
				}
			}
			if (flag && c == '%')
			{
				if (length - i < 2)
				{
					throw new InvalidFormatException("The segment " + segment + " contain invalid encoded character !");
				}
				flag = false;
				char c2 = (char)Convert.ToInt32(segment.Substring(i + 1, 2), 16);
				i += 2;
				if (c2 == '/' || c2 == '\\')
				{
					throw new InvalidFormatException("A segment shall not contain percent-encoded forward slash ('/'), or backward slash ('') characters. [M1.7]");
				}
				if ((c2 >= 'A' && c2 <= 'Z') || (c2 >= 'a' && c2 <= 'z') || (c2 >= '0' && c2 <= '9'))
				{
					flag = true;
				}
				int num3 = 0;
				while (!flag && num3 < RFC3986_PCHAR_UNRESERVED_SUP.Length)
				{
					if (c == RFC3986_PCHAR_UNRESERVED_SUP[num3][0])
					{
						flag = true;
						break;
					}
					num3++;
				}
				if (flag)
				{
					throw new InvalidFormatException("A segment shall not contain percent-encoded unreserved characters. [M1.8]");
				}
			}
			if (flag)
			{
				throw new InvalidFormatException("A segment shall not hold any characters other than pchar characters. [M1.6]");
			}
		}
	}

	private static void ThrowExceptionIfPartNameNotStartsWithForwardSlashChar(Uri partUri)
	{
		string originalString = partUri.OriginalString;
		if (originalString.Length > 0 && originalString[0] != PackagingUriHelper.FORWARD_SLASH_CHAR)
		{
			throw new InvalidFormatException("A part name shall start with a forward slash ('/') character [M1.4]: " + partUri.OriginalString);
		}
	}

	private static void ThrowExceptionIfPartNameEndsWithForwardSlashChar(Uri partUri)
	{
		string originalString = partUri.OriginalString;
		if (originalString.Length > 0 && originalString[originalString.Length - 1] == PackagingUriHelper.FORWARD_SLASH_CHAR)
		{
			throw new InvalidFormatException("A part name shall not have a forward slash as the last character [M1.5]: " + partUri.OriginalString);
		}
	}

	private static void ThrowExceptionIfAbsoluteUri(Uri partUri)
	{
		if (partUri.IsAbsoluteUri)
		{
			throw new InvalidFormatException("Absolute URI forbidden: " + partUri);
		}
	}

	public int CompareTo(PackagePartName other)
	{
		return Compare(this, other);
	}

	public override bool Equals(object other)
	{
		if (other is PackagePartName)
		{
			return partNameURI.OriginalString.ToLower().Equals(((PackagePartName)other).partNameURI.OriginalString.ToLower());
		}
		return false;
	}

	public override int GetHashCode()
	{
		return partNameURI.OriginalString.ToLower().GetHashCode();
	}

	public override string ToString()
	{
		return Name;
	}

	public static int Compare(PackagePartName obj1, PackagePartName obj2)
	{
		if (obj1 == null)
		{
			if (obj2 != null)
			{
				return -1;
			}
			return 0;
		}
		if (obj2 == null)
		{
			return 1;
		}
		return Compare(obj1.URI.OriginalString.ToLower(), obj2.URI.OriginalString.ToLower());
	}

	public static int Compare(string str1, string str2)
	{
		if (str1 == null)
		{
			if (str2 != null)
			{
				return -1;
			}
			return 0;
		}
		if (str2 == null)
		{
			return 1;
		}
		int length = str1.Length;
		int length2 = str2.Length;
		int i = 0;
		int j = 0;
		while (i < length && j < length2)
		{
			char c = str1[i++];
			char c2 = str2[j++];
			if (char.IsDigit(c) && char.IsDigit(c2))
			{
				int num = i - 1;
				for (; i < length && char.IsDigit(str1[i]); i++)
				{
				}
				int num2 = j - 1;
				for (; j < length2 && char.IsDigit(str2[j]); j++)
				{
				}
				int num3 = decimal.Parse(str1.Substring(num, i - num)).CompareTo(decimal.Parse(str2.Substring(num2, j - num2)));
				if (num3 != 0)
				{
					return num3;
				}
			}
			else if (c != c2)
			{
				return c - c2;
			}
		}
		return length - length2;
	}
}
