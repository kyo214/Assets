using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using NPOI.OpenXml4Net.Exceptions;

namespace NPOI.OpenXml4Net.OPC;

public class PackagingUriHelper
{
	private static Uri packageRootUri;

	public static string RELATIONSHIP_PART_EXTENSION_NAME;

	public static string RELATIONSHIP_PART_SEGMENT_NAME;

	public static string PACKAGE_PROPERTIES_SEGMENT_NAME;

	public static string PACKAGE_CORE_PROPERTIES_NAME;

	public static char FORWARD_SLASH_CHAR;

	public static string FORWARD_SLASH_STRING;

	public static Uri PACKAGE_RELATIONSHIPS_ROOT_URI;

	public static PackagePartName PACKAGE_RELATIONSHIPS_ROOT_PART_NAME;

	public static Uri CORE_PROPERTIES_URI;

	public static PackagePartName CORE_PROPERTIES_PART_NAME;

	public static Uri PACKAGE_ROOT_URI;

	public static PackagePartName PACKAGE_ROOT_PART_NAME;

	private static Regex missingAuthPattern;

	private static readonly bool IsMono;

	private static char[] hexDigits;

	public static Uri PackageRootUri => packageRootUri;

	static PackagingUriHelper()
	{
		missingAuthPattern = new Regex("\\w+://$", RegexOptions.Compiled);
		IsMono = Type.GetType("Mono.Runtime") != null;
		hexDigits = new char[16]
		{
			'0', '1', '2', '3', '4', '5', '6', '7', '8', '9',
			'A', 'B', 'C', 'D', 'E', 'F'
		};
		RELATIONSHIP_PART_SEGMENT_NAME = "_rels";
		RELATIONSHIP_PART_EXTENSION_NAME = ".rels";
		FORWARD_SLASH_CHAR = '/';
		FORWARD_SLASH_STRING = "/";
		PACKAGE_PROPERTIES_SEGMENT_NAME = "docProps";
		PACKAGE_CORE_PROPERTIES_NAME = "core.xml";
		Uri uri = null;
		Uri uri2 = null;
		uri = ParseUri("/", UriKind.Relative);
		uri2 = ParseUri(FORWARD_SLASH_CHAR + RELATIONSHIP_PART_SEGMENT_NAME + FORWARD_SLASH_CHAR + RELATIONSHIP_PART_EXTENSION_NAME, UriKind.Relative);
		packageRootUri = ParseUri("/", UriKind.Relative);
		Uri cORE_PROPERTIES_URI = ParseUri(FORWARD_SLASH_CHAR + PACKAGE_PROPERTIES_SEGMENT_NAME + FORWARD_SLASH_CHAR + PACKAGE_CORE_PROPERTIES_NAME, UriKind.Relative);
		PACKAGE_ROOT_URI = uri;
		PACKAGE_RELATIONSHIPS_ROOT_URI = uri2;
		CORE_PROPERTIES_URI = cORE_PROPERTIES_URI;
		PackagePartName pACKAGE_ROOT_PART_NAME = null;
		PackagePartName pACKAGE_RELATIONSHIPS_ROOT_PART_NAME = null;
		PackagePartName cORE_PROPERTIES_PART_NAME = null;
		try
		{
			pACKAGE_RELATIONSHIPS_ROOT_PART_NAME = CreatePartName(PACKAGE_RELATIONSHIPS_ROOT_URI);
			cORE_PROPERTIES_PART_NAME = CreatePartName(CORE_PROPERTIES_URI);
			pACKAGE_ROOT_PART_NAME = new PackagePartName(PACKAGE_ROOT_URI, checkConformance: false);
		}
		catch (InvalidFormatException)
		{
		}
		PACKAGE_RELATIONSHIPS_ROOT_PART_NAME = pACKAGE_RELATIONSHIPS_ROOT_PART_NAME;
		CORE_PROPERTIES_PART_NAME = cORE_PROPERTIES_PART_NAME;
		PACKAGE_ROOT_PART_NAME = pACKAGE_ROOT_PART_NAME;
	}

	public static Uri ParseUri(string s, UriKind kind)
	{
		if (IsMono)
		{
			switch (kind)
			{
			case UriKind.Absolute:
				throw new UriFormatException();
			case UriKind.RelativeOrAbsolute:
				if (s.StartsWith("/"))
				{
					kind = UriKind.Relative;
				}
				break;
			}
		}
		return new Uri(s, kind);
	}

