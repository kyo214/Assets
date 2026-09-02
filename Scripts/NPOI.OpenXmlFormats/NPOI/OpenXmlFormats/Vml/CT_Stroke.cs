using System;
using System.ComponentModel;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using NPOI.OpenXml4Net.Util;
using NPOI.OpenXmlFormats.Util;

namespace NPOI.OpenXmlFormats.Vml;

[Serializable]
[DesignerCategory("code")]
[XmlType(Namespace = "urn:schemas-microsoft-com:vml")]
[XmlRoot(Namespace = "urn:schemas-microsoft-com:vml", IsNullable = true)]
public class CT_Stroke
{
	private string idField;

	private ST_TrueFalse onField;

	private bool onFieldSpecified;

	private string weightField;

	private string colorField;

	private string opacityField;

	private ST_StrokeLineStyle linestyleField;

	private bool linestyleFieldSpecified;

	private decimal miterlimitField;

	private bool miterlimitFieldSpecified;

	private ST_StrokeJoinStyle joinstyleField;

	private bool joinstyleFieldSpecified;

	private ST_StrokeEndCap endcapField;

	private bool endcapFieldSpecified;

	private string dashstyleField;

	private ST_FillType filltypeField;

	private bool filltypeFieldSpecified;

	private string srcField;

	private ST_ImageAspect imageaspectField;

	private bool imageaspectFieldSpecified;

	private string imagesizeField;

	private ST_TrueFalse imagealignshapeField;

	private bool imagealignshapeFieldSpecified;

	private string color2Field;

	private ST_StrokeArrowType startarrowField;

	private bool startarrowFieldSpecified;

	private ST_StrokeArrowWidth startarrowwidthField;

	private bool startarrowwidthFieldSpecified;

	private ST_StrokeArrowLength startarrowlengthField;

	private bool startarrowlengthFieldSpecified;

	private ST_StrokeArrowType endarrowField;

	private bool endarrowFieldSpecified;

	private ST_StrokeArrowWidth endarrowwidthField;

	private bool endarrowwidthFieldSpecified;

	private ST_StrokeArrowLength endarrowlengthField;

	private bool endarrowlengthFieldSpecified;

	private ST_TrueFalse insetpenField;

	private bool insetpenFieldSpecified;

	[XmlAttribute]
	public string id
	{
		get
		{
			return idField;
		}
		set
		{
			idField = value;
		}
	}

	[XmlAttribute]
	public ST_TrueFalse on
	{
		get
		{
			return onField;
		}
		set
		{
			onField = value;
		}
	}

	[XmlIgnore]
	public bool onSpecified
	{
		get
		{
			return onFieldSpecified;
		}
		set
		{
			onFieldSpecified = value;
		}
	}

	[XmlAttribute]
	public string weight
	{
		get
		{
			return weightField;
		}
		set
		{
			weightField = value;
		}
	}

	[XmlAttribute]
	public string color
	{
		get
		{
			return colorField;
		}
		set
		{
			colorField = value;
		}
	}

	[XmlAttribute]
	public string opacity
	{
		get
		{
			return opacityField;
		}
		set
		{
			opacityField = value;
		}
	}

	[XmlAttribute]
	public ST_StrokeLineStyle linestyle
	{
		get
		{
			return linestyleField;
		}
		set
		{
			linestyleField = value;
			linestyleFieldSpecified = true;
		}
	}

	[XmlIgnore]
	public bool linestyleSpecified
	{
		get
		{
			return linestyleFieldSpecified;
		}
		set
		{
			linestyleFieldSpecified = value;
		}
	}

	[XmlAttribute]
	public decimal miterlimit
	{
		get
		{
			return miterlimitField;
		}
		set
		{
			miterlimitField = value;
			miterlimitFieldSpecified = true;
		}
	}

	[XmlIgnore]
	public bool miterlimitSpecified
	{
		get
		{
			return miterlimitFieldSpecified;
		}
		set
		{
			miterlimitFieldSpecified = value;
		}
	}

	[XmlAttribute]
	public ST_StrokeJoinStyle joinstyle
	{
		get
		{
			return joinstyleField;
		}
		set
		{
			joinstyleField = value;
			joinstyleFieldSpecified = true;
		}
	}

