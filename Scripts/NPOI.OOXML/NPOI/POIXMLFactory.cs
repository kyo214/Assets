using System;
using NPOI.OpenXml4Net.Exceptions;
using NPOI.OpenXml4Net.OPC;
using NPOI.Util;

namespace NPOI;

public abstract class POIXMLFactory
{
	private static POILogger LOGGER = POILogFactory.GetLogger(typeof(POIXMLFactory));

	private static Type[] PARENT_PART = new Type[2]
	{
		typeof(POIXMLDocumentPart),
		typeof(PackagePart)
	};

	private static Type[] ORPHAN_PART = new Type[1] { typeof(PackagePart) };

	public virtual POIXMLDocumentPart CreateDocumentPart(POIXMLDocumentPart parent, PackagePart part)
	{
		PackageRelationship packageRelationship = GetPackageRelationship(parent, part);
		POIXMLRelation descriptor = GetDescriptor(packageRelationship.RelationshipType);
		if (descriptor == null || descriptor.RelationClass == null)
		{
			LOGGER.Log(1, "using default POIXMLDocumentPart for " + packageRelationship.RelationshipType);
			return new POIXMLDocumentPart(parent, part);
		}
		Type relationClass = descriptor.RelationClass;
		try
		{
			try
			{
				return CreateDocumentPart(relationClass, PARENT_PART, new object[2] { parent, part });
			}
			catch (MissingMethodException)
			{
				return CreateDocumentPart(relationClass, ORPHAN_PART, new object[1] { part });
			}
		}
		catch (Exception ex2)
		{
			throw new POIXMLException(ex2);
		}
	}

	protected abstract POIXMLDocumentPart CreateDocumentPart(Type cls, Type[] classes, object[] values);

	protected abstract POIXMLRelation GetDescriptor(string relationshipType);

	[Obsolete("deprecated in POI 3.14, scheduled for removal in POI 3.16")]
	public virtual POIXMLDocumentPart CreateDocumentPart(POIXMLDocumentPart parent, PackageRelationship rel, PackagePart part)
	{
		return CreateDocumentPart(parent, part);
	}

	public POIXMLDocumentPart NewDocumentPart(POIXMLRelation descriptor)
	{
		Type relationClass = descriptor.RelationClass;
		try
		{
			return CreateDocumentPart(relationClass, null, null);
		}
		catch (Exception ex)
		{
			throw new POIXMLException(ex);
		}
	}

	protected PackageRelationship GetPackageRelationship(POIXMLDocumentPart parent, PackagePart part)
	{
		try
		{
			string name = part.PartName.Name;
			foreach (PackageRelationship relationship in parent.GetPackagePart().Relationships)
			{
				if (relationship.TargetUri.ToString().Equals(name, StringComparison.CurrentCultureIgnoreCase))
				{
					return relationship;
				}
			}
		}
		catch (InvalidFormatException ex)
		{
			throw new POIXMLException("error while determining package relations", ex);
		}
		throw new POIXMLException("package part isn't a child of the parent document.");
	}
}
