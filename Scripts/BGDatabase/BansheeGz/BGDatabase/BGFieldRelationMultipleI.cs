using System.Collections.Generic;

namespace BansheeGz.BGDatabase;

public interface BGFieldRelationMultipleI
{
	List<BGEntity> GetRelatedEntity(int entityIndex);

	void SetRelatedEntity(int entityIndex, List<BGEntity> entity);
}
