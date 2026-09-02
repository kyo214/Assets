using System;
using System.IO;
using NPOI.OpenXml4Net.OPC;
using NPOI.OpenXmlFormats.Wordprocessing;

namespace NPOI.XWPF.UserModel;

public class XWPFFooter : XWPFHeaderFooter
{
	public override BodyType PartType => BodyType.FOOTER;

	public XWPFFooter()
	{
		headerFooter = new CT_Ftr();
		ReadHdrFtr();
	}

	public XWPFFooter(XWPFDocument doc, CT_HdrFtr hdrFtr)
		: base(doc, hdrFtr)
	{
		foreach (object item3 in hdrFtr.Items)
		{
			if (item3 is CT_P)
			{
				XWPFParagraph item = new XWPFParagraph((CT_P)item3, this);
				paragraphs.Add(item);
			}
			if (item3 is CT_Tbl)
			{
				XWPFTable item2 = new XWPFTable((CT_Tbl)item3, this);
				tables.Add(item2);
			}
		}
	}

	public XWPFFooter(POIXMLDocumentPart parent, PackagePart part)
		: base(parent, part)
	{
	}

	[Obsolete("deprecated in POI 3.14, scheduled for removal in POI 3.16")]
	public XWPFFooter(POIXMLDocumentPart parent, PackagePart part, PackageRelationship rel)
		: this(parent, part)
	{
	}

	protected internal override void Commit()
	{
		using Stream stream = GetPackagePart().GetOutputStream();
		new FtrDocument((CT_Ftr)headerFooter).Save(stream);
	}

	internal override void OnDocumentRead()
	{
		base.OnDocumentRead();
		FtrDocument ftrDocument = null;
		try
		{
			ftrDocument = FtrDocument.Parse(POIXMLDocumentPart.ConvertStreamToXml(GetPackagePart().GetInputStream()), POIXMLDocumentPart.NamespaceManager);
			headerFooter = ftrDocument.Ftr;
			foreach (object item4 in headerFooter.Items)
			{
				if (item4 is CT_P)
				{
					XWPFParagraph item = new XWPFParagraph((CT_P)item4, this);
					paragraphs.Add(item);
					bodyElements.Add(item);
				}
				if (item4 is CT_Tbl)
				{
					XWPFTable item2 = new XWPFTable((CT_Tbl)item4, this);
					tables.Add(item2);
					bodyElements.Add(item2);
				}
				if (item4 is CT_SdtBlock)
				{
					XWPFSDT item3 = new XWPFSDT((CT_SdtBlock)item4, this);
					bodyElements.Add(item3);
				}
			}
		}
		catch (Exception ex)
		{
			throw new POIXMLException(ex);
		}
	}
}
