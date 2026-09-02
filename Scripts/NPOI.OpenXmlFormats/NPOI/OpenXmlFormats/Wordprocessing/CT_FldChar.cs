using System;
using System.IO;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using NPOI.OpenXml4Net.Util;

namespace NPOI.OpenXmlFormats.Wordprocessing;

[Serializable]
[XmlType(Namespace = "http://schemas.openxmlformats.org/wordprocessingml/2006/main")]
[XmlRoot(Namespace = "http://schemas.openxmlformats.org/wordprocessingml/2006/main", IsNullable = true)]
public class CT_FldChar
{
	private object itemField;

	private ST_FldCharType fldCharTypeField;

	private ST_OnOff fldLockField;

	private bool fldLockFieldSpecified;

	private ST_OnOff dirtyField;

	private bool dirtyFieldSpecified;

	private CT_FFData ffDataField;

	private CT_Text fldDataField;

	private CT_TrackChangeNumbering numberingChangeField;

	public CT_FFData ffData
	{
		get
		{
			return ffDataField;
		}
		set
		{
			ffDataField = value;
		}
	}

	public CT_Text fldData
	{
		get
		{
			return fldDataField;
		}
		set
		{
			fldDataField = value;
		}
	}

	public CT_TrackChangeNumbering numberingChange
	{
		get
		{
			return numberingChangeField;
		}
		set
		{
			numberingChangeField = value;
		}
	}

	[XmlAttribute(Form = XmlSchemaForm.Qualified)]
	public ST_FldCharType fldCharType
	{
		get
		{
			return fldCharTypeField;
		}
		set
		{
			fldCharTypeField = value;
		}
	}

	[XmlAttribute(Form = XmlSchemaForm.Qualified)]
	public ST_OnOff fldLock
	{
		get
		{
			return fldLockField;
		}
		set
		{
			fldLockField = value;
		}
	}

	[XmlIgnore]
	public bool fldLockSpecified
	{
		get
		{
			return fldLockFieldSpecified;
		}
		set
		{
			fldLockFieldSpecified = value;
		}
	}

	[XmlAttribute(Form = XmlSchemaForm.Qualified)]
	public ST_OnOff dirty
	{
		get
		{
			return dirtyField;
		}
		set
		{
			dirtyField = value;
		}
	}

	[XmlIgnore]
	public bool dirtySpecified
	{
		get
		{
			return dirtyFieldSpecified;
		}
		set
		{
			dirtyFieldSpecified = value;
		}
	}

	public static CT_FldChar Parse(XmlNode node, XmlNamespaceManager namespaceManager)
	{
		if (node == null)
		{
			return null;
		}
		CT_FldChar cT_FldChar = new CT_FldChar();
		if (node.Attributes["w:fldCharType"] != null)
		{
			cT_FldChar.fldCharType = (ST_FldCharType)Enum.Parse(typeof(ST_FldCharType), node.Attributes["w:fldCharType"].Value);
		}
		if (node.Attributes["w:fldLock"] != null)
		{
			cT_FldChar.fldLock = (ST_OnOff)Enum.Parse(typeof(ST_OnOff), node.Attributes["w:fldLock"].Value, ignoreCase: true);
		}
		if (node.Attributes["w:dirty"] != null)
		{
			cT_FldChar.dirty = (ST_OnOff)Enum.Parse(typeof(ST_OnOff), node.Attributes["w:dirty"].Value, ignoreCase: true);
		}
		foreach (XmlNode childNode in node.ChildNodes)
		{
			if (childNode.LocalName == "ffData")
			{
				cT_FldChar.ffDataField = CT_FFData.Parse(childNode, namespaceManager);
			}
			else if (childNode.LocalName == "fldData")
			{
				cT_FldChar.fldDataField = CT_Text.Parse(childNode, namespaceManager);
			}
			else if (childNode.LocalName == "numberingChange")
			{
				cT_FldChar.numberingChangeField = CT_TrackChangeNumbering.Parse(childNode, namespaceManager);
			}
		}
		return cT_FldChar;
	}

	internal void Write(StreamWriter sw, string nodeName)
	{
		sw.Write($"<w:{nodeName}");
		XmlHelper.WriteAttribute(sw, "w:fldCharType", fldCharType.ToString());
		if (fldLock != ST_OnOff.off)
		{
			XmlHelper.WriteAttribute(sw, "w:fldLock", fldLock.ToString());
		}
		if (dirty != ST_OnOff.off)
		{
			XmlHelper.WriteAttribute(sw, "w:dirty", dirty.ToString());
		}
		if (ffDataField == null && fldDataField == null && numberingChangeField == null)
		{
			sw.Write(string.Format("/>", nodeName));
			return;
		}
		sw.Write(">");
		if (ffDataField != null)
		{
			ffDataField.Write(sw, "ffData");
		}
		if (fldDataField != null)
		{
			fldDataField.Write(sw, "fldData");
		}
		if (numberingChangeField != null)
		{
			numberingChangeField.Write(sw, "numberingChange");
		}
		sw.Write($"</w:{nodeName}>");
	}
}
