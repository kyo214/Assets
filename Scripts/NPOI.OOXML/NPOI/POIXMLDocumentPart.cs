using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using NPOI.OpenXml4Net.Exceptions;
using NPOI.OpenXml4Net.OPC;
using NPOI.OpenXml4Net.OPC.Internal;
using NPOI.OpenXml4Net.Util;
using NPOI.Util;

namespace NPOI;

public class POIXMLDocumentPart
{
	public class RelationPart
	{
		private PackageRelationship relationship;

		private POIXMLDocumentPart documentPart;

		public PackageRelationship Relationship => relationship;

		public POIXMLDocumentPart DocumentPart => documentPart;

		internal RelationPart(PackageRelationship relationship, POIXMLDocumentPart documentPart)
		{
			this.relationship = relationship;
			this.documentPart = documentPart;
		}

		public T GetDocumentPart<T>() where T : POIXMLDocumentPart
		{
			return (T)documentPart;
		}
	}

	private static POILogger logger = POILogFactory.GetLogger(typeof(POIXMLDocumentPart));

	private string coreDocumentRel = "http://schemas.openxmlformats.org/officeDocument/2006/relationships/officeDocument";

	private PackagePart packagePart;

	private PackageRelationship packageRel;

	private POIXMLDocumentPart parent;

	private Dictionary<string, RelationPart> relations = new Dictionary<string, RelationPart>();

	private int relationCounter;

	private static XmlNamespaceManager nsm = null;

	public static XmlNamespaceManager NamespaceManager
	{
		get
		{
			if (nsm == null)
			{
				nsm = CreateDefaultNSM();
			}
			return nsm;
		}
	}

	public List<RelationPart> RelationParts => new List<RelationPart>(relations.Values);

	private int IncrementRelationCounter()
	{
		relationCounter++;
		return relationCounter;
	}

	private int DecrementRelationCounter()
	{
		relationCounter--;
		return relationCounter;
	}

	private int GetRelationCounter()
	{
		return relationCounter;
	}

	public POIXMLDocumentPart(OPCPackage pkg)
		: this(pkg, "http://schemas.openxmlformats.org/officeDocument/2006/relationships/officeDocument")
	{
	}

	public POIXMLDocumentPart(OPCPackage pkg, string coreDocumentRel)
		: this(GetPartFromOPCPackage(pkg, coreDocumentRel))
	{
		this.coreDocumentRel = coreDocumentRel;
		PackageRelationship relationship = pkg.GetRelationshipsByType(this.coreDocumentRel).GetRelationship(0);
		if (relationship == null)
		{
			relationship = pkg.GetRelationshipsByType("http://purl.oclc.org/ooxml/officeDocument/relationships/officeDocument").GetRelationship(0);
			if (relationship != null)
			{
				throw new POIXMLException("Strict OOXML isn't currently supported, please see bug #57699");
			}
		}
		if (relationship == null)
		{
			throw new POIXMLException("OOXML file structure broken/invalid - no core document found!");
		}
		packagePart = pkg.GetPart(relationship);
		packageRel = relationship;
	}

	public POIXMLDocumentPart()
	{
	}

	public POIXMLDocumentPart(PackagePart part)
		: this(null, part)
	{
	}

	public POIXMLDocumentPart(POIXMLDocumentPart parent, PackagePart part)
	{
		packagePart = part;
		this.parent = parent;
	}

	[Obsolete("deprecated in POI 3.14, scheduled for removal in POI 3.16")]
	public POIXMLDocumentPart(PackagePart part, PackageRelationship rel)
		: this(null, part)
	{
	}

	[Obsolete("deprecated in POI 3.14, scheduled for removal in POI 3.16")]
	public POIXMLDocumentPart(POIXMLDocumentPart parent, PackagePart part, PackageRelationship rel)
		: this(null, part)
	{
		packagePart = part;
		packageRel = rel;
		this.parent = parent;
	}

	protected void Rebase(OPCPackage pkg)
	{
		PackageRelationshipCollection relationshipsByType = packagePart.GetRelationshipsByType(coreDocumentRel);
		if (relationshipsByType.Size != 1)
		{
			throw new InvalidOperationException("Tried to rebase using " + coreDocumentRel + " but found " + relationshipsByType.Size + " parts of the right type");
		}
		packagePart = packagePart.GetRelatedPart(relationshipsByType.GetRelationship(0));
	}