	public static bool IsRelationshipPartURI(Uri partUri)
	{
		if (partUri == null)
		{
			throw new ArgumentException("partUri");
		}
		return Regex.IsMatch(partUri.OriginalString, ".*" + RELATIONSHIP_PART_SEGMENT_NAME + ".*" + RELATIONSHIP_PART_EXTENSION_NAME + "$");
	}

	public static string GetFilename(Uri uri)
	{
		if (uri != null)
		{
			string originalString = uri.OriginalString;
			int num = originalString.Length;
			while (--num >= 0)
			{
				if (originalString[num] == FORWARD_SLASH_CHAR)
				{
					return originalString.Substring(num + 1);
				}
			}
		}
		return "";
	}

	public static string GetFilenameWithoutExtension(Uri uri)
	{
		string filename = GetFilename(uri);
		int num = filename.LastIndexOf(".");
		if (num == -1)
		{
			return filename;
		}
		return filename.Substring(0, num);
	}

	public static Uri GetPath(Uri uri)
	{
		if (uri != null)
		{
			string originalString = uri.OriginalString;
			int num = originalString.Length;
			while (--num >= 0)
			{
				if (originalString[num] == FORWARD_SLASH_CHAR)
				{
					try
					{
						return ParseUri(originalString.Substring(0, num), UriKind.Absolute);
					}
					catch (UriFormatException)
					{
						return null;
					}
				}
			}
		}
		return null;
	}

	public static Uri Combine(Uri prefix, Uri suffix)
	{
		Uri uri = null;
		try
		{
			return ParseUri(Combine(prefix.OriginalString, suffix.OriginalString), UriKind.Absolute);
		}
		catch (UriFormatException)
		{
			throw new ArgumentException("Prefix and suffix can't be Combine !");
		}
	}

	public static string Combine(string prefix, string suffix)
	{
		if (!prefix.EndsWith(FORWARD_SLASH_CHAR.ToString() ?? "") && !suffix.StartsWith(FORWARD_SLASH_CHAR.ToString() ?? ""))
		{
			return prefix + FORWARD_SLASH_CHAR + suffix;
		}
		if ((!prefix.EndsWith(FORWARD_SLASH_CHAR.ToString() ?? "") && suffix.StartsWith(FORWARD_SLASH_CHAR.ToString() ?? "")) || (prefix.EndsWith(FORWARD_SLASH_CHAR.ToString() ?? "") && !suffix.StartsWith(FORWARD_SLASH_CHAR.ToString() ?? "")))
		{
			return prefix + suffix;
		}
		return "";
	}

	public static Uri RelativizeUri(Uri sourceURI, Uri targetURI, bool msCompatible)
	{
		StringBuilder stringBuilder = new StringBuilder();
		string[] array = sourceURI.ToString().Split(new char[1] { '/' });
		string[] array2 = targetURI.ToString().Split(new char[1] { '/' });
		if (array.Length == 0)
		{
			throw new ArgumentException("Can't relativize an empty source Uri !");
		}
		if (array2.Length == 0)
		{
			throw new ArgumentException("Can't relativize an empty target Uri !");
		}
		if (sourceURI.ToString().Equals("/"))
		{
			string text = targetURI.ToString();
			if (msCompatible && text.Length > 0 && text[0] == '/')
			{
				try
				{
					targetURI = ParseUri(text.Substring(1), UriKind.RelativeOrAbsolute);
				}
				catch (Exception)
				{
					return null;
				}
			}
			return targetURI;
		}
		int num = 0;
		for (int i = 0; i < array.Length && i < array2.Length && array[i].Equals(array2[i]); i++)
		{
			num++;
		}
		if ((num == 0 || num == 1) && array[0].Equals("") && array2[0].Equals(""))
		{
			for (int j = 0; j < array.Length - 2; j++)
			{
				stringBuilder.Append("../");
			}
			for (int k = 0; k < array2.Length; k++)
			{
				if (!array2[k].Equals(""))
				{
					stringBuilder.Append(array2[k]);
					if (k != array2.Length - 1)
					{
						stringBuilder.Append("/");
					}
				}
			}
			try
			{
				return ParseUri(stringBuilder.ToString(), UriKind.RelativeOrAbsolute);
			}
			catch (Exception)
			{
				return null;
			}
		}
		if (num == array.Length && num == array2.Length)
		{
			if (sourceURI.Equals(targetURI))
			{
				stringBuilder.Append(array[^1]);
			}
			else
			{
				stringBuilder.Append("");
			}
		}
		else
		{
			if (num == 1)
			{
				stringBuilder.Append("/");
			}
			else
			{
				for (int l = num; l < array.Length - 1; l++)
				{
					stringBuilder.Append("../");
				}
			}
			for (int m = num; m < array2.Length; m++)
			{
				if (stringBuilder.Length > 0 && stringBuilder[stringBuilder.Length - 1] != '/')
				{
					stringBuilder.Append("/");
				}
				stringBuilder.Append(array2[m]);
			}
		}
		try
		{
			return ParseUri(stringBuilder.ToString(), UriKind.RelativeOrAbsolute);
		}
		catch (Exception)
		{
			return null;
		}
	}

