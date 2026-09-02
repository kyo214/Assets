using System;
using System.ComponentModel;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using NPOI.OpenXml4Net.Util;

namespace NPOI.OpenXmlFormats.Dml;

[Serializable]
[DesignerCategory("code")]
[XmlType(Namespace = "http://schemas.openxmlformats.org/drawingml/2006/main")]
[XmlRoot(Namespace = "http://schemas.openxmlformats.org/drawingml/2006/main", IsNullable = true)]
public class CT_TextBodyProperties
{
	private CT_PresetTextShape prstTxWarpField;

	private CT_TextNoAutofit noAutofitField;

	private CT_TextNormalAutofit normAutofitField;

	private CT_TextShapeAutofit spAutoFitField;

	private CT_Scene3D scene3dField;

	private CT_Shape3D sp3dField;

	private CT_FlatText flatTxField;

	private CT_OfficeArtExtensionList extLstField;

	private int rotField;

	private bool rotFieldSpecified;

	private bool spcFirstLastParaField;

	private bool spcFirstLastParaFieldSpecified;

	private ST_TextVertOverflowType vertOverflowField;

	private bool vertOverflowFieldSpecified;

	private ST_TextHorzOverflowType horzOverflowField;

	private bool horzOverflowFieldSpecified;

	private ST_TextVerticalType vertField;

	private bool vertFieldSpecified;

	private ST_TextWrappingType wrapField;

	private bool wrapFieldSpecified;

	private int lInsField;

	private bool lInsFieldSpecified;

	private int tInsField;

	private bool tInsFieldSpecified;

	private int rInsField;

	private bool rInsFieldSpecified;

	private int bInsField;

	private bool bInsFieldSpecified;

	private int numColField;

	private bool numColFieldSpecified;

	private int spcColField;

	private bool spcColFieldSpecified;

	private bool rtlColField;

	private bool rtlColFieldSpecified;

	private bool fromWordArtField;

	private bool fromWordArtFieldSpecified;

	private ST_TextAnchoringType anchorField;

	private bool anchorFieldSpecified;

	private bool anchorCtrField;

	private bool anchorCtrFieldSpecified;

	private bool forceAAField;

	private bool forceAAFieldSpecified;

	private bool uprightField;

	private bool compatLnSpcField;

	private bool compatLnSpcFieldSpecified;

	public CT_PresetTextShape prstTxWarp
	{
		get
		{
			return prstTxWarpField;
		}
		set
		{
			prstTxWarpField = value;
		}
	}

	public CT_TextNoAutofit noAutofit
	{
		get
		{
			return noAutofitField;
		}
		set
		{
			noAutofitField = value;
		}
	}

	public CT_TextNormalAutofit normAutofit
	{
		get
		{
			return normAutofitField;
		}
		set
		{
			normAutofitField = value;
		}
	}

	public CT_TextShapeAutofit spAutoFit
	{
		get
		{
			return spAutoFitField;
		}
		set
		{
			spAutoFitField = value;
		}
	}

	public CT_Scene3D scene3d
	{
		get
		{
			return scene3dField;
		}
		set
		{
			scene3dField = value;
		}
	}

	public CT_Shape3D sp3d
	{
		get
		{
			return sp3dField;
		}
		set
		{
			sp3dField = value;
		}
	}

	public CT_FlatText flatTx
	{
		get
		{
			return flatTxField;
		}
		set
		{
			flatTxField = value;
		}
	}

	public CT_OfficeArtExtensionList extLst
	{
		get
		{
			return extLstField;
		}
		set
		{
			extLstField = value;
		}
	}

	[XmlAttribute]
	public int rot
	{
		get
		{
			return rotField;
		}
		set
		{
			rotField = value;
			rotFieldSpecified = true;
		}
	}

	[XmlIgnore]
	public bool rotSpecified
	{
		get
		{
			return rotFieldSpecified;
		}
		set
		{
			rotFieldSpecified = value;
		}
	}

	[XmlAttribute]
	public bool spcFirstLastPara
	{
		get
		{
			return spcFirstLastParaField;
		}
		set
		{
			spcFirstLastParaField = value;
			spcFirstLastParaFieldSpecified = value;
		}
	}

	[XmlIgnore]
	public bool spcFirstLastParaSpecified
	{
		get
		{
			return spcFirstLastParaFieldSpecified;
		}
		set
		{
			spcFirstLastParaFieldSpecified = value;
		}
	}