	internal static XmlNamespaceManager CreateDefaultNSM()
	{
		XmlNamespaceManager xmlNamespaceManager = new XmlNamespaceManager(new NameTable());
		xmlNamespaceManager.AddNamespace(string.Empty, "http://schemas.openxmlformats.org/spreadsheetml/2006/main");
		xmlNamespaceManager.AddNamespace("d", "http://schemas.openxmlformats.org/spreadsheetml/2006/main");
		xmlNamespaceManager.AddNamespace("a", "http://schemas.openxmlformats.org/drawingml/2006/main");
		xmlNamespaceManager.AddNamespace("xdr", "http://schemas.openxmlformats.org/drawingml/2006/spreadsheetDrawing");
		xmlNamespaceManager.AddNamespace("r", "http://schemas.openxmlformats.org/officeDocument/2006/relationships");
		xmlNamespaceManager.AddNamespace("c", "http://schemas.openxmlformats.org/drawingml/2006/chart");
		xmlNamespaceManager.AddNamespace("vt", "http://schemas.openxmlformats.org/officeDocument/2006/docPropsVTypes");
		xmlNamespaceManager.AddNamespace("w", "http://schemas.openxmlformats.org/wordprocessingml/2006/main");
		xmlNamespaceManager.AddNamespace("wp", "http://schemas.openxmlformats.org/drawingml/2006/wordprocessingDrawing");
		xmlNamespaceManager.AddNamespace("m", "http://schemas.openxmlformats.org/officeDocument/2006/math");
		xmlNamespaceManager.AddNamespace("ve", "http://schemas.openxmlformats.org/markup-compatibility/2006");
		xmlNamespaceManager.AddNamespace("mc", "http://schemas.openxmlformats.org/markup-compatibility/2006");
		xmlNamespaceManager.AddNamespace("o", "urn:schemas-microsoft-com:office:office");
		xmlNamespaceManager.AddNamespace("v", "urn:schemas-microsoft-com:vml");
		xmlNamespaceManager.AddNamespace("wne", "http://schemas.microsoft.com/office/word/2006/wordml");
		xmlNamespaceManager.AddNamespace("xp", "http://schemas.openxmlformats.org/officeDocument/2006/relationships/extended-properties");
		xmlNamespaceManager.AddNamespace("ctp", "http://schemas.openxmlformats.org/officeDocument/2006/relationships/custom-properties");
		xmlNamespaceManager.AddNamespace("cp", PackagePropertiesPart.NAMESPACE_CP_URI);
		xmlNamespaceManager.AddNamespace("dc", PackagePropertiesPart.NAMESPACE_DC_URI);
		xmlNamespaceManager.AddNamespace("dcterms", PackagePropertiesPart.NAMESPACE_DCTERMS_URI);
		xmlNamespaceManager.AddNamespace("dcmitype", "http://purl.org/dc/dcmitype/");
		xmlNamespaceManager.AddNamespace("xsi", PackagePropertiesPart.NAMESPACE_XSI_URI);
		xmlNamespaceManager.AddNamespace("xsd", "http://www.w3.org/2001/XMLSchema");
		return xmlNamespaceManager;
	}

	public PackagePart GetPackagePart()
	{
		return packagePart;
	}

	public static XmlDocument ConvertStreamToXml(Stream xmlStream)
	{
		XmlDocument xmlDocument = new XmlDocument();
		XmlHelper.LoadXmlSafe(xmlDocument, xmlStream);
		return xmlDocument;
	}

	[Obsolete("deprecated in POI 3.14, scheduled for removal in POI 3.16")]
	public PackageRelationship GetPackageRelationship()
	{
		if (parent != null)
		{
			foreach (RelationPart relationPart in parent.RelationParts)
			{
				if (relationPart.DocumentPart == this)
				{
					return relationPart.Relationship;
				}
			}
		}
		else
		{
			OPCPackage package = GetPackagePart().Package;
			string name = GetPackagePart().PartName.Name;
			foreach (PackageRelationship relationship in package.Relationships)
			{
				if (relationship.TargetUri.ToString().Equals(name))
				{
					return relationship;
				}
			}
		}
		return null;
	}

	public List<POIXMLDocumentPart> GetRelations()
	{
		List<POIXMLDocumentPart> list = new List<POIXMLDocumentPart>();
		foreach (RelationPart value in relations.Values)
		{
			list.Add(value.DocumentPart);
		}
		return list;
	}