	[XmlIgnore]
	public bool joinstyleSpecified
	{
		get
		{
			return joinstyleFieldSpecified;
		}
		set
		{
			joinstyleFieldSpecified = value;
		}
	}

	[XmlAttribute]
	public ST_StrokeEndCap endcap
	{
		get
		{
			return endcapField;
		}
		set
		{
			endcapField = value;
			endcapFieldSpecified = true;
		}
	}

	[XmlIgnore]
	public bool endcapSpecified
	{
		get
		{
			return endcapFieldSpecified;
		}
		set
		{
			endcapFieldSpecified = value;
		}
	}

	[XmlAttribute]
	public string dashstyle
	{
		get
		{
			return dashstyleField;
		}
		set
		{
			dashstyleField = value;
		}
	}

	[XmlAttribute]
	public ST_FillType filltype
	{
		get
		{
			return filltypeField;
		}
		set
		{
			filltypeField = value;
			filltypeFieldSpecified = true;
		}
	}

	[XmlIgnore]
	public bool filltypeSpecified
	{
		get
		{
			return filltypeFieldSpecified;
		}
		set
		{
			filltypeFieldSpecified = value;
		}
	}

	[XmlAttribute]
	public string src
	{
		get
		{
			return srcField;
		}
		set
		{
			srcField = value;
		}
	}

	[XmlAttribute]
	public ST_ImageAspect imageaspect
	{
		get
		{
			return imageaspectField;
		}
		set
		{
			imageaspectField = value;
			imageaspectFieldSpecified = true;
		}
	}

	[XmlIgnore]
	public bool imageaspectSpecified
	{
		get
		{
			return imageaspectFieldSpecified;
		}
		set
		{
			imageaspectFieldSpecified = value;
		}
	}

	[XmlAttribute]
	public string imagesize
	{
		get
		{
			return imagesizeField;
		}
		set
		{
			imagesizeField = value;
		}
	}

	[XmlAttribute]
	public ST_TrueFalse imagealignshape
	{
		get
		{
			return imagealignshapeField;
		}
		set
		{
			imagealignshapeField = value;
			imagealignshapeFieldSpecified = true;
		}
	}

	[XmlIgnore]
	public bool imagealignshapeSpecified
	{
		get
		{
			return imagealignshapeFieldSpecified;
		}
		set
		{
			imagealignshapeFieldSpecified = value;
		}
	}

	[XmlAttribute]
	public string color2
	{
		get
		{
			return color2Field;
		}
		set
		{
			color2Field = value;
		}
	}

	[XmlAttribute]
	public ST_StrokeArrowType startarrow
	{
		get
		{
			return startarrowField;
		}
		set
		{
			startarrowField = value;
			startarrowFieldSpecified = true;
		}
	}

	[XmlIgnore]
	public bool startarrowSpecified
	{
		get
		{
			return startarrowFieldSpecified;
		}
		set
		{
			startarrowFieldSpecified = value;
		}
	}

	[XmlAttribute]
	public ST_StrokeArrowWidth startarrowwidth
	{
		get
		{
			return startarrowwidthField;
		}
		set
		{
			startarrowwidthField = value;
			startarrowwidthFieldSpecified = true;
		}
	}

	[XmlIgnore]
	public bool startarrowwidthSpecified
	{
		get
		{
			return startarrowwidthFieldSpecified;
		}
		set
		{
			startarrowwidthFieldSpecified = value;
		}
	}

	[XmlAttribute]
	public ST_StrokeArrowLength startarrowlength
	{
		get
		{
			return startarrowlengthField;
		}
		set
		{
			startarrowlengthField = value;
			startarrowlengthFieldSpecified = true;
		}
	}

	[XmlIgnore]
	public bool startarrowlengthSpecified
	{
		get
		{
			return startarrowlengthFieldSpecified;
		}
		set
		{
			startarrowlengthFieldSpecified = value;
		}
	}

	[XmlAttribute]
	public ST_StrokeArrowType endarrow
	{
		get
		{
			return endarrowField;
		}
		set
		{
			endarrowField = value;
			endarrowFieldSpecified = true;
		}
	}

