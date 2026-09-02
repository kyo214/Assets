using System.Collections.Generic;

namespace BansheeGz.BGDatabase;

public interface BGAbstractRelationI
{
	List<BGEntity> GetRelatedIn(BGId entityId, List<BGEntity> result = null);

	List<BGEntity> GetRelatedIn(HashSet<BGId> entityIds, List<BGEntity> result = null);

	void ClearToValue(BGId entityId);

	void ClearToValue(HashSet<BGId> entityIds);
}
