using System;

namespace NPOI.OpenXml4Net.OPC;

public interface PackageProperties
{
	string GetCategoryProperty();

	void SetCategoryProperty(string category);

	string GetContentStatusProperty();

	void SetContentStatusProperty(string contentStatus);

	string GetContentTypeProperty();

	void SetContentTypeProperty(string contentType);

	DateTime? GetCreatedProperty();

	void SetCreatedProperty(string created);

	void SetCreatedProperty(DateTime? created);

	string GetCreatorProperty();

	void SetCreatorProperty(string creator);

	string GetDescriptionProperty();

	void SetDescriptionProperty(string description);

	string GetIdentifierProperty();

	void SetIdentifierProperty(string identifier);

	string GetKeywordsProperty();

	void SetKeywordsProperty(string keywords);

	string GetLanguageProperty();

	void SetLanguageProperty(string language);

	string GetLastModifiedByProperty();

	void SetLastModifiedByProperty(string lastModifiedBy);

	DateTime? GetLastPrintedProperty();

	void SetLastPrintedProperty(string lastPrinted);

	void SetLastPrintedProperty(DateTime? lastPrinted);

	DateTime? GetModifiedProperty();

	void SetModifiedProperty(string modified);

	void SetModifiedProperty(DateTime? modified);

	string GetRevisionProperty();

	void SetRevisionProperty(string revision);

	string GetSubjectProperty();

	void SetSubjectProperty(string subject);

	string GetTitleProperty();

	void SetTitleProperty(string title);

	string GetVersionProperty();

	void SetVersionProperty(string version);
}
