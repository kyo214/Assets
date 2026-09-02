using System;
using NPOI.OpenXml4Net.OPC;

namespace NPOI.XSSF.UserModel;

public class XSSFVBAPart : POIXMLDocumentPart
{
	public XSSFVBAPart()
	{
	}

	public XSSFVBAPart(PackagePart part)
		: base(part)
	{
	}

	[Obsolete("deprecated in POI 3.14, scheduled for removal in POI 3.16")]
	protected XSSFVBAPart(PackagePart part, PackageRelationship rel)
		: this(part)
	{
	}

	protected internal override void PrepareForCommit()
	{
	}
}