	public static Uri RelativizeUri(Uri sourceURI, Uri targetURI)
	{
		return RelativizeUri(sourceURI, targetURI, msCompatible: false);
	}

	public static Uri ResolvePartUri(Uri sourcePartUri, Uri targetUri)
	{
		if (sourcePartUri == null || sourcePartUri.IsAbsoluteUri)
		{
			throw new ArgumentException("sourcePartUri invalid - " + sourcePartUri);
		}
		if (targetUri == null || targetUri.IsAbsoluteUri)
		{
			throw new ArgumentException("targetUri invalid - " + targetUri);
		}
		string text = ((!(sourcePartUri.OriginalString == "/")) ? Path.GetDirectoryName(sourcePartUri.OriginalString).Replace("\\", "/") : "/");
		string text2 = targetUri.OriginalString;
		if (text2.StartsWith("#"))
		{
			text = text + "/" + Path.GetFileName(sourcePartUri.OriginalString) + text2;
		}
		else if (text2.StartsWith("../"))
		{
			string[] array = text.Split(new char[1] { '/' });
			int num = array.Length - 1;
			while (text2.StartsWith("../"))
			{
				text2 = text2.Substring(3);
				num--;
			}
			text = "/";
			for (int i = 0; i <= num; i++)
			{
				if (array[i] != string.Empty)
				{
					text = text + array[i] + "/";
				}
			}
			text += text2;
		}
		else
		{
			text = Path.Combine(text, targetUri.OriginalString).Replace("\\", "/");
		}
		return ParseUri(text, UriKind.RelativeOrAbsolute);
	}

	public static Uri GetURIFromPath(string path)
	{
		Uri uri = null;
		try
		{
			return ParseUri(path, UriKind.RelativeOrAbsolute);
		}
		catch (UriFormatException)
		{
			throw new ArgumentException("path");
		}
	}

	public static Uri GetSourcePartUriFromRelationshipPartUri(Uri relationshipPartUri)
	{
		if (relationshipPartUri == null)
		{
			throw new ArgumentException("Must not be null");
		}
		if (!IsRelationshipPartURI(relationshipPartUri))
		{
			throw new ArgumentException("Must be a relationship part");
		}
		if (Uri.Compare(relationshipPartUri, PACKAGE_RELATIONSHIPS_ROOT_URI, UriComponents.AbsoluteUri, UriFormat.SafeUnescaped, StringComparison.InvariantCultureIgnoreCase) == 0)
		{
			return PACKAGE_ROOT_URI;
		}
		string originalString = relationshipPartUri.OriginalString;
		string filenameWithoutExtension = GetFilenameWithoutExtension(relationshipPartUri);
		originalString = originalString.Substring(0, originalString.Length - filenameWithoutExtension.Length - RELATIONSHIP_PART_EXTENSION_NAME.Length);
		originalString = originalString.Substring(0, originalString.Length - RELATIONSHIP_PART_SEGMENT_NAME.Length - 1);
		originalString = Combine(originalString, filenameWithoutExtension);
		return GetURIFromPath(originalString);
	}

	public static PackagePartName CreatePartName(Uri partUri)
	{
		if (partUri == null)
		{
			throw new ArgumentException("partName");
		}
		return new PackagePartName(partUri, checkConformance: true);
	}

	public static PackagePartName CreatePartName(string partName)
	{
		Uri partUri;
		try
		{
			partName = partName.Replace("\\", "/");
			partUri = ParseUri(partName, UriKind.Relative);
		}
		catch (UriFormatException ex)
		{
			throw new InvalidFormatException(ex.Message);
		}
		return CreatePartName(partUri);
	}

