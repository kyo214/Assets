namespace BansheeGz.BGDatabase;

public interface BGAbstractEntityI : BGObjectWithNameI, BGObjectI
{
	new string Name { get; set; }

	int Index { get; }

	BGMetaEntity Meta { get; }

	BGId MetaId { get; }

	string MetaName { get; }

	string FullName { get; }

	BGRepo Repo { get; }

	void Delete();

	T Get<T>(BGField field);

	T Get<T>(BGId fieldId);

	T Get<T>(string fieldName);

	void Set<T>(BGField field, T value);

	void Set<T>(string fieldName, T value);

	void Set<T>(BGId fieldId, T value);
}
