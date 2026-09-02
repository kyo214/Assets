using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using NPOI.OpenXml4Net.Util;

namespace NPOI.OpenXmlFormats.Spreadsheet;

[Serializable]
public class CT_Authors
{
	private List<string> authorField;

	[XmlElement("author")]
	public List<string> author
	{
		get
		{
			return authorField;
		}
		set
		{
			authorField = value;
		}
	}

	public int SizeOfAuthorArray()
	{
		if (authorField != null)
		{
			return authorField.Count;
		}
		return 0;
	}

	public string GetAuthorArray(int index)
	{
		if (authorField != null)
		{
			return authorField[index];
		}
		return null;
	}

	public void Insert(int index, string author)
	{
		if (authorField == null)
		{
			authorField = new List<string>();
		}
		authorField.Insert(index, author);
	}

	public void AddAuthor(string name)
	{
		if (authorField == null)
		{
			authorField = new List<string>();
		}
		authorField.Add(name);
	}

	public static CT_Authors Parse(XmlNode node, XmlNamespaceManager namespaceManager)
	{
		if (node == null)
		{
			return null;
		}
		CT_Authors cT_Authors = new CT_Authors();
		cT_Authors.author = new List<string>();
		foreach (XmlNode childNode in node.ChildNodes)
		{
			if (childNode.LocalName == "author")
			{
				cT_Authors.author.Add(childNode.InnerText);
			}
		}
		return cT_Authors;
	}

	internal void Write(StreamWriter sw, string nodeName)
	{
		sw.Write($"<{nodeName}>");
		if (author != null)
		{
			foreach (string item in author)
			{
				sw.Write($"<author>{XmlHelper.EncodeXml(item)}</author>");
			}
		}
		sw.Write($"</{nodeName}>");
	}
}
