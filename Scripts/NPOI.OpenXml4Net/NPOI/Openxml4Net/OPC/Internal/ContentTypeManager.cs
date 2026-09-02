using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.XPath;
using NPOI.OpenXml4Net.Exceptions;
using NPOI.Util;

namespace NPOI.OpenXml4Net.OPC.Internal;

public abstract class ContentTypeManager
{
	public const string CONTENT_TYPES_PART_NAME = "[Content_Types].xml";

	public const string TYPES_NAMESPACE_URI = "http://schemas.openxmlformats.org/package/2006/content-types";

	private const string TYPES_TAG_NAME = "Types";

	private const string DEFAULT_TAG_NAME = "Default";

	private const string EXTENSION_ATTRIBUTE_NAME = "Extension";

	private const string CONTENT_TYPE_ATTRIBUTE_NAME = "ContentType";

	private const string OVERRIDE_TAG_NAME = "Override";

	private const string PART_NAME_ATTRIBUTE_NAME = "PartName";

	protected OPCPackage container;

	private SortedList<string, string> defaultContentType;

	private SortedList<PackagePartName, string> overrideContentType;

	public ContentTypeManager(Stream in1, OPCPackage pkg)
	{
		container = pkg;
		defaultContentType = new SortedList<string, string>();
		if (in1 != null)
		{
			try
			{
				ParseContentTypesFile(in1);
			}
			catch (InvalidFormatException innerException)
			{
				throw new InvalidFormatException("Can't read content types part !", innerException);
			}
		}
	}

	public void AddContentType(PackagePartName partName, string contentType)
	{
		bool flag = false;
		string text = partName.Extension.ToLower();
		if (text.Length == 0 || (defaultContentType.ContainsKey(text) && !(flag = defaultContentType.ContainsValue(contentType))))
		{
			AddOverrideContentType(partName, contentType);
		}
		else if (!flag)
		{
			AddDefaultContentType(text, contentType);
		}
	}

	private void AddOverrideContentType(PackagePartName partName, string contentType)
	{
		if (overrideContentType == null)
		{
			overrideContentType = new SortedList<PackagePartName, string>();
		}
		if (!overrideContentType.ContainsKey(partName))
		{
			overrideContentType.Add(partName, contentType);
		}
		else
		{
			overrideContentType[partName] = contentType;
		}
	}

	private void AddDefaultContentType(string extension, string contentType)
	{
		defaultContentType.Add(extension.ToLower(), contentType);
	}

	public void RemoveContentType(PackagePartName partName)
	{
		if (partName == null)
		{
			throw new ArgumentException("partName");
		}
		if (overrideContentType != null && overrideContentType.ContainsKey(partName))
		{
			overrideContentType.Remove(partName);
			return;
		}
		string extension = partName.Extension;
		bool flag = true;
		if (container != null)
		{
			try
			{
				foreach (PackagePart part in container.GetParts())
				{
					if (!part.PartName.Equals(partName) && part.PartName.Extension.Equals(extension, StringComparison.InvariantCultureIgnoreCase))
					{
						flag = false;
						break;
					}
				}
			}
			catch (InvalidFormatException ex)
			{
				throw new InvalidOperationException(ex.Message);
			}
		}
		if (flag)
		{
			defaultContentType.Remove(extension);
		}
		if (container == null)
		{
			return;
		}
		try
		{
			foreach (PackagePart part2 in container.GetParts())
			{
				if (!part2.PartName.Equals(partName) && GetContentType(part2.PartName) == null)
				{
					throw new InvalidOperationException("Rule M2.4 is not respected: Nor a default element or override element is associated with the part: " + part2.PartName.Name);
				}
			}
		}
		catch (InvalidFormatException ex2)
		{
			throw new InvalidOperationException(ex2.Message);
		}
	}

	public bool IsContentTypeRegister(string contentType)
	{
		if (contentType == null)
		{
			throw new ArgumentException("contentType");
		}
		if (!defaultContentType.Values.Contains(contentType))
		{
			if (overrideContentType != null)
			{
				return overrideContentType.Values.Contains(contentType);
			}
			return false;
		}
		return true;
	}

