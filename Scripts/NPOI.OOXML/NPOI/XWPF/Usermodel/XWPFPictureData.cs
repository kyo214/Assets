using System;
using System.IO;
using NPOI.OpenXml4Net.OPC;
using NPOI.Util;

namespace NPOI.XWPF.UserModel;

public class XWPFPictureData : POIXMLDocumentPart
{
	internal static POIXMLRelation[] RELATIONS;

	private long? checksum;

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

	public string FileName
	{
		get
		{
			string name = GetPackagePart().PartName.Name;
			return name.Substring(name.LastIndexOf('/') + 1);
		}
	}

	public long Checksum
	{
		get
		{
			if (!checksum.HasValue)
			{
				Stream stream = null;
				byte[] data;
				try
				{
					stream = GetPackagePart().GetInputStream();
					data = IOUtils.ToByteArray(stream);
				}
				catch (IOException ex)
				{
					throw new POIXMLException(ex);
				}
				finally
				{
					try
					{
						stream?.Close();
					}
					catch (IOException ex2)
					{
						throw new POIXMLException(ex2);
					}
				}
				checksum = IOUtils.CalculateChecksum(data);
			}
			return checksum.Value;
		}
	}

	static XWPFPictureData()
	{
		RELATIONS = new POIXMLRelation[14];
		RELATIONS[2] = XWPFRelation.IMAGE_EMF;
		RELATIONS[3] = XWPFRelation.IMAGE_WMF;
		RELATIONS[4] = XWPFRelation.IMAGE_PICT;
		RELATIONS[5] = XWPFRelation.IMAGE_JPEG;
		RELATIONS[6] = XWPFRelation.IMAGE_PNG;
		RELATIONS[7] = XWPFRelation.IMAGE_DIB;
		RELATIONS[8] = XWPFRelation.IMAGE_GIF;
		RELATIONS[9] = XWPFRelation.IMAGE_TIFF;
		RELATIONS[10] = XWPFRelation.IMAGE_EPS;
		RELATIONS[11] = XWPFRelation.IMAGE_BMP;
		RELATIONS[12] = XWPFRelation.IMAGE_WPG;
		RELATIONS[13] = XWPFRelation.IMAGE_SVG;
	}

	protected XWPFPictureData()
	{
	}

	public XWPFPictureData(PackagePart part)
		: base(part)
	{
	}

	[Obsolete("deprecated in POI 3.14, scheduled for removal in POI 3.16")]
	public XWPFPictureData(PackagePart part, PackageRelationship rel)
		: this(part)
	{
	}

	internal override void OnDocumentRead()
	{
		base.OnDocumentRead();
	}

	public string SuggestFileExtension()
	{
		return GetPackagePart().PartName.Extension;
	}

	public int GetPictureType()
	{
		string contentType = GetPackagePart().ContentType;
		for (int i = 0; i < RELATIONS.Length; i++)
		{
			if (RELATIONS[i] != null && RELATIONS[i].ContentType.Equals(contentType))
			{
				return i;
			}
		}
		return 0;
	}

	public override bool Equals(object obj)
	{
		if (obj == this)
		{
			return true;
		}
		if (obj == null)
		{
			return false;
		}
		if (!(obj is XWPFPictureData))
		{
			return false;
		}
		XWPFPictureData xWPFPictureData = (XWPFPictureData)obj;
		PackagePart packagePart = xWPFPictureData.GetPackagePart();
		PackagePart packagePart2 = GetPackagePart();
		if ((packagePart != null && packagePart2 == null) || (packagePart == null && packagePart2 != null))
		{
			return false;
		}
		if (packagePart2 != null)
		{
			OPCPackage package = packagePart.Package;
			OPCPackage package2 = packagePart2.Package;
			if ((package != null && package2 == null) || (package == null && package2 != null))
			{
				return false;
			}
			if (package2 != null && !package2.Equals(package))
			{
				return false;
			}
		}
		long obj2 = xWPFPictureData.Checksum;
		if (!Checksum.Equals(obj2))
		{
			return false;
		}
		return Arrays.Equals(Data, xWPFPictureData.Data);
	}

	public override int GetHashCode()
	{
		return Checksum.GetHashCode();
	}

	protected internal override void PrepareForCommit()
	{
	}
}