	public static PackagePartName CreatePartName(string partName, PackagePart relativePart)
	{
		Uri partUri;
		try
		{
			partUri = ResolvePartUri(relativePart.PartName.URI, ParseUri(partName, UriKind.RelativeOrAbsolute));
		}
		catch (UriFormatException ex)
		{
			throw new InvalidFormatException(ex.Message);
		}
		return CreatePartName(partUri);
	}

	public static PackagePartName CreatePartName(Uri partName, PackagePart relativePart)
	{
		return CreatePartName(ResolvePartUri(relativePart.PartName.URI, partName));
	}

	public static bool IsValidPartName(Uri partUri)
	{
		if (partUri == null)
		{
			throw new ArgumentException("partUri");
		}
		try
		{
			CreatePartName(partUri);
			return true;
		}
		catch (Exception)
		{
			return false;
		}
	}

	public static string DecodeURI(Uri uri)
	{
		StringBuilder stringBuilder = new StringBuilder();
		string originalString = uri.OriginalString;
		int length = originalString.Length;
		for (int i = 0; i < length; i++)
		{
			char c = originalString[i];
			if (c == '%')
			{
				if (length - i < 2)
				{
					throw new ArgumentException("The uri " + originalString + " contain invalid encoded character !");
				}
				char value = (char)Convert.ToInt32(originalString.Substring(i + 1, 2), 16);
				stringBuilder.Append(value);
				i += 2;
			}
			else
			{
				stringBuilder.Append(c);
			}
		}
		return stringBuilder.ToString();
	}

	public static Uri ToUri(string value)
	{
		if (value.IndexOf("\\") != -1)
		{
			value = value.Replace('\\', '/');
		}
		int num = value.IndexOf('#');
		if (num != -1)
		{
			string text = value.Substring(0, num);
			string s = value.Substring(num + 1);
			value = text + "#" + Encode(s);
		}
		if (value.Length > 0)
		{
			StringBuilder stringBuilder = new StringBuilder();
			int num2;
			for (num2 = value.Length - 1; num2 >= 0; num2--)
			{
				char c = value[num2];
				if (!char.IsWhiteSpace(c) && c != '\u00a0')
				{
					break;
				}
				stringBuilder.Append(c);
			}
			if (stringBuilder.Length > 0)
			{
				char[] array = stringBuilder.ToString().ToCharArray();
				Array.Reverse((Array)array);
				value = value.Substring(0, num2 + 1) + Encode(new string(array));
			}
		}
		if (missingAuthPattern.IsMatch(value))
		{
			value += "/";
		}
		return ParseUri(value, UriKind.RelativeOrAbsolute);
	}

	public static string Encode(string s)
	{
		if (s.Length == 0)
		{
			return s;
		}
		byte[] bytes = Encoding.UTF8.GetBytes(s);
		StringBuilder stringBuilder = new StringBuilder();
		byte[] array = bytes;
		for (int i = 0; i < array.Length; i++)
		{
			int num = array[i] & 0xFF;
			if (IsUnsafe(num))
			{
				stringBuilder.Append('%');
				stringBuilder.Append(hexDigits[(num >> 4) & 0xF]);
				stringBuilder.Append(hexDigits[num & 0xF]);
			}
			else
			{
				stringBuilder.Append((char)num);
			}
		}
		return stringBuilder.ToString();
	}

	private static bool IsUnsafe(int ch)
	{
		if (ch <= 128)
		{
			return char.IsWhiteSpace((char)ch);
		}
		return true;
	}

	public static PackagePartName GetRelationshipPartName(PackagePartName partName)
	{
		if (partName == null)
		{
			throw new ArgumentException("partName");
		}
		if (PACKAGE_ROOT_URI.OriginalString == partName.URI.OriginalString)
		{
			return PACKAGE_RELATIONSHIPS_ROOT_PART_NAME;
		}
		if (partName.IsRelationshipPartURI())
		{
			throw new InvalidOperationException("Can't be a relationship part");
		}
		string originalString = partName.URI.OriginalString;
		string filename = GetFilename(partName.URI);
		originalString = originalString.Substring(0, originalString.Length - filename.Length);
		originalString = Combine(originalString, RELATIONSHIP_PART_SEGMENT_NAME);
		originalString = Combine(originalString, filename);
		originalString += RELATIONSHIP_PART_EXTENSION_NAME;
		try
		{
			return CreatePartName(originalString);
		}
		catch (InvalidFormatException)
		{
			return null;
		}
	}
}