	[XmlAttribute]
	public ST_TextVertOverflowType vertOverflow
	{
		get
		{
			return vertOverflowField;
		}
		set
		{
			vertOverflowField = value;
			vertOverflowFieldSpecified = true;
		}
	}

	[XmlIgnore]
	public bool vertOverflowSpecified
	{
		get
		{
			return vertOverflowFieldSpecified;
		}
		set
		{
			vertOverflowFieldSpecified = value;
		}
	}

	[XmlAttribute]
	public ST_TextHorzOverflowType horzOverflow
	{
		get
		{
			return horzOverflowField;
		}
		set
		{
			horzOverflowField = value;
			horzOverflowFieldSpecified = true;
		}
	}

	[XmlIgnore]
	public bool horzOverflowSpecified
	{
		get
		{
			return horzOverflowFieldSpecified;
		}
		set
		{
			horzOverflowFieldSpecified = value;
		}
	}

	[XmlAttribute]
	public ST_TextVerticalType vert
	{
		get
		{
			return vertField;
		}
		set
		{
			vertField = value;
			vertFieldSpecified = true;
		}
	}

	[XmlIgnore]
	public bool vertSpecified
	{
		get
		{
			return vertFieldSpecified;
		}
		set
		{
			vertFieldSpecified = value;
		}
	}

	[XmlAttribute]
	public ST_TextWrappingType wrap
	{
		get
		{
			return wrapField;
		}
		set
		{
			wrapField = value;
			wrapFieldSpecified = true;
		}
	}

	[XmlIgnore]
	public bool wrapSpecified
	{
		get
		{
			return wrapFieldSpecified;
		}
		set
		{
			wrapFieldSpecified = value;
		}
	}

	[XmlAttribute]
	public int lIns
	{
		get
		{
			return lInsField;
		}
		set
		{
			lInsField = value;
			lInsFieldSpecified = true;
		}
	}

	[XmlIgnore]
	public bool lInsSpecified
	{
		get
		{
			return lInsFieldSpecified;
		}
		set
		{
			lInsFieldSpecified = value;
		}
	}

	[XmlAttribute]
	public int tIns
	{
		get
		{
			return tInsField;
		}
		set
		{
			tInsField = value;
			tInsFieldSpecified = true;
		}
	}

	[XmlIgnore]
	public bool tInsSpecified
	{
		get
		{
			return tInsFieldSpecified;
		}
		set
		{
			tInsFieldSpecified = value;
		}
	}

	[XmlAttribute]
	public int rIns
	{
		get
		{
			return rInsField;
		}
		set
		{
			rInsField = value;
			rInsFieldSpecified = true;
		}
	}

	[XmlIgnore]
	public bool rInsSpecified
	{
		get
		{
			return rInsFieldSpecified;
		}
		set
		{
			rInsFieldSpecified = value;
		}
	}

	[XmlAttribute]
	public int bIns
	{
		get
		{
			return bInsField;
		}
		set
		{
			bInsField = value;
			bInsFieldSpecified = true;
		}
	}

	[XmlIgnore]
	public bool bInsSpecified
	{
		get
		{
			return bInsFieldSpecified;
		}
		set
		{
			bInsFieldSpecified = value;
		}
	}

	[XmlAttribute]
	public int numCol
	{
		get
		{
			return numColField;
		}
		set
		{
			numColField = value;
			numColFieldSpecified = true;
		}
	}

	[XmlIgnore]
	public bool numColSpecified
	{
		get
		{
			return numColFieldSpecified;
		}
		set
		{
			numColFieldSpecified = value;
		}
	}

	[XmlAttribute]
	public int spcCol
	{
		get
		{
			return spcColField;
		}
		set
		{
			spcColField = value;
			spcColFieldSpecified = true;
		}
	}

	[XmlIgnore]
	public bool spcColSpecified
	{
		get
		{
			return spcColFieldSpecified;
		}
		set
		{
			spcColFieldSpecified = value;
		}
	}

	[XmlAttribute]
	public bool rtlCol
	{
		get
		{
			return rtlColField;
		}
		set
		{
			rtlColField = value;
			rtlColFieldSpecified = value;
		}
	}

	[XmlIgnore]
	public bool rtlColSpecified
	{
		get
		{
			return rtlColFieldSpecified;
		}
		set
		{
			rtlColFieldSpecified = value;
		}
	}

	[XmlAttribute]
	public bool fromWordArt
	{
		get
		{
			return fromWordArtField;
		}
		set
		{
			fromWordArtField = value;
			fromWordArtFieldSpecified = value;
		}
	}

