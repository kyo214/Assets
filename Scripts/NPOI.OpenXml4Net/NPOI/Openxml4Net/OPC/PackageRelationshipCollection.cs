using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Xml.XPath;
using NPOI.OpenXml4Net.Exceptions;
using NPOI.Util;

namespace NPOI.OpenXml4Net.OPC;

public class PackageRelationshipCollection : IEnumerator<PackageRelationship>, IDisposable, IEnumerator
{
	private class DuplicateComparer : IComparer<string>
	{
		public int Compare(string x, string y)
		{
			if (x.CompareTo(y) < 0)
			{
				return -1;
			}
			return 1;
		}
	}

	private static POILogger logger = POILogFactory.GetLogger(typeof(PackageRelationshipCollection));

	private SortedList<string, PackageRelationship> relationshipsByID;

	private SortedList<string, PackageRelationship> relationshipsByType;

	private SortedList<string, PackageRelationship> internalRelationshipsByTargetName;

	private PackagePart relationshipPart;

	private PackagePart sourcePart;

	private PackagePartName partName;

	private OPCPackage container;

	private int nextRelationshipId = -1;

	public int Size => relationshipsByID.Values.Count;

	PackageRelationship IEnumerator<PackageRelationship>.Current
	{
		get
		{
			throw new NotImplementedException();
		}
	}

	object IEnumerator.Current
	{
		get
		{
			throw new NotImplementedException();
		}
	}

	public PackageRelationshipCollection()
	{
		relationshipsByID = new SortedList<string, PackageRelationship>();
		relationshipsByType = new SortedList<string, PackageRelationship>(new DuplicateComparer());
		internalRelationshipsByTargetName = new SortedList<string, PackageRelationship>();
	}

	public PackageRelationshipCollection(PackageRelationshipCollection coll, string filter)
		: this()
	{
		foreach (PackageRelationship value in coll.relationshipsByID.Values)
		{
			if (filter == null || value.RelationshipType.Equals(filter))
			{
				AddRelationship(value);
			}
		}
	}

	public PackageRelationshipCollection(OPCPackage container)
		: this(container, null)
	{
	}

	public PackageRelationshipCollection(PackagePart part)
		: this(part._container, part)
	{
	}

	public PackageRelationshipCollection(OPCPackage container, PackagePart part)
		: this()
	{
		if (container == null)
		{
			throw new ArgumentException("container");
		}
		if (part != null && part.IsRelationshipPart)
		{
			throw new ArgumentException("part");
		}
		this.container = container;
		sourcePart = part;
		partName = GetRelationshipPartName(part);
		if (container.GetPackageAccess() != PackageAccess.WRITE && container.ContainPart(partName))
		{
			relationshipPart = container.GetPart(partName);
			ParseRelationshipsPart(relationshipPart);
		}
	}

	private static PackagePartName GetRelationshipPartName(PackagePart part)
	{
		PackagePartName packagePartName = ((part != null) ? part.PartName : PackagingUriHelper.PACKAGE_ROOT_PART_NAME);
		return PackagingUriHelper.GetRelationshipPartName(packagePartName);
	}

	public void AddRelationship(PackageRelationship relPart)
	{
		if (relPart == null || string.IsNullOrEmpty(relPart.Id))
		{
			throw new ArgumentException("invalid relationship part/id");
		}
		relationshipsByID[relPart.Id] = relPart;
		relationshipsByType[relPart.RelationshipType] = relPart;
	}

	public PackageRelationship AddRelationship(Uri targetUri, TargetMode targetMode, string relationshipType, string id)
	{
		if (id == null)
		{
			if (nextRelationshipId == -1)
			{
				nextRelationshipId = Size + 1;
			}
			do
			{
				id = "rId" + nextRelationshipId++;
			}
			while (relationshipsByID.ContainsKey(id));
		}
		PackageRelationship packageRelationship = new PackageRelationship(container, sourcePart, targetUri, targetMode, relationshipType, id);
		relationshipsByID[packageRelationship.Id] = packageRelationship;
		relationshipsByType[packageRelationship.RelationshipType] = packageRelationship;
		if (targetMode == TargetMode.Internal)
		{
			internalRelationshipsByTargetName.Add(targetUri.OriginalString, packageRelationship);
		}
		return packageRelationship;
	}

	public void RemoveRelationship(string id)
	{
		if (relationshipsByID == null || relationshipsByType == null)
		{
			return;
		}
		PackageRelationship packageRelationship = relationshipsByID[id];
		if (packageRelationship == null)
		{
			return;
		}
		relationshipsByID.Remove(packageRelationship.Id);
		for (int i = 0; i < relationshipsByType.Count; i++)
		{
			if (relationshipsByType.Values[i] == packageRelationship)
			{
				relationshipsByType.RemoveAt(i);
			}
		}
		internalRelationshipsByTargetName.RemoveAt(internalRelationshipsByTargetName.IndexOfValue(packageRelationship));
	}

	public void RemoveRelationship(PackageRelationship rel)
	{
		if (rel == null)
		{
			throw new ArgumentException("rel");
		}
		relationshipsByID.Values.Remove(rel);
		relationshipsByType.Values.Remove(rel);
	}

