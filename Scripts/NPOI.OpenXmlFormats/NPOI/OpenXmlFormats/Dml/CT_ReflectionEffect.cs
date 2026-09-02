using System;
using System.ComponentModel;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using NPOI.OpenXml4Net.Util;

namespace NPOI.OpenXmlFormats.Dml;

[Serializable]
[XmlType(Namespace = "http://schemas.openxmlformats.org/drawingml/2006/main")]
[XmlRoot(Namespace = "http://schemas.openxmlformats.org/drawingml/2006/main", IsNullable = true)]
public class CT_ReflectionEffect
{
	private long blurRadField;

	private int stAField;

	private int stPosField;

	private int endAField;

	private int endPosField;

	private long distField;

	private int dirField;

	private int fadeDirField;

	private int sxField;

	private int syField;

	private int kxField;

	private int kyField;

	private ST_RectAlignment algnField;

	private bool rotWithShapeField;

	[XmlAttribute]
	[DefaultValue(typeof(long), "0")]
	public long blurRad
	{
		get
		{
			return blurRadField;
		}
		set
		{
			blurRadField = value;
		}
	}

	[XmlAttribute]
	[DefaultValue(100000)]
	public int stA
	{
		get
		{
			return stAField;
		}
		set
		{
			stAField = value;
		}
	}

	[XmlAttribute]
	[DefaultValue(0)]
	public int stPos
	{
		get
		{
			return stPosField;
		}
		set
		{
			stPosField = value;
		}
	}

	[XmlAttribute]
	[DefaultValue(0)]
	public int endA
	{
		get
		{
			return endAField;
		}
		set
		{
			endAField = value;
		}
	}

	[XmlAttribute]
	[DefaultValue(100000)]
	public int endPos
	{
		get
		{
			return endPosField;
		}
		set
		{
			endPosField = value;
		}
	}

	[XmlAttribute]
	[DefaultValue(typeof(long), "0")]
	public long dist
	{
		get
		{
			return distField;
		}
		set
		{
			distField = value;
		}
	}

	[XmlAttribute]
	[DefaultValue(0)]
	public int dir
	{
		get
		{
			return dirField;
		}
		set
		{
			dirField = value;
		}
	}

	[XmlAttribute]
	[DefaultValue(5400000)]
	public int fadeDir
	{
		get
		{
			return fadeDirField;
		}
		set
		{
			fadeDirField = value;
		}
	}

	[XmlAttribute]
	[DefaultValue(100000)]
	public int sx
	{
		get
		{
			return sxField;
		}
		set
		{
			sxField = value;
		}
	}

	[XmlAttribute]
	[DefaultValue(100000)]
	public int sy
	{
		get
		{
			return syField;
		}
		set
		{
			syField = value;
		}
	}

	[XmlAttribute]
	[DefaultValue(0)]
	public int kx
	{
		get
		{
			return kxField;
		}
		set
		{
			kxField = value;
		}
	}

	[XmlAttribute]
	[DefaultValue(0)]
	public int ky
	{
		get
		{
			return kyField;
		}
		set
		{
			kyField = value;
		}
	}

	[XmlAttribute]
	[DefaultValue(ST_RectAlignment.b)]
	public ST_RectAlignment algn
	{
		get
		{
			return algnField;
		}
		set
		{
			algnField = value;
		}
	}

	[XmlAttribute]
	[DefaultValue(true)]
	public bool rotWithShape
	{
		get
		{
			return rotWithShapeField;
		}
		set
		{
			rotWithShapeField = value;
		}
	}

	public static CT_ReflectionEffect Parse(XmlNode node, XmlNamespaceManager namespaceManager)
	{
		if (node == null)
		{
			return null;
		}
		CT_ReflectionEffect cT_ReflectionEffect = new CT_ReflectionEffect();
		cT_ReflectionEffect.blurRad = XmlHelper.ReadLong(node.Attributes["blurRad"]);
		cT_ReflectionEffect.stA = XmlHelper.ReadInt(node.Attributes["stA"]);
		cT_ReflectionEffect.stPos = XmlHelper.ReadInt(node.Attributes["stPos"]);
		cT_ReflectionEffect.endA = XmlHelper.ReadInt(node.Attributes["endA"]);
		cT_ReflectionEffect.endPos = XmlHelper.ReadInt(node.Attributes["endPos"]);
		cT_ReflectionEffect.dist = XmlHelper.ReadLong(node.Attributes["dist"]);
		cT_ReflectionEffect.dir = XmlHelper.ReadInt(node.Attributes["dir"]);
		cT_ReflectionEffect.fadeDir = XmlHelper.ReadInt(node.Attributes["fadeDir"]);
		cT_ReflectionEffect.sx = XmlHelper.ReadInt(node.Attributes["sx"]);
		cT_ReflectionEffect.sy = XmlHelper.ReadInt(node.Attributes["sy"]);
		cT_ReflectionEffect.kx = XmlHelper.ReadInt(node.Attributes["kx"]);
		cT_ReflectionEffect.ky = XmlHelper.ReadInt(node.Attributes["ky"]);
		if (node.Attributes["algn"] != null)
		{
			cT_ReflectionEffect.algn = (ST_RectAlignment)Enum.Parse(typeof(ST_RectAlignment), node.Attributes["algn"].Value);
		}
		cT_ReflectionEffect.rotWithShape = XmlHelper.ReadBool(node.Attributes["rotWithShape"]);
		return cT_ReflectionEffect;
	}

	internal void Write(StreamWriter sw, string nodeName)
	{
		sw.Write($"<a:{nodeName}");
		XmlHelper.WriteAttribute(sw, "blurRad", blurRad);
		XmlHelper.WriteAttribute(sw, "stA", stA);
		XmlHelper.WriteAttribute(sw, "stPos", stPos);
		XmlHelper.WriteAttribute(sw, "endA", endA);
		XmlHelper.WriteAttribute(sw, "endPos", endPos);
		XmlHelper.WriteAttribute(sw, "dist", dist);
		XmlHelper.WriteAttribute(sw, "dir", dir);
		XmlHelper.WriteAttribute(sw, "fadeDir", fadeDir);
		XmlHelper.WriteAttribute(sw, "sx", sx);
		XmlHelper.WriteAttribute(sw, "sy", sy);
		XmlHelper.WriteAttribute(sw, "kx", kx);
		XmlHelper.WriteAttribute(sw, "ky", ky);
		XmlHelper.WriteAttribute(sw, "algn", algn.ToString());
		XmlHelper.WriteAttribute(sw, "rotWithShape", rotWithShape);
		sw.Write(">");
		sw.Write($"</a:{nodeName}>");
	}

	public CT_ReflectionEffect()
	{
		blurRadField = 0L;
		stAField = 100000;
		stPosField = 0;
		endAField = 0;
		endPosField = 100000;
		distField = 0L;
		dirField = 0;
		fadeDirField = 5400000;
		sxField = 100000;
		syField = 100000;
		kxField = 0;
		kyField = 0;
		algnField = ST_RectAlignment.b;
		rotWithShapeField = true;
	}
}