	[XmlIgnore]
	public bool fromWordArtSpecified
	{
		get
		{
			return fromWordArtFieldSpecified;
		}
		set
		{
			fromWordArtFieldSpecified = value;
		}
	}

	[XmlAttribute]
	public ST_TextAnchoringType anchor
	{
		get
		{
			return anchorField;
		}
		set
		{
			anchorField = value;
			anchorFieldSpecified = true;
		}
	}

	[XmlIgnore]
	public bool anchorSpecified
	{
		get
		{
			return anchorFieldSpecified;
		}
		set
		{
			anchorFieldSpecified = value;
		}
	}

	[XmlAttribute]
	public bool anchorCtr
	{
		get
		{
			return anchorCtrField;
		}
		set
		{
			anchorCtrField = value;
			anchorCtrFieldSpecified = value;
		}
	}

	[XmlIgnore]
	public bool anchorCtrSpecified
	{
		get
		{
			return anchorCtrFieldSpecified;
		}
		set
		{
			anchorCtrFieldSpecified = value;
		}
	}

	[XmlAttribute]
	public bool forceAA
	{
		get
		{
			return forceAAField;
		}
		set
		{
			forceAAField = value;
			forceAAFieldSpecified = value;
		}
	}

	[XmlIgnore]
	public bool forceAASpecified
	{
		get
		{
			return forceAAFieldSpecified;
		}
		set
		{
			forceAAFieldSpecified = value;
		}
	}

	[XmlAttribute]
	[DefaultValue(false)]
	public bool upright
	{
		get
		{
			return uprightField;
		}
		set
		{
			uprightField = value;
		}
	}

	[XmlAttribute]
	public bool compatLnSpc
	{
		get
		{
			return compatLnSpcField;
		}
		set
		{
			compatLnSpcField = value;
			compatLnSpcFieldSpecified = value;
		}
	}

	[XmlIgnore]
	public bool compatLnSpcSpecified
	{
		get
		{
			return compatLnSpcFieldSpecified;
		}
		set
		{
			compatLnSpcFieldSpecified = value;
		}
	}

	public CT_TextBodyProperties()
	{
		uprightField = false;
		vertField = ST_TextVerticalType.horz;
		wrapField = ST_TextWrappingType.none;
		spcFirstLastParaField = false;
	}

