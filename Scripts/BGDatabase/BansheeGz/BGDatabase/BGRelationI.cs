namespace BansheeGz.BGDatabase;

public interface BGRelationI : BGAbstractRelationI
{
	BGMetaEntity RelatedMeta { get; }

	BGId ToId { get; }
}