	[XmlIgnore]
	public bool endarrowSpecified
	{
		get
		{
			return endarrowFieldSpecified;
		}
		set
		{
			endarrowFieldSpecified = value;
		}
	}

	[XmlAttribute]
	public ST_StrokeArrowWidth endarrowwidth
	{
		get
		{
			return endarrowwidthField;
		}
		set
		{
			endarrowwidthField = value;
			endarrowwidthFieldSpecified = true;
		}
	}

	[XmlIgnore]
	public bool endarrowwidthSpecified
	{
		get
		{
			return endarrowwidthFieldSpecified;
		}
		set
		{
			endarrowwidthFieldSpecified = value;
		}
	}

	[XmlAttribute]
	public ST_StrokeArrowLength endarrowlength
	{
		get
		{
			return endarrowlengthField;
		}
		set
		{
			endarrowlengthField = value;
			endarrowlengthFieldSpecified = true;
		}
	}

	[XmlIgnore]
	public bool endarrowlengthSpecified
	{
		get
		{
			return endarrowlengthFieldSpecified;
		}
		set
		{
			endarrowlengthFieldSpecified = value;
		}
	}

	[XmlAttribute]
	public ST_TrueFalse insetpen
	{
		get
		{
			return insetpenField;
		}
		set
		{
			insetpenField = value;
			insetpenFieldSpecified = true;
		}
	}

	[XmlIgnore]
	public bool insetpenSpecified
	{
		get
		{
			return insetpenFieldSpecified;
		}
		set
		{
			insetpenFieldSpecified = value;
		}
	}

