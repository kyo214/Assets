namespace NPOI.OpenXml4Net.OPC;

internal interface RelationshipSource
{
	PackageRelationshipCollection Relationships { get; }

	bool HasRelationships { get; }

	PackageRelationship AddRelationship(PackagePartName targetPartName, TargetMode targetMode, string relationshipType);

	PackageRelationship AddRelationship(PackagePartName targetPartName, TargetMode targetMode, string relationshipType, string id);

	PackageRelationship AddExternalRelationship(string target, string relationshipType);

	PackageRelationship AddExternalRelationship(string target, string relationshipType, string id);

	void ClearRelationships();

	void RemoveRelationship(string id);

	PackageRelationship GetRelationship(string id);

	PackageRelationshipCollection GetRelationshipsByType(string relationshipType);

	bool IsRelationshipExists(PackageRelationship rel);
}