	public static CT_TextBodyProperties Parse(XmlNode node, XmlNamespaceManager namespaceManager)
	{
		if (node == null)
		{
			return null;
		}
		CT_TextBodyProperties cT_TextBodyProperties = new CT_TextBodyProperties();
		cT_TextBodyProperties.rotFieldSpecified = node.Attributes["rot"] != null;
		cT_TextBodyProperties.rotField = XmlHelper.ReadInt(node.Attributes["rot"]);
		cT_TextBodyProperties.spcFirstLastPara = XmlHelper.ReadBool(node.Attributes["spcFirstLastPara"]);
		cT_TextBodyProperties.vertOverflowFieldSpecified = node.Attributes["vertOverflow"] != null;
		if (node.Attributes["vertOverflow"] != null)
		{
			cT_TextBodyProperties.vertOverflowField = (ST_TextVertOverflowType)Enum.Parse(typeof(ST_TextVertOverflowType), node.Attributes["vertOverflow"].Value);
		}
		cT_TextBodyProperties.horzOverflowFieldSpecified = node.Attributes["horzOverflow"] != null;
		if (node.Attributes["horzOverflow"] != null)
		{
			cT_TextBodyProperties.horzOverflowField = (ST_TextHorzOverflowType)Enum.Parse(typeof(ST_TextHorzOverflowType), node.Attributes["horzOverflow"].Value);
		}
		cT_TextBodyProperties.vertFieldSpecified = node.Attributes["vert"] != null;
		if (node.Attributes["vert"] != null)
		{
			cT_TextBodyProperties.vertField = (ST_TextVerticalType)Enum.Parse(typeof(ST_TextVerticalType), node.Attributes["vert"].Value);
		}
		cT_TextBodyProperties.wrapFieldSpecified = node.Attributes["wrap"] != null;
		if (node.Attributes["wrap"] != null)
		{
			cT_TextBodyProperties.wrapField = (ST_TextWrappingType)Enum.Parse(typeof(ST_TextWrappingType), node.Attributes["wrap"].Value);
		}
		cT_TextBodyProperties.lIns = XmlHelper.ReadInt(node.Attributes["lIns"]);
		cT_TextBodyProperties.tIns = XmlHelper.ReadInt(node.Attributes["tIns"]);
		cT_TextBodyProperties.rIns = XmlHelper.ReadInt(node.Attributes["rIns"]);
		cT_TextBodyProperties.bIns = XmlHelper.ReadInt(node.Attributes["bIns"]);
		cT_TextBodyProperties.numCol = XmlHelper.ReadInt(node.Attributes["numCol"]);
		cT_TextBodyProperties.spcCol = XmlHelper.ReadInt(node.Attributes["spcCol"]);
		cT_TextBodyProperties.rtlCol = XmlHelper.ReadBool(node.Attributes["rtlCol"]);
		cT_TextBodyProperties.fromWordArt = XmlHelper.ReadBool(node.Attributes["fromWordArt"]);
		cT_TextBodyProperties.anchorFieldSpecified = node.Attributes["anchor"] != null;
		if (node.Attributes["anchor"] != null)
		{
			cT_TextBodyProperties.anchorField = (ST_TextAnchoringType)Enum.Parse(typeof(ST_TextAnchoringType), node.Attributes["anchor"].Value);
		}
		cT_TextBodyProperties.anchorCtr = XmlHelper.ReadBool(node.Attributes["anchorCtr"]);
		cT_TextBodyProperties.forceAA = XmlHelper.ReadBool(node.Attributes["forceAA"]);
		cT_TextBodyProperties.upright = XmlHelper.ReadBool(node.Attributes["upright"]);
		cT_TextBodyProperties.compatLnSpc = XmlHelper.ReadBool(node.Attributes["compatLnSpc"]);
		foreach (XmlNode childNode in node.ChildNodes)
		{
			if (childNode.LocalName == "prstTxWarp")
			{
				cT_TextBodyProperties.prstTxWarp = CT_PresetTextShape.Parse(childNode, namespaceManager);
			}
			else if (childNode.LocalName == "noAutofit")
			{
				cT_TextBodyProperties.noAutofit = new CT_TextNoAutofit();
			}
			else if (childNode.LocalName == "normAutofit")
			{
				cT_TextBodyProperties.normAutofit = CT_TextNormalAutofit.Parse(childNode, namespaceManager);
			}
			else if (childNode.LocalName == "spAutoFit")
			{
				cT_TextBodyProperties.spAutoFit = new CT_TextShapeAutofit();
			}
			else if (childNode.LocalName == "scene3d")
			{
				cT_TextBodyProperties.scene3d = CT_Scene3D.Parse(childNode, namespaceManager);
			}
			else if (childNode.LocalName == "sp3d")
			{
				cT_TextBodyProperties.sp3d = CT_Shape3D.Parse(childNode, namespaceManager);
			}
			else if (childNode.LocalName == "flatTx")
			{
				cT_TextBodyProperties.flatTx = CT_FlatText.Parse(childNode, namespaceManager);
			}
			else if (childNode.LocalName == "extLst")
			{
				cT_TextBodyProperties.extLst = CT_OfficeArtExtensionList.Parse(childNode, namespaceManager);
			}
		}
		return cT_TextBodyProperties;
	}

