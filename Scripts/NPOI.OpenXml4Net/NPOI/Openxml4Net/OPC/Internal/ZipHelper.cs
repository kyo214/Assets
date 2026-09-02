using System;
using System.Collections;
using System.IO;
using ICSharpCode.SharpZipLib.Zip;
using NPOI.Openxml4Net.Exceptions;
using NPOI.POIFS.Common;
using NPOI.Util;

namespace NPOI.OpenXml4Net.OPC.Internal;

public class ZipHelper
{
	private static string FORWARD_SLASH = "/";

	public static int READ_WRITE_FILE_BUFFER_SIZE = 8192;

	private ZipHelper()
	{
	}

	public static ZipEntry GetCorePropertiesZipEntry(ZipPackage pkg)
	{
		PackageRelationship relationship = pkg.GetRelationshipsByType("http://schemas.openxmlformats.org/package/2006/relationships/metadata/core-properties").GetRelationship(0);
		if (relationship == null)
		{
			return null;
		}
		return new ZipEntry(relationship.TargetUri.OriginalString);
	}

	public static ZipEntry GetContentTypeZipEntry(ZipPackage pkg)
	{
		IEnumerator entries = pkg.ZipArchive.Entries;
		while (entries.MoveNext())
		{
			ZipEntry zipEntry = (ZipEntry)entries.Current;
			if (zipEntry.Name.Equals("[Content_Types].xml"))
			{
				return zipEntry;
			}
		}
		return null;
	}

	public static string GetOPCNameFromZipItemName(string zipItemName)
	{
		if (zipItemName == null)
		{
			throw new ArgumentException("zipItemName");
		}
		if (zipItemName.StartsWith(FORWARD_SLASH))
		{
			return zipItemName;
		}
		return FORWARD_SLASH + zipItemName;
	}

	public static string GetZipItemNameFromOPCName(string opcItemName)
	{
		if (opcItemName == null)
		{
			throw new ArgumentException("opcItemName");
		}
		string text = opcItemName;
		while (text.StartsWith(FORWARD_SLASH))
		{
			text = text.Substring(1);
		}
		return text;
	}

	public static Uri GetZipURIFromOPCName(string opcItemName)
	{
		if (opcItemName == null)
		{
			throw new ArgumentException("opcItemName");
		}
		string text = opcItemName;
		while (text.StartsWith(FORWARD_SLASH))
		{
			text = text.Substring(1);
		}
		try
		{
			return PackagingUriHelper.ParseUri(text, UriKind.RelativeOrAbsolute);
		}
		catch (UriFormatException)
		{
			return null;
		}
	}

	public static void VerifyZipHeader(InputStream stream)
	{
		byte[] array = new byte[8];
		IOUtils.ReadFully(stream, array);
		if (LittleEndian.GetLong(array) == -2226271756974174256L)
		{
			throw new OLE2NotOfficeXmlFileException("The supplied data appears to be in the OLE2 Format. You are calling the part of POI that deals with OOXML (Office Open XML) Documents. You need to call a different part of POI to process this data (eg HSSF instead of XSSF)");
		}
		byte[] rAW_XML_FILE_HEADER = POIFSConstants.RAW_XML_FILE_HEADER;
		if (array[0] == rAW_XML_FILE_HEADER[0] && array[1] == rAW_XML_FILE_HEADER[1] && array[2] == rAW_XML_FILE_HEADER[2] && array[3] == rAW_XML_FILE_HEADER[3] && array[4] == rAW_XML_FILE_HEADER[4])
		{
			throw new NotOfficeXmlFileException("The supplied data appears to be a raw XML file. Formats such as Office 2003 XML are not supported");
		}
		if (stream is PushbackInputStream)
		{
			((PushbackInputStream)stream).Unread(array);
		}
		else if (stream.MarkSupported())
		{
			stream.Reset();
		}
	}

	private static InputStream PrepareToCheckHeader(InputStream stream)
	{
		if (stream is PushbackInputStream)
		{
			return stream;
		}
		if (stream.MarkSupported())
		{
			stream.Mark(8);
			return stream;
		}
		return new PushbackInputStream(stream, 8);
	}

	public static ZipInputStream OpenZipStream(Stream stream)
	{
		return new ZipInputStream(stream);
	}

	public static ZipFile OpenZipFile(FileInfo file)
	{
		if (!file.Exists)
		{
			throw new FileNotFoundException("File does not exist");
		}
		FileInputStream fileInputStream = new FileInputStream(file.OpenRead());
		try
		{
			VerifyZipHeader(fileInputStream);
		}
		finally
		{
			fileInputStream.Close();
		}
		return new ZipFile(File.OpenRead(file.FullName));
	}

	public static ZipFile OpenZipFile(string path)
	{
		return OpenZipFile(new FileInfo(path));
	}
}