	public static CT_Stroke Parse(XmlNode node, XmlNamespaceManager namespaceManager)
	{
		if (node == null)
		{
			return null;
		}
		CT_Stroke cT_Stroke = new CT_Stroke();
		cT_Stroke.id = NPOI.OpenXml4Net.Util.XmlHelper.ReadString(node.Attributes["id"]);
		if (node.Attributes["on"] != null)
		{
			cT_Stroke.on = NPOI.OpenXmlFormats.Util.XmlHelper.ReadTrueFalse2(node.Attributes["on"]);
		}
		cT_Stroke.weight = NPOI.OpenXml4Net.Util.XmlHelper.ReadString(node.Attributes["weight"]);
		cT_Stroke.color = NPOI.OpenXml4Net.Util.XmlHelper.ReadString(node.Attributes["color"]);
		cT_Stroke.opacity = NPOI.OpenXml4Net.Util.XmlHelper.ReadString(node.Attributes["opacity"]);
		if (node.Attributes["linestyle"] != null)
		{
			cT_Stroke.linestyle = (ST_StrokeLineStyle)Enum.Parse(typeof(ST_StrokeLineStyle), node.Attributes["linestyle"].Value);
			cT_Stroke.linestyleFieldSpecified = true;
		}
		else
		{
			cT_Stroke.linestyleFieldSpecified = false;
		}
		if (node.Attributes["miterlimit"] != null)
		{
			cT_Stroke.miterlimit = NPOI.OpenXml4Net.Util.XmlHelper.ReadDecimal(node.Attributes["miterlimit"]);
			cT_Stroke.miterlimitFieldSpecified = true;
		}
		else
		{
			cT_Stroke.miterlimitFieldSpecified = false;
		}
		if (node.Attributes["joinstyle"] != null)
		{
			cT_Stroke.joinstyleFieldSpecified = true;
			cT_Stroke.joinstyle = (ST_StrokeJoinStyle)Enum.Parse(typeof(ST_StrokeJoinStyle), node.Attributes["joinstyle"].Value);
		}
		else
		{
			cT_Stroke.joinstyleFieldSpecified = false;
		}
		if (node.Attributes["endcap"] != null)
		{
			cT_Stroke.endcap = (ST_StrokeEndCap)Enum.Parse(typeof(ST_StrokeEndCap), node.Attributes["endcap"].Value);
			cT_Stroke.endcapFieldSpecified = true;
		}
		else
		{
			cT_Stroke.endcapFieldSpecified = false;
		}
		cT_Stroke.dashstyle = NPOI.OpenXml4Net.Util.XmlHelper.ReadString(node.Attributes["dashstyle"]);
		if (node.Attributes["filltype"] != null)
		{
			cT_Stroke.filltype = (ST_FillType)Enum.Parse(typeof(ST_FillType), node.Attributes["filltype"].Value);
			cT_Stroke.filltypeFieldSpecified = true;
		}
		else
		{
			cT_Stroke.filltypeFieldSpecified = false;
		}
		cT_Stroke.src = NPOI.OpenXml4Net.Util.XmlHelper.ReadString(node.Attributes["src"]);
		if (node.Attributes["imageaspect"] != null)
		{
			cT_Stroke.imageaspect = (ST_ImageAspect)Enum.Parse(typeof(ST_ImageAspect), node.Attributes["imageaspect"].Value);
			cT_Stroke.imageaspectFieldSpecified = true;
		}
		else
		{
			cT_Stroke.imageaspectFieldSpecified = false;
		}
		cT_Stroke.imagesize = NPOI.OpenXml4Net.Util.XmlHelper.ReadString(node.Attributes["imagesize"]);
		if (node.Attributes["imagealignshape"] != null)
		{
			cT_Stroke.imagealignshape = NPOI.OpenXmlFormats.Util.XmlHelper.ReadTrueFalse2(node.Attributes["imagealignshape"]);
			cT_Stroke.imagealignshapeSpecified = true;
		}
		else
		{
			cT_Stroke.imagealignshapeSpecified = false;
		}
		cT_Stroke.color2 = NPOI.OpenXml4Net.Util.XmlHelper.ReadString(node.Attributes["color2"]);
		if (node.Attributes["startarrow"] != null)
		{
			cT_Stroke.startarrow = (ST_StrokeArrowType)Enum.Parse(typeof(ST_StrokeArrowType), node.Attributes["startarrow"].Value);
			cT_Stroke.startarrowFieldSpecified = true;
		}
		else
		{
			cT_Stroke.startarrowFieldSpecified = false;
		}
		if (node.Attributes["startarrowwidth"] != null)
		{
			cT_Stroke.startarrowwidth = (ST_StrokeArrowWidth)Enum.Parse(typeof(ST_StrokeArrowWidth), node.Attributes["startarrowwidth"].Value);
			cT_Stroke.startarrowwidthFieldSpecified = true;
		}
		else
		{
			cT_Stroke.startarrowwidthFieldSpecified = false;
		}
		if (node.Attributes["startarrowlength"] != null)
		{
			cT_Stroke.startarrowlength = (ST_StrokeArrowLength)Enum.Parse(typeof(ST_StrokeArrowLength), node.Attributes["startarrowlength"].Value);
			cT_Stroke.startarrowlengthFieldSpecified = true;
		}
		else
		{
			cT_Stroke.startarrowlengthFieldSpecified = false;
		}
		if (node.Attributes["endarrow"] != null)
		{
			cT_Stroke.endarrow = (ST_StrokeArrowType)Enum.Parse(typeof(ST_StrokeArrowType), node.Attributes["endarrow"].Value);
			cT_Stroke.endarrowFieldSpecified = true;
		}
		else
		{
			cT_Stroke.endarrowFieldSpecified = false;
		}
		if (node.Attributes["endarrowwidth"] != null)
		{
			cT_Stroke.endarrowwidth = (ST_StrokeArrowWidth)Enum.Parse(typeof(ST_StrokeArrowWidth), node.Attributes["endarrowwidth"].Value);
			cT_Stroke.endarrowwidthFieldSpecified = true;
		}
		else
		{
			cT_Stroke.endarrowwidthFieldSpecified = false;
		}
		if (node.Attributes["endarrowlength"] != null)
		{
			cT_Stroke.endarrowlength = (ST_StrokeArrowLength)Enum.Parse(typeof(ST_StrokeArrowLength), node.Attributes["endarrowlength"].Value);
			cT_Stroke.endarrowlengthFieldSpecified = true;
		}
		else
		{
			cT_Stroke.endarrowlengthFieldSpecified = false;
		}
		if (node.Attributes["insetpen"] != null)
		{
			cT_Stroke.insetpen = NPOI.OpenXmlFormats.Util.XmlHelper.ReadTrueFalse2(node.Attributes["insetpen"]);
		}
		return cT_Stroke;
	}

