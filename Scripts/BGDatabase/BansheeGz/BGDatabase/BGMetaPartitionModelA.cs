namespace BansheeGz.BGDatabase;

public abstract class BGMetaPartitionModelA : BGMetaPartitionModelI
{
	public interface FieldOwner : BGMetaPartitionModelI
	{
		BGPartitionFieldTypeEnum FieldType { get; }
	}

	private readonly BGMetaEntity meta;

	public BGMetaEntity Meta => meta;

	public virtual bool IsRoot => true;

	protected BGMetaPartitionModelA(BGMetaEntity meta)
	{
		this.meta = meta;
	}

	public abstract int? GetPartitionIndex(BGEntity entity);
}