	public POIXMLDocumentPart GetRelationById(string id)
	{
		if (string.IsNullOrEmpty(id) || !relations.ContainsKey(id))
		{
			return null;
		}
		return relations[id]?.DocumentPart;
	}

	public string GetRelationId(POIXMLDocumentPart part)
	{
		foreach (KeyValuePair<string, RelationPart> relation in relations)
		{
			if (relation.Value.DocumentPart == part)
			{
				return relation.Value.Relationship.Id;
			}
		}
		return null;
	}

	[Obsolete("deprecated in POI 3.14, scheduled for removal in POI 3.16")]
	public void AddRelation(string id, POIXMLDocumentPart part)
	{
		PackageRelationship relationship = part.GetPackagePart().GetRelationship(id);
		AddRelation(relationship, part);
	}

	public RelationPart AddRelation(string relId, POIXMLRelation relationshipType, POIXMLDocumentPart part)
	{
		PackageRelationship packageRelationship = packagePart.FindExistingRelation(part.GetPackagePart());
		if (packageRelationship == null)
		{
			PackagePartName partName = part.GetPackagePart().PartName;
			string relation = relationshipType.Relation;
			packageRelationship = packagePart.AddRelationship(partName, TargetMode.Internal, relation, relId);
		}
		AddRelation(packageRelationship, part);
		return new RelationPart(packageRelationship, part);
	}

	private void AddRelation(PackageRelationship pr, POIXMLDocumentPart part)
	{
		if (relations.ContainsKey(pr.Id))
		{
			relations[pr.Id] = new RelationPart(pr, part);
		}
		else
		{
			relations.Add(pr.Id, new RelationPart(pr, part));
		}
		part.IncrementRelationCounter();
	}

	protected internal void RemoveRelation(POIXMLDocumentPart part)
	{
		RemoveRelation(part, RemoveUnusedParts: true);
	}

	protected internal bool RemoveRelation(POIXMLDocumentPart part, bool RemoveUnusedParts)
	{
		string relationId = GetRelationId(part);
		if (relationId == null)
		{
			return false;
		}
		part.DecrementRelationCounter();
		GetPackagePart().RemoveRelationship(relationId);
		relations.Remove(relationId);
		if (RemoveUnusedParts && part.GetRelationCounter() == 0)
		{
			try
			{
				part.onDocumentRemove();
			}
			catch (IOException ex)
			{
				throw new POIXMLException(ex);
			}
			GetPackagePart().Package.RemovePart(part.GetPackagePart());
		}
		return true;
	}

	public POIXMLDocumentPart GetParent()
	{
		return parent;
	}

	public override string ToString()
	{
		if (packagePart != null)
		{
			return packagePart.ToString();
		}
		return string.Empty;
	}

	protected internal virtual void Commit()
	{
	}

	protected internal void OnSave(List<PackagePart> alreadySaved)
	{
		PrepareForCommit();
		Commit();
		alreadySaved.Add(GetPackagePart());
		foreach (RelationPart value in relations.Values)
		{
			POIXMLDocumentPart documentPart = value.DocumentPart;
			if (!alreadySaved.Contains(documentPart.GetPackagePart()))
			{
				documentPart.OnSave(alreadySaved);
			}
		}
	}

	protected internal virtual void PrepareForCommit()
	{
		GetPackagePart()?.Clear();
	}

	public POIXMLDocumentPart CreateRelationship(POIXMLRelation descriptor, POIXMLFactory factory)
	{
		return CreateRelationship(descriptor, factory, -1, noRelation: false).DocumentPart;
	}

	public POIXMLDocumentPart CreateRelationship(POIXMLRelation descriptor, POIXMLFactory factory, int idx)
	{
		return CreateRelationship(descriptor, factory, idx, noRelation: false).DocumentPart;
	}

	protected RelationPart CreateRelationship(POIXMLRelation descriptor, POIXMLFactory factory, int idx, bool noRelation)
	{
		try
		{
			PackagePartName packagePartName = PackagingUriHelper.CreatePartName(descriptor.GetFileName(idx));
			PackageRelationship packageRelationship = null;
			PackagePart packagePart = this.packagePart.Package.CreatePart(packagePartName, descriptor.ContentType);
			if (!noRelation)
			{
				packageRelationship = this.packagePart.AddRelationship(packagePartName, TargetMode.Internal, descriptor.Relation);
			}
			POIXMLDocumentPart pOIXMLDocumentPart = factory.NewDocumentPart(descriptor);
			pOIXMLDocumentPart.packageRel = packageRelationship;
			pOIXMLDocumentPart.packagePart = packagePart;
			pOIXMLDocumentPart.parent = this;
			if (!noRelation)
			{
				AddRelation(packageRelationship, pOIXMLDocumentPart);
			}
			return new RelationPart(packageRelationship, pOIXMLDocumentPart);
		}
		catch (PartAlreadyExistsException ex)
		{
			throw ex;
		}
		catch (Exception ex2)
		{
			throw new POIXMLException(ex2);
		}
	}

