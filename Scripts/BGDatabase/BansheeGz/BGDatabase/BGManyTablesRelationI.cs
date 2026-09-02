using System.Collections.Generic;

namespace BansheeGz.BGDatabase;

public interface BGManyTablesRelationI : BGAbstractRelationI
{
	List<BGMetaEntity> RelatedMetas { get; }

	List<BGId> ToIds { get; }

	void RemoveRelatedMeta(BGMetaEntity metaEntity);

	void AddRelatedMeta(BGMetaEntity metaEntity);
}
