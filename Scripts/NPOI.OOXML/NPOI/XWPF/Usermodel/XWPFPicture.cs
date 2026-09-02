using NPOI.OpenXml4Net.OPC;
using NPOI.OpenXmlFormats.Dml;
using NPOI.OpenXmlFormats.Dml.Picture;

namespace NPOI.XWPF.UserModel;

public class XWPFPicture
{
	private CT_Picture ctPic;

	private string description;

	private XWPFRun run;

	public long Width => ctPic.spPr.xfrm.ext.cx;

	public long Height => ctPic.spPr.xfrm.ext.cy;

	public XWPFPicture(CT_Picture ctPic, XWPFRun Run)
	{
		run = Run;
		this.ctPic = ctPic;
		description = ctPic.nvPicPr.cNvPr.descr;
	}

	public void SetPictureReference(PackageRelationship rel)
	{
		ctPic.blipFill.blip.embed = rel.Id;
	}

	public CT_Picture GetCTPicture()
	{
		return ctPic;
	}

	public XWPFPictureData GetPictureData()
	{
		CT_BlipFillProperties blipFill = ctPic.blipFill;
		if (blipFill == null || !blipFill.IsSetBlip())
		{
			return null;
		}
		string embed = blipFill.blip.embed;
		POIXMLDocumentPart part = run.Parent.Part;
		if (part != null)
		{
			POIXMLDocumentPart relationById = part.GetRelationById(embed);
			if (relationById is XWPFPictureData)
			{
				return (XWPFPictureData)relationById;
			}
		}
		return null;
	}

	public string GetDescription()
	{
		return description;
	}
}
