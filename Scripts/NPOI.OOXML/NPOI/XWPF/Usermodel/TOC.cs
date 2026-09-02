using System.Text;
using NPOI.OpenXmlFormats.Wordprocessing;

namespace NPOI.XWPF.UserModel;

public class TOC
{
	private CT_SdtBlock block;

	private bool isBuilt;

	public TOC()
		: this(new CT_SdtBlock())
	{
	}

	public TOC(CT_SdtBlock block)
	{
		this.block = block;
		CT_SdtPr cT_SdtPr = block.AddNewSdtPr();
		cT_SdtPr.AddNewId().val = "4844945";
		cT_SdtPr.AddNewDocPartObj().AddNewDocPartGallery().val = "Table of Contents";
		CT_Fonts cT_Fonts = block.AddNewSdtEndPr().AddNewRPr().AddNewRFonts();
		cT_Fonts.asciiTheme = ST_Theme.minorHAnsi;
		cT_Fonts.eastAsiaTheme = ST_Theme.minorHAnsi;
		cT_Fonts.hAnsiTheme = ST_Theme.minorHAnsi;
		cT_Fonts.cstheme = ST_Theme.minorBidi;
		CT_SdtContentBlock cT_SdtContentBlock = block.AddNewSdtContent();
		CT_P cT_P = cT_SdtContentBlock.AddNewP();
		cT_P.rsidRDefault = (cT_P.rsidR = Encoding.Unicode.GetBytes("00EF7E24"));
		CT_PPr cT_PPr = cT_P.AddNewPPr();
		cT_PPr.AddNewPStyle().val = "TOCHeading";
		cT_PPr.AddNewJc().val = ST_Jc.center;
		CT_R cT_R = cT_P.AddNewR();
		cT_R.AddNewRPr().AddNewSz().val = 48uL;
		cT_R.AddNewT().Value = "Table of Contents";
		cT_R.AddNewBr().type = ST_BrType.textWrapping;
		CT_P cT_P2 = cT_SdtContentBlock.AddNewP();
		CT_PPr cT_PPr2 = cT_P2.AddNewPPr();
		cT_PPr2.AddNewPStyle().val = "TOC1";
		cT_PPr2.AddNewRPr().AddNewNoProof();
		cT_P2.AddNewR().AddNewFldChar().fldCharType = ST_FldCharType.begin;
		CT_Text cT_Text = cT_P2.AddNewR().AddNewInstrText();
		cT_Text.space = "preserve";
		cT_Text.Value = " TOC \\h \\z ";
		cT_P2.AddNewR().AddNewFldChar().fldCharType = ST_FldCharType.separate;
	}

	public CT_SdtBlock GetBlock()
	{
		return block;
	}

	public void AddRow(int level, string title, int page, string bookmarkRef)
	{
		CT_P cT_P = block.sdtContent.AddNewP();
		byte[] rsidRDefault = (cT_P.rsidR = Encoding.Unicode.GetBytes("00EF7E24"));
		cT_P.rsidRDefault = rsidRDefault;
		CT_PPr cT_PPr = cT_P.AddNewPPr();
		cT_PPr.AddNewPStyle().val = "TOC" + level;
		CT_TabStop cT_TabStop = cT_PPr.AddNewTabs().AddNewTab();
		cT_TabStop.val = ST_TabJc.right;
		cT_TabStop.leader = ST_TabTlc.dot;
		cT_TabStop.pos = "8290";
		cT_PPr.AddNewRPr().AddNewNoProof();
		CT_R cT_R = cT_P.AddNewR();
		cT_R.AddNewRPr().AddNewNoProof();
		cT_R.AddNewT().Value = title;
		CT_R cT_R2 = cT_P.AddNewR();
		cT_R2.AddNewRPr().AddNewNoProof();
		cT_R2.AddNewTab();
		CT_R cT_R3 = cT_P.AddNewR();
		cT_R3.AddNewRPr().AddNewNoProof();
		cT_R3.AddNewFldChar().fldCharType = ST_FldCharType.begin;
		CT_R cT_R4 = cT_P.AddNewR();
		cT_R4.AddNewRPr().AddNewNoProof();
		CT_Text cT_Text = cT_R4.AddNewInstrText();
		cT_Text.space = "preserve";
		cT_Text.Value = " PAGEREF _Toc" + bookmarkRef + " \\h ";
		cT_P.AddNewR().AddNewRPr().AddNewNoProof();
		CT_R cT_R5 = cT_P.AddNewR();
		cT_R5.AddNewRPr().AddNewNoProof();
		cT_R5.AddNewFldChar().fldCharType = ST_FldCharType.separate;
		CT_R cT_R6 = cT_P.AddNewR();
		cT_R6.AddNewRPr().AddNewNoProof();
		cT_R6.AddNewT().Value = page.ToString();
		CT_R cT_R7 = cT_P.AddNewR();
		cT_R7.AddNewRPr().AddNewNoProof();
		cT_R7.AddNewFldChar().fldCharType = ST_FldCharType.end;
	}

	public CT_SdtBlock Build()
	{
		if (!isBuilt)
		{
			CT_R cT_R = block.sdtContent.AddNewP().AddNewR();
			cT_R.AddNewRPr().AddNewNoProof();
			cT_R.AddNewFldChar().fldCharType = ST_FldCharType.end;
			isBuilt = true;
		}
		return block;
	}
}
