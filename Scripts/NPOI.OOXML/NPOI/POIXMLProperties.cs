using System;
using System.IO;
using NPOI.OpenXml4Net.Exceptions;
using NPOI.OpenXml4Net.OPC;
using NPOI.OpenXml4Net.OPC.Internal;
using NPOI.OpenXmlFormats;

namespace NPOI;

public class POIXMLProperties
{
	private OPCPackage pkg;

	private CoreProperties core;

	private ExtendedProperties ext;

	private CustomProperties cust;

	private PackagePart extPart;

	private PackagePart custPart;

	private static ExtendedPropertiesDocument NEW_EXT_INSTANCE;

	private static CustomPropertiesDocument NEW_CUST_INSTANCE;

	public CoreProperties CoreProperties => core;

	public ExtendedProperties ExtendedProperties => ext;

	public CustomProperties CustomProperties => cust;

	protected internal PackagePart ThumbnailPart
	{
		get
		{
			PackageRelationshipCollection relationshipsByType = pkg.GetRelationshipsByType("http://schemas.openxmlformats.org/package/2006/relationships/metadata/thumbnail");
			if (relationshipsByType.Size == 1)
			{
				return pkg.GetPart(relationshipsByType.GetRelationship(0));
			}
			return null;
		}
	}

	public string ThumbnailFilename
	{
		get
		{
			PackagePart thumbnailPart = ThumbnailPart;
			if (thumbnailPart == null)
			{
				return null;
			}
			string name = thumbnailPart.PartName.Name;
			return name.Substring(name.LastIndexOf('/') + 1);
		}
	}

	public Stream ThumbnailImage => ThumbnailPart?.GetInputStream();

	static POIXMLProperties()
	{
		NEW_EXT_INSTANCE = new ExtendedPropertiesDocument();
		NEW_EXT_INSTANCE.AddNewProperties();
		NEW_CUST_INSTANCE = new CustomPropertiesDocument();
		NEW_CUST_INSTANCE.AddNewProperties();
	}

	public POIXMLProperties(OPCPackage docPackage)
	{
		pkg = docPackage;
		core = new CoreProperties((PackagePropertiesPart)pkg.GetPackageProperties());
		PackageRelationshipCollection relationshipsByType = pkg.GetRelationshipsByType("http://schemas.openxmlformats.org/officeDocument/2006/relationships/extended-properties");
		if (relationshipsByType.Size == 1)
		{
			extPart = pkg.GetPart(relationshipsByType.GetRelationship(0));
			ExtendedPropertiesDocument props = ExtendedPropertiesDocument.Parse(extPart.GetInputStream());
			ext = new ExtendedProperties(props);
		}
		else
		{
			extPart = null;
			ext = new ExtendedProperties(NEW_EXT_INSTANCE.Copy());
		}
		PackageRelationshipCollection relationshipsByType2 = pkg.GetRelationshipsByType("http://schemas.openxmlformats.org/officeDocument/2006/relationships/custom-properties");
		if (relationshipsByType2.Size == 1)
		{
			custPart = pkg.GetPart(relationshipsByType2.GetRelationship(0));
			CustomPropertiesDocument props2 = CustomPropertiesDocument.Parse(custPart.GetInputStream());
			cust = new CustomProperties(props2);
		}
		else
		{
			custPart = null;
			cust = new CustomProperties(NEW_CUST_INSTANCE.Copy());
		}
	}

	public void SetThumbnail(string filename, Stream imageData)
	{
		PackagePart thumbnailPart = ThumbnailPart;
		if (thumbnailPart == null)
		{
			pkg.AddThumbnail(filename, imageData);
			return;
		}
		string contentTypeFromFileExtension = ContentTypes.GetContentTypeFromFileExtension(filename);
		if (!contentTypeFromFileExtension.Equals(thumbnailPart.ContentType))
		{
			throw new ArgumentException("Can't set a Thumbnail of type " + contentTypeFromFileExtension + " when existing one is of a different type " + thumbnailPart.ContentType);
		}
		StreamHelper.CopyStream(imageData, thumbnailPart.GetOutputStream());
	}

	public virtual void Commit()
	{
		if (extPart == null && !NEW_EXT_INSTANCE.ToString().Equals(ext.props.ToString()))
		{
			try
			{
				PackagePartName packagePartName = PackagingUriHelper.CreatePartName("/docProps/app.xml");
				pkg.AddRelationship(packagePartName, TargetMode.Internal, "http://schemas.openxmlformats.org/officeDocument/2006/relationships/extended-properties");
				extPart = pkg.CreatePart(packagePartName, "application/vnd.openxmlformats-officedocument.extended-properties+xml");
			}
			catch (InvalidFormatException ex)
			{
				throw new POIXMLException(ex);
			}
		}
		if (custPart == null && !NEW_CUST_INSTANCE.ToString().Equals(cust.props.ToString()))
		{
			try
			{
				PackagePartName packagePartName2 = PackagingUriHelper.CreatePartName("/docProps/custom.xml");
				pkg.AddRelationship(packagePartName2, TargetMode.Internal, "http://schemas.openxmlformats.org/officeDocument/2006/relationships/custom-properties");
				custPart = pkg.CreatePart(packagePartName2, "application/vnd.openxmlformats-officedocument.custom-properties+xml");
			}
			catch (InvalidFormatException ex2)
			{
				throw new POIXMLException(ex2);
			}
		}
		if (extPart != null)
		{
			Stream outputStream = extPart.GetOutputStream();
			if (extPart.Size > 0)
			{
				extPart.Clear();
			}
			ext.props.Save(outputStream);
			outputStream.Close();
		}
		if (custPart != null)
		{
			Stream outputStream2 = custPart.GetOutputStream();
			cust.props.Save(outputStream2);
			outputStream2.Close();
		}
	}
}
