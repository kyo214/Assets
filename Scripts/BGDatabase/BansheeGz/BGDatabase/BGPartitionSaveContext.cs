using System;

namespace BansheeGz.BGDatabase;

public class BGPartitionSaveContext
{
	public readonly string basicPath;

	public readonly BGRepo repo;

	public readonly BGPartitionsModel partitions;

	public readonly BGMetaPartitionModelProvider provider;

	public readonly BGAddonPartition partitionAddon;

	public readonly bool verticalDisabled;

	public BGPartitionSaveContext(string basicPath, BGRepo repo, BGAddonPartition partitionAddon, BGPartitionsModel partitions, BGMetaPartitionModelProvider provider, bool verticalDisabled)
	{
		if (string.IsNullOrEmpty(basicPath))
		{
			throw new Exception("basicPath can not be null");
		}
		this.basicPath = basicPath;
		this.repo = repo;
		this.partitionAddon = partitionAddon;
		this.partitions = partitions;
		this.provider = provider;
		this.verticalDisabled = verticalDisabled;
	}
}