	public PackageRelationship GetRelationship(int index)
	{
		if (index < 0 || index > relationshipsByID.Values.Count)
		{
			throw new ArgumentException("index");
		}
		int num = 0;
		foreach (PackageRelationship value in relationshipsByID.Values)
		{
			if (index == num++)
			{
				return value;
			}
		}
		return null;
	}

	public PackageRelationship GetRelationshipByID(string id)
	{
		if (!relationshipsByID.ContainsKey(id))
		{
			return null;
		}
		return relationshipsByID[id];
	}

	private void ParseRelationshipsPart(PackagePart relPart)
	{
		try
		{
			logger.Log(1, "Parsing relationship: " + relPart.PartName);
			XPathDocument xPathDocument = DocumentHelper.ReadDocument(relPart.GetInputStream());
			bool flag = false;
			XPathNavigator xPathNavigator = xPathDocument.CreateNavigator();
			XmlNamespaceManager xmlNamespaceManager = new XmlNamespaceManager(xPathNavigator.NameTable);
			xmlNamespaceManager.AddNamespace("x", "http://schemas.openxmlformats.org/package/2006/relationships");
			XPathNodeIterator xPathNodeIterator = xPathNavigator.Select("//x:" + PackageRelationship.RELATIONSHIP_TAG_NAME, xmlNamespaceManager);
			while (xPathNodeIterator.MoveNext())
			{
				string attribute = xPathNodeIterator.Current.GetAttribute(PackageRelationship.ID_ATTRIBUTE_NAME, xPathNavigator.NamespaceURI);
				string attribute2 = xPathNodeIterator.Current.GetAttribute(PackageRelationship.TYPE_ATTRIBUTE_NAME, xPathNavigator.NamespaceURI);
				if (attribute2.Equals("http://schemas.openxmlformats.org/package/2006/relationships/metadata/core-properties"))
				{
					if (flag)
					{
						throw new InvalidFormatException("OPC Compliance error [M4.1]: there is more than one core properties relationship in the package !");
					}
					flag = true;
				}
				string attribute3 = xPathNodeIterator.Current.GetAttribute(PackageRelationship.TARGET_MODE_ATTRIBUTE_NAME, xPathNavigator.NamespaceURI);
				TargetMode targetMode = TargetMode.Internal;
				if (attribute3 != string.Empty)
				{
					targetMode = ((!attribute3.ToLower().Equals("internal")) ? TargetMode.External : TargetMode.Internal);
				}
				Uri targetUri = PackagingUriHelper.ToUri("http://invalid.uri");
				string attribute4 = xPathNodeIterator.Current.GetAttribute(PackageRelationship.TARGET_ATTRIBUTE_NAME, xPathNavigator.NamespaceURI);
				try
				{
					targetUri = PackagingUriHelper.ToUri(attribute4);
				}
				catch (UriFormatException exception)
				{
					logger.Log(7, "Cannot convert " + attribute4 + " in a valid relationship URI-> dummy-URI used", exception);
				}
				AddRelationship(targetUri, targetMode, attribute2, attribute);
			}
		}
		catch (Exception ex)
		{
			logger.Log(7, ex);
			throw new InvalidFormatException(ex.Message);
		}
	}

	public PackageRelationshipCollection GetRelationships(string typeFilter)
	{
		return new PackageRelationshipCollection(this, typeFilter);
	}

	public IEnumerator<PackageRelationship> GetEnumerator()
	{
		return relationshipsByID.Values.GetEnumerator();
	}

	public IEnumerator<PackageRelationship> Iterator(string typeFilter)
	{
		List<PackageRelationship> list = new List<PackageRelationship>();
		foreach (PackageRelationship value in relationshipsByID.Values)
		{
			if (value.RelationshipType.Equals(typeFilter))
			{
				list.Add(value);
			}
		}
		return list.GetEnumerator();
	}

	public void Clear()
	{
		relationshipsByID.Clear();
		relationshipsByType.Clear();
		internalRelationshipsByTargetName.Clear();
	}

	public PackageRelationship FindExistingInternalRelation(PackagePart packagePart)
	{
		string name = packagePart.PartName.Name;
		if (!internalRelationshipsByTargetName.ContainsKey(name))
		{
			return null;
		}
		return internalRelationshipsByTargetName[name];
	}

	public override string ToString()
	{
		string text = ((relationshipsByID != null) ? (relationshipsByID.Count + " relationship(s) = [") : "relationshipsByID=null");
		text = ((relationshipPart == null || relationshipPart.PartName == null) ? (text + ",relationshipPart=null") : (text + "," + relationshipPart.PartName));
		text = ((sourcePart == null || sourcePart.PartName == null) ? (text + ",sourcePart=null") : (text + "," + sourcePart.PartName));
		text = ((partName == null) ? (text + ",uri=null)") : (text + "," + partName));
		return text + "]";
	}

	void IDisposable.Dispose()
	{
	}

	bool IEnumerator.MoveNext()
	{
		throw new NotImplementedException();
	}

	void IEnumerator.Reset()
	{
		Clear();
	}
}
