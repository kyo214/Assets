using System;
using System.Text;

namespace NPOI.OpenXml4Net.OPC;

public class PackageRelationship
{
	private static Uri containerRelationshipPart = PackagingUriHelper.ParseUri("/_rels/.rels", UriKind.RelativeOrAbsolute);

	public static string ID_ATTRIBUTE_NAME = "Id";

	public static string RELATIONSHIPS_TAG_NAME = "Relationships";

	public static string RELATIONSHIP_TAG_NAME = "Relationship";

	public static string TARGET_ATTRIBUTE_NAME = "Target";

	public static string TARGET_MODE_ATTRIBUTE_NAME = "TargetMode";

	public static string TYPE_ATTRIBUTE_NAME = "Type";

	private string id;

	private OPCPackage container;

	private string relationshipType;

	private PackagePart source;

	private TargetMode? targetMode;

	private Uri targetUri;

	public static Uri ContainerPartRelationship => containerRelationshipPart;

	public OPCPackage Package => container;

	public string Id => id;

	public string RelationshipType => relationshipType;

	public PackagePart Source => source;

	public Uri SourceUri
	{
		get
		{
			if (source == null)
			{
				return PackagingUriHelper.PACKAGE_ROOT_URI;
			}
			return source.PartName.URI;
		}
	}

	public TargetMode? TargetMode => targetMode;

	public Uri TargetUri
	{
		get
		{
			if (targetMode == NPOI.OpenXml4Net.OPC.TargetMode.External)
			{
				return targetUri;
			}
			if (!targetUri.ToString().StartsWith("/"))
			{
				return PackagingUriHelper.ResolvePartUri(SourceUri, targetUri);
			}
			return targetUri;
		}
	}

	public PackageRelationship(OPCPackage pkg, PackagePart sourcePart, Uri targetUri, TargetMode targetMode, string relationshipType, string id)
	{
		if (pkg == null)
		{
			throw new ArgumentException("pkg");
		}
		if (targetUri == null)
		{
			throw new ArgumentException("targetUri");
		}
		if (relationshipType == null)
		{
			throw new ArgumentException("relationshipType");
		}
		if (id == null)
		{
			throw new ArgumentException("id");
		}
		container = pkg;
		source = sourcePart;
		this.targetUri = targetUri;
		this.targetMode = targetMode;
		this.relationshipType = relationshipType;
		this.id = id;
	}

	public override bool Equals(object obj)
	{
		if (!(obj is PackageRelationship))
		{
			return false;
		}
		PackageRelationship packageRelationship = (PackageRelationship)obj;
		if (id == packageRelationship.id && relationshipType == packageRelationship.relationshipType && (packageRelationship.source == null || packageRelationship.source.Equals(source)) && targetMode == packageRelationship.targetMode)
		{
			return targetUri.Equals(packageRelationship.targetUri);
		}
		return false;
	}

	public override int GetHashCode()
	{
		return id.GetHashCode() + relationshipType.GetHashCode() + source.GetHashCode() + targetMode.GetHashCode() + targetUri.GetHashCode();
	}

	public override string ToString()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append((id == null) ? "id=null" : ("id=" + id));
		stringBuilder.Append((container == null) ? " - container=null" : (" - container=" + container.ToString()));
		stringBuilder.Append((relationshipType == null) ? " - relationshipType=null" : (" - relationshipType=" + relationshipType.ToString()));
		stringBuilder.Append((source == null) ? " - source=null" : (" - source=" + SourceUri.OriginalString));
		stringBuilder.Append((targetUri == null) ? " - target=null" : (" - target=" + TargetUri.OriginalString));
		stringBuilder.Append((!targetMode.HasValue) ? ",targetMode=null" : (",targetMode=" + targetMode.ToString()));
		return stringBuilder.ToString();
	}
}
