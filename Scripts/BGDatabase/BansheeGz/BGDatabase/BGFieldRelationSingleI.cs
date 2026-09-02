namespace BansheeGz.BGDatabase;

public interface BGFieldRelationSingleI
{
	BGEntity GetRelatedEntity(int entityIndex);

	void SetRelatedEntity(int entityIndex, BGEntity entity);
}