	internal void Write(StreamWriter sw, string nodeName)
	{
		sw.Write($"<v:{nodeName}");
		NPOI.OpenXml4Net.Util.XmlHelper.WriteAttribute(sw, "id", id);
		NPOI.OpenXmlFormats.Util.XmlHelper.WriteAttribute(sw, "on", on);
		NPOI.OpenXml4Net.Util.XmlHelper.WriteAttribute(sw, "weight", weight);
		NPOI.OpenXml4Net.Util.XmlHelper.WriteAttribute(sw, "color", color);
		NPOI.OpenXml4Net.Util.XmlHelper.WriteAttribute(sw, "opacity", opacity);
		if (linestyleFieldSpecified)
		{
			NPOI.OpenXml4Net.Util.XmlHelper.WriteAttribute(sw, "linestyle", linestyle.ToString());
		}
		if (miterlimitFieldSpecified)
		{
			NPOI.OpenXml4Net.Util.XmlHelper.WriteAttribute(sw, "miterlimit", (float)miterlimit);
		}
		if (joinstyleFieldSpecified)
		{
			NPOI.OpenXml4Net.Util.XmlHelper.WriteAttribute(sw, "joinstyle", joinstyle.ToString());
		}
		if (endcapFieldSpecified)
		{
			NPOI.OpenXml4Net.Util.XmlHelper.WriteAttribute(sw, "endcap", endcap.ToString());
		}
		NPOI.OpenXml4Net.Util.XmlHelper.WriteAttribute(sw, "dashstyle", dashstyle);
		if (filltypeFieldSpecified)
		{
			NPOI.OpenXml4Net.Util.XmlHelper.WriteAttribute(sw, "filltype", filltype.ToString());
		}
		NPOI.OpenXml4Net.Util.XmlHelper.WriteAttribute(sw, "src", src);
		if (imageaspectFieldSpecified)
		{
			NPOI.OpenXml4Net.Util.XmlHelper.WriteAttribute(sw, "imageaspect", imageaspect.ToString());
		}
		NPOI.OpenXml4Net.Util.XmlHelper.WriteAttribute(sw, "imagesize", imagesize);
		if (imagealignshapeFieldSpecified)
		{
			NPOI.OpenXmlFormats.Util.XmlHelper.WriteAttribute(sw, "imagealignshape", imagealignshape);
		}
		NPOI.OpenXml4Net.Util.XmlHelper.WriteAttribute(sw, "color2", color2);
		if (startarrowSpecified)
		{
			NPOI.OpenXml4Net.Util.XmlHelper.WriteAttribute(sw, "startarrow", startarrow.ToString());
		}
		if (startarrowwidthFieldSpecified)
		{
			NPOI.OpenXml4Net.Util.XmlHelper.WriteAttribute(sw, "startarrowwidth", startarrowwidth.ToString());
		}
		if (startarrowlengthFieldSpecified)
		{
			NPOI.OpenXml4Net.Util.XmlHelper.WriteAttribute(sw, "startarrowlength", startarrowlength.ToString());
		}
		if (endarrowFieldSpecified)
		{
			NPOI.OpenXml4Net.Util.XmlHelper.WriteAttribute(sw, "endarrow", endarrow.ToString());
		}
		if (endarrowwidthFieldSpecified)
		{
			NPOI.OpenXml4Net.Util.XmlHelper.WriteAttribute(sw, "endarrowwidth", endarrowwidth.ToString());
		}
		if (endarrowlengthFieldSpecified)
		{
			NPOI.OpenXml4Net.Util.XmlHelper.WriteAttribute(sw, "endarrowlength", endarrowlength.ToString());
		}
		NPOI.OpenXmlFormats.Util.XmlHelper.WriteAttribute(sw, "insetpen", insetpen);
		sw.Write("/>");
	}
}