	public TValue PutDictionary<TKey, TValue>(Dictionary<TKey, TValue> dict, TKey key, TValue value)
	{
		TValue result = default;
		if (dict.ContainsKey(key))
		{
			result = dict[key];
			dict[key] = value;
		}
		else
		{
			dict.Add(key, value);
		}
		return result;
	}

	public TValue GetDictionary<TKey, TValue>(Dictionary<TKey, TValue> dict, TKey key)
	{
		if (dict.ContainsKey(key))
		{
			return dict[key];
		}
		return default;
	}

	protected void Read(POIXMLFactory factory, Dictionary<PackagePart, POIXMLDocumentPart> context)
	{
		PackagePart packagePart = GetPackagePart();
		POIXMLDocumentPart pOIXMLDocumentPart = PutDictionary(context, packagePart, this);
		if (pOIXMLDocumentPart != null && pOIXMLDocumentPart != this)
		{
			throw new POIXMLException("Unique PackagePart-POIXMLDocumentPart relation broken!");
		}
		if (!packagePart.HasRelationships)
		{
			return;
		}
		PackageRelationshipCollection relationships = this.packagePart.Relationships;
		List<POIXMLDocumentPart> list = new List<POIXMLDocumentPart>();
		foreach (PackageRelationship item in relationships)
		{
			if (item.TargetMode != TargetMode.Internal)
			{
				continue;
			}
			Uri targetUri = item.TargetUri;
			PackagePartName partName;
			if (targetUri.OriginalString.IndexOf('#') >= 0)
			{
				string empty = string.Empty;
				try
				{
					empty = targetUri.AbsolutePath;
				}
				catch (InvalidOperationException)
				{
					empty = targetUri.OriginalString.Substring(0, targetUri.OriginalString.IndexOf('#'));
				}
				partName = PackagingUriHelper.CreatePartName(empty);
			}
			else
			{
				partName = PackagingUriHelper.CreatePartName(targetUri);
			}
			PackagePart part = this.packagePart.Package.GetPart(partName);
			if (part != null)
			{
				POIXMLDocumentPart pOIXMLDocumentPart2 = GetDictionary(context, part);
				if (pOIXMLDocumentPart2 == null)
				{
					pOIXMLDocumentPart2 = factory.CreateDocumentPart(this, part);
					pOIXMLDocumentPart2.parent = this;
					PutDictionary(context, part, pOIXMLDocumentPart2);
					list.Add(pOIXMLDocumentPart2);
				}
				AddRelation(item, pOIXMLDocumentPart2);
			}
		}
		foreach (POIXMLDocumentPart item2 in list)
		{
			item2.Read(factory, context);
		}
	}

	protected PackagePart GetTargetPart(PackageRelationship rel)
	{
		return GetPackagePart().GetRelatedPart(rel);
	}

	internal virtual void OnDocumentCreate()
	{
	}

	internal virtual void OnDocumentRead()
	{
	}

	protected virtual void onDocumentRemove()
	{
	}

	private static PackagePart GetPartFromOPCPackage(OPCPackage pkg, string coreDocumentRel)
	{
		PackageRelationship relationship = pkg.GetRelationshipsByType(coreDocumentRel).GetRelationship(0);
		if (relationship != null)
		{
			return pkg.GetPart(relationship) ?? throw new POIXMLException("OOXML file structure broken/invalid - core document '" + relationship.TargetUri?.ToString() + "' not found.");
		}
		relationship = pkg.GetRelationshipsByType("http://purl.oclc.org/ooxml/officeDocument/relationships/officeDocument").GetRelationship(0);
		if (relationship != null)
		{
			throw new POIXMLException("Strict OOXML isn't currently supported, please see bug #57699");
		}
		throw new POIXMLException("OOXML file structure broken/invalid - no core document found!");
	}
}