	internal void Write(StreamWriter sw, string nodeName)
	{
		sw.Write($"<a:{nodeName}");
		if (rotFieldSpecified)
		{
			XmlHelper.WriteAttribute(sw, "rot", rotField, writeIfBlank: true);
		}
		if (spcFirstLastPara)
		{
			XmlHelper.WriteAttribute(sw, "spcFirstLastPara", spcFirstLastPara);
		}
		if (vertOverflowFieldSpecified)
		{
			XmlHelper.WriteAttribute(sw, "vertOverflow", vertOverflowField.ToString());
		}
		if (horzOverflowFieldSpecified)
		{
			XmlHelper.WriteAttribute(sw, "horzOverflow", horzOverflowField.ToString());
		}
		if (vertFieldSpecified)
		{
			XmlHelper.WriteAttribute(sw, "vert", vert.ToString());
		}
		if (wrapFieldSpecified && wrap != ST_TextWrappingType.none)
		{
			XmlHelper.WriteAttribute(sw, "wrap", wrap.ToString());
		}
		XmlHelper.WriteAttribute(sw, "lIns", lIns);
		XmlHelper.WriteAttribute(sw, "tIns", tIns);
		XmlHelper.WriteAttribute(sw, "rIns", rIns);
		XmlHelper.WriteAttribute(sw, "bIns", bIns);
		XmlHelper.WriteAttribute(sw, "numCol", numCol);
		XmlHelper.WriteAttribute(sw, "spcCol", spcCol);
		XmlHelper.WriteAttribute(sw, "rtlCol", rtlCol);
		XmlHelper.WriteAttribute(sw, "fromWordArt", fromWordArt, writeIfBlank: false);
		if (anchorFieldSpecified)
		{
			XmlHelper.WriteAttribute(sw, "anchor", anchorField.ToString());
		}
		XmlHelper.WriteAttribute(sw, "anchorCtr", anchorCtr, writeIfBlank: false);
		XmlHelper.WriteAttribute(sw, "forceAA", forceAA, writeIfBlank: false);
		if (upright)
		{
			XmlHelper.WriteAttribute(sw, "upright", upright);
		}
		if (compatLnSpc)
		{
			XmlHelper.WriteAttribute(sw, "compatLnSpc", compatLnSpc);
		}
		sw.Write(">");
		if (prstTxWarp != null)
		{
			prstTxWarp.Write(sw, "prstTxWarp");
		}
		if (noAutofit != null)
		{
			sw.Write("<a:noAutofit/>");
		}
		if (normAutofit != null)
		{
			normAutofit.Write(sw, "normAutofit");
		}
		if (spAutoFit != null)
		{
			sw.Write("<a:spAutoFit/>");
		}
		if (scene3d != null)
		{
			scene3d.Write(sw, "scene3d");
		}
		if (sp3d != null)
		{
			sp3d.Write(sw, "sp3d");
		}
		if (flatTx != null)
		{
			flatTx.Write(sw, "flatTx");
		}
		if (extLst != null)
		{
			extLst.Write(sw, "extLst");
		}
		sw.Write($"</a:{nodeName}>");
	}

	public void UnsetTIns()
	{
		tInsFieldSpecified = false;
	}

	public void UnsetVertOverflow()
	{
		vertOverflowFieldSpecified = false;
	}

	public void UnsetVert()
	{
		vertFieldSpecified = false;
	}

	public bool IsSetVert()
	{
		return vertFieldSpecified;
	}

	public bool IsSetBIns()
	{
		return bInsFieldSpecified;
	}

	public bool IsSetLIns()
	{
		return lInsFieldSpecified;
	}

	public bool IsSetRIns()
	{
		return rInsFieldSpecified;
	}

	public bool IsSetTIns()
	{
		return tInsFieldSpecified;
	}

	public void UnsetBIns()
	{
		bInsFieldSpecified = false;
	}

	public void UnsetLIns()
	{
		lInsFieldSpecified = false;
	}

	public void UnsetRIns()
	{
		rInsFieldSpecified = false;
	}

	public bool IsSetSpAutoFit()
	{
		return spAutoFitField != null;
	}

	public bool IsSetNoAutofit()
	{
		return noAutofitField != null;
	}

	public bool IsSetNormAutofit()
	{
		return normAutofitField != null;
	}

	public void UnsetSpAutoFit()
	{
		spAutoFitField = null;
	}

	public void UnsetNoAutofit()
	{
		noAutofitField = null;
	}

	public void UnsetNormAutofit()
	{
		normAutofitField = null;
	}

	public CT_TextNoAutofit AddNewNoAutofit()
	{
		noAutofitField = new CT_TextNoAutofit();
		return noAutofitField;
	}

	public CT_TextNormalAutofit AddNewNormAutofit()
	{
		normAutofitField = new CT_TextNormalAutofit();
		return normAutofitField;
	}

	public CT_TextShapeAutofit AddNewSpAutoFit()
	{
		spAutoFitField = new CT_TextShapeAutofit();
		return spAutoFitField;
	}

	public void UnsetHorzOverflow()
	{
		horzOverflowFieldSpecified = false;
	}

	public bool IsSetHorzOverflow()
	{
		return horzOverflowFieldSpecified;
	}

	public bool IsSetVertOverflow()
	{
		return vertOverflowFieldSpecified;
	}

	public bool IsSetAnchor()
	{
		return anchorFieldSpecified;
	}

	public void UnsetAnchor()
	{
		anchorFieldSpecified = false;
	}

	public bool IsSetWrap()
	{
		return wrapFieldSpecified;
	}

	public void UnsetWrap()
	{
		wrapFieldSpecified = false;
	}
}
