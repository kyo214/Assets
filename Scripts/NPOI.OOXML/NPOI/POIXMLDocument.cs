using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using NPOI.OpenXml4Net;
using NPOI.OpenXml4Net.Exceptions;
using NPOI.OpenXml4Net.OPC;
using NPOI.POIFS.FileSystem;
using NPOI.Util;

namespace NPOI;

public abstract class POIXMLDocument : POIXMLDocumentPart, ICloseable
{
	public static string DOCUMENT_CREATOR = "NPOI";

	public static string OLE_OBJECT_REL_TYPE = "http://schemas.openxmlformats.org/officeDocument/2006/relationships/oleObject";

	public static string PACK_OBJECT_REL_TYPE = "http://schemas.openxmlformats.org/officeDocument/2006/relationships/package";

	private OPCPackage pkg;

	private POIXMLProperties properties;

	public OPCPackage Package => pkg;

	protected PackagePart CorePart => GetPackagePart();

	protected POIXMLDocument(OPCPackage pkg)
		: base(pkg)
	{
		init(pkg);
	}

	protected POIXMLDocument(OPCPackage pkg, string coreDocumentRel)
		: base(pkg, coreDocumentRel)
	{
		init(pkg);
	}

	private void init(OPCPackage pkg)
	{
		this.pkg = pkg;
	}

	public static OPCPackage OpenPackage(string path)
	{
		try
		{
			return OPCPackage.Open(path);
		}
		catch (InvalidFormatException ex)
		{
			throw new IOException(ex.ToString());
		}
	}

	protected PackagePart[] GetRelatedByType(string contentType)
	{
		PackageRelationshipCollection relationshipsByType = GetPackagePart().GetRelationshipsByType(contentType);
		PackagePart[] array = new PackagePart[relationshipsByType.Size];
		int num = 0;
		foreach (PackageRelationship item in relationshipsByType)
		{
			array[num] = GetPackagePart().GetRelatedPart(item);
			num++;
		}
		return array;
	}

	[Obsolete("Use the method from DocumentFactoryHelper")]
	public static bool HasOOXMLHeader(Stream inp)
	{
		return DocumentFactoryHelper.HasOOXMLHeader(inp);
	}

	public POIXMLProperties GetProperties()
	{
		if (properties == null)
		{
			try
			{
				properties = new POIXMLProperties(pkg);
			}
			catch (Exception ex)
			{
				throw new POIXMLException(ex);
			}
		}
		return properties;
	}

	public abstract List<PackagePart> GetAllEmbedds();

	protected void Load(POIXMLFactory factory)
	{
		Dictionary<PackagePart, POIXMLDocumentPart> dictionary = new Dictionary<PackagePart, POIXMLDocumentPart>();
		try
		{
			Read(factory, dictionary);
		}
		catch (OpenXml4NetException ex)
		{
			throw new POIXMLException(ex);
		}
		OnDocumentRead();
		dictionary.Clear();
	}

	public void Close()
	{
		if (pkg != null)
		{
			if (pkg.GetPackageAccess() == PackageAccess.READ)
			{
				pkg.Revert();
			}
			else
			{
				pkg.Close();
			}
			pkg = null;
		}
	}

	public void Write(Stream stream)
	{
		OPCPackage obj = Package ?? throw new IOException("Cannot write data, document seems to have been closed already");
		if (!GetProperties().CustomProperties.Contains("Generator"))
		{
			GetProperties().CustomProperties.AddProperty("Generator", "NPOI");
		}
		if (!GetProperties().CustomProperties.Contains("Generator Version"))
		{
			GetProperties().CustomProperties.AddProperty("Generator Version", Assembly.GetExecutingAssembly().GetName().Version.ToString(3));
		}
		List<PackagePart> list = new List<PackagePart>();
		OnSave(list);
		list.Clear();
		GetProperties().Commit();
		obj.Save(stream);
	}
}