	public string GetContentType(PackagePartName partName)
	{
		if (partName == null)
		{
			throw new ArgumentException("partName");
		}
		if (overrideContentType != null && overrideContentType.ContainsKey(partName))
		{
			return overrideContentType[partName];
		}
		string key = partName.Extension.ToLower();
		if (defaultContentType.ContainsKey(key))
		{
			return defaultContentType[key];
		}
		if (container != null && container.GetPart(partName) != null)
		{
			throw new OpenXml4NetException("Rule M2.4 exception : this error should NEVER happen! If you can provide the triggering file, then please raise a bug at https://bz.apache.org/bugzilla/enter_bug.cgi?product=POI and attach a file that triggers it, thanks!");
		}
		return null;
	}

	public void ClearAll()
	{
		defaultContentType.Clear();
		if (overrideContentType != null)
		{
			overrideContentType.Clear();
		}
	}

	public void ClearOverrideContentTypes()
	{
		if (overrideContentType != null)
		{
			overrideContentType.Clear();
		}
	}

	private void ParseContentTypesFile(Stream in1)
	{
		try
		{
			XPathNavigator xPathNavigator = DocumentHelper.ReadDocument(in1).CreateNavigator();
			XmlNamespaceManager xmlNamespaceManager = new XmlNamespaceManager(xPathNavigator.NameTable);
			xmlNamespaceManager.AddNamespace("x", "http://schemas.openxmlformats.org/package/2006/content-types");
			XPathNodeIterator xPathNodeIterator = xPathNavigator.Select("//x:Default", xmlNamespaceManager);
			while (xPathNodeIterator.MoveNext())
			{
				string attribute = xPathNodeIterator.Current.GetAttribute("Extension", xPathNavigator.NamespaceURI);
				string attribute2 = xPathNodeIterator.Current.GetAttribute("ContentType", xPathNavigator.NamespaceURI);
				AddDefaultContentType(attribute, attribute2);
			}
			xPathNodeIterator = xPathNavigator.Select("//x:Override", xmlNamespaceManager);
			while (xPathNodeIterator.MoveNext())
			{
				PackagePartName partName = PackagingUriHelper.CreatePartName(PackagingUriHelper.ParseUri(xPathNodeIterator.Current.GetAttribute("PartName", xPathNavigator.NamespaceURI), UriKind.RelativeOrAbsolute));
				string attribute3 = xPathNodeIterator.Current.GetAttribute("ContentType", xPathNavigator.NamespaceURI);
				AddOverrideContentType(partName, attribute3);
			}
		}
		catch (UriFormatException ex)
		{
			throw new InvalidFormatException(ex.Message);
		}
	}

	public bool Save(Stream outStream)
	{
		XmlDocument xmlDocument = new XmlDocument();
		new XmlNamespaceManager(xmlDocument.NameTable).AddNamespace("x", "http://schemas.openxmlformats.org/package/2006/content-types");
		XmlElement xmlElement = xmlDocument.CreateElement("Types", "http://schemas.openxmlformats.org/package/2006/content-types");
		xmlDocument.AppendChild(xmlElement);
		IEnumerator<KeyValuePair<string, string>> enumerator = defaultContentType.GetEnumerator();
		while (enumerator.MoveNext())
		{
			AppendDefaultType(xmlDocument, xmlElement, enumerator.Current);
		}
		if (overrideContentType != null)
		{
			IEnumerator<KeyValuePair<PackagePartName, string>> enumerator2 = overrideContentType.GetEnumerator();
			while (enumerator2.MoveNext())
			{
				AppendSpecificTypes(xmlDocument, xmlElement, enumerator2.Current);
			}
		}
		xmlDocument.Normalize();
		return SaveImpl(xmlDocument, outStream);
	}

	private void AppendSpecificTypes(XmlDocument xmldoc, XmlElement root, KeyValuePair<PackagePartName, string> entry)
	{
		XmlElement xmlElement = xmldoc.CreateElement("Override", "http://schemas.openxmlformats.org/package/2006/content-types");
		root.AppendChild(xmlElement);
		xmlElement.SetAttribute("PartName", entry.Key.Name);
		xmlElement.SetAttribute("ContentType", entry.Value);
	}

	private void AppendDefaultType(XmlDocument xmldoc, XmlElement root, KeyValuePair<string, string> entry)
	{
		XmlElement xmlElement = xmldoc.CreateElement("Default", "http://schemas.openxmlformats.org/package/2006/content-types");
		root.AppendChild(xmlElement);
		xmlElement.SetAttribute("Extension", entry.Key);
		xmlElement.SetAttribute("ContentType", entry.Value);
	}

	public abstract bool SaveImpl(XmlDocument content, Stream out1);
}
