using System;
using System.Collections.Generic;
using System.IO;
using NPOI.OpenXml4Net.OPC;
using NPOI.SS.UserModel;
using NPOI.Util;

namespace NPOI.XSSF.UserModel;

public class XSSFPictureData : POIXMLDocumentPart, IPictureData
{
	internal static Dictionary<int, POIXMLRelation> RELATIONS;

	public PictureType PictureType
	{
		get
		{
			string contentType = GetPackagePart().ContentType;
			foreach (int key in RELATIONS.Keys)
			{
				if (RELATIONS[key].ContentType.Equals(contentType))
				{
					return (PictureType)key;
				}
			}
			return PictureType.None;
		}
	}

	public byte[] Data
	{
		get
		{
			try
			{
				return IOUtils.ToByteArray(GetPackagePart().GetInputStream());
			}
			catch (IOException ex)
			{
				throw new POIXMLException(ex);
			}
		}
	}

	public string MimeType => GetPackagePart().ContentType;

	static XSSFPictureData()
	{
		RELATIONS = new Dictionary<int, POIXMLRelation>(12);
		RELATIONS[2] = XSSFRelation.IMAGE_EMF;
		RELATIONS[3] = XSSFRelation.IMAGE_WMF;
		RELATIONS[4] = XSSFRelation.IMAGE_PICT;
		RELATIONS[5] = XSSFRelation.IMAGE_JPEG;
		RELATIONS[6] = XSSFRelation.IMAGE_PNG;
		RELATIONS[7] = XSSFRelation.IMAGE_DIB;
		RELATIONS[XSSFWorkbook.PICTURE_TYPE_GIF] = XSSFRelation.IMAGE_GIF;
		RELATIONS[XSSFWorkbook.PICTURE_TYPE_TIFF] = XSSFRelation.IMAGE_TIFF;
		RELATIONS[XSSFWorkbook.PICTURE_TYPE_EPS] = XSSFRelation.IMAGE_EPS;
		RELATIONS[XSSFWorkbook.PICTURE_TYPE_BMP] = XSSFRelation.IMAGE_BMP;
		RELATIONS[XSSFWorkbook.PICTURE_TYPE_WPG] = XSSFRelation.IMAGE_WPG;
		RELATIONS[XSSFWorkbook.PICTURE_TYPE_JPG] = XSSFRelation.IMAGE_JPG;
	}

	public XSSFPictureData()
	{
	}

	public XSSFPictureData(PackagePart part)
		: base(part)
	{
	}

	[Obsolete("deprecated in POI 3.14, scheduled for removal in POI 3.16")]
	internal XSSFPictureData(PackagePart part, PackageRelationship rel)
		: this(part)
	{
	}

	public string SuggestFileExtension()
	{
		return GetPackagePart().PartName.Extension;
	}

	protected internal override void PrepareForCommit()
	{
	}
}
