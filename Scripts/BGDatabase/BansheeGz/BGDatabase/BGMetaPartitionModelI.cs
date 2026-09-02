namespace BansheeGz.BGDatabase;

public interface BGMetaPartitionModelI
{
	BGMetaEntity Meta { get; }

	bool IsRoot { get; }

	int? GetPartitionIndex(BGEntity entity);
}
