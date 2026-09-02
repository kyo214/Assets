using System;
using System.Collections.Generic;

namespace BansheeGz.BGDatabase;

public class BGRowsOrder
{
	public class EntityOrderInfo
	{
		private readonly BGEntity entity;

		private readonly BGEntity sourceEntity;

		public int TargetIndex { get; set; }

		public BGEntity Entity => entity;

		public BGEntity SourceEntity => sourceEntity;

		public EntityOrderInfo(BGEntity sourceEntity, BGEntity entity, int targetIndex)
		{
			this.sourceEntity = sourceEntity;
			this.entity = entity;
			TargetIndex = targetIndex;
		}

		public override string ToString()
		{
			return entity.ToString() + " - " + TargetIndex;
		}
	}

	private readonly BGLogger logger;

	private readonly BGMetaEntity meta;

	private readonly List<EntityOrderInfo> rows = new List<EntityOrderInfo>();

	private readonly Action<int, int> swap;

	public BGRowsOrder(BGLogger logger, BGMetaEntity meta, Action<int, int> swap)
	{
		this.logger = logger;
		this.meta = meta;
		this.swap = swap;
	}

	public void Add(EntityOrderInfo entityOrderInfo)
	{
		rows.Add(entityOrderInfo);
	}

	public void Complete(Action finished)
	{
		try
		{
			List<EntityOrderInfo> list = new List<EntityOrderInfo>(rows);
			list.Sort((EntityOrderInfo e1, EntityOrderInfo e2) => e1.SourceEntity.Index.CompareTo(e2.SourceEntity.Index));
			if (!RequireReordering(list))
			{
				logger.AppendLine("No rows sorting is required. Sorting skipped..");
				return;
			}
			List<EntityOrderInfo> list2 = new List<EntityOrderInfo>(rows);
			list2.Sort((EntityOrderInfo e1, EntityOrderInfo e2) => e1.TargetIndex.CompareTo(e2.TargetIndex));
			int num = 0;
			for (int num2 = list.Count - 1; num2 >= 0; num2--)
			{
				EntityOrderInfo infoSource = list[num2];
				EntityOrderInfo entityOrderInfo = list2[num2];
				if (entityOrderInfo != infoSource)
				{
					num++;
					swap(entityOrderInfo.TargetIndex, infoSource.TargetIndex);
					int num3 = list2.FindIndex((EntityOrderInfo orderInfo) => orderInfo == infoSource);
					List<EntityOrderInfo> list3 = list2;
					int index = num2;
					int index2 = num3;
					EntityOrderInfo value = list2[num3];
					EntityOrderInfo value2 = list2[num2];
					list3[index] = value;
					list2[index2] = value2;
					value2 = entityOrderInfo;
					EntityOrderInfo entityOrderInfo2 = infoSource;
					index2 = infoSource.TargetIndex;
					index = entityOrderInfo.TargetIndex;
					value2.TargetIndex = index2;
					entityOrderInfo2.TargetIndex = index;
				}
			}
			logger.AppendLine("Rows were sorted with $ operations ", num);
		}
		finally
		{
			finished?.Invoke();
		}
	}

	private static bool RequireReordering(List<EntityOrderInfo> sortedRows)
	{
		int num = -1;
		foreach (EntityOrderInfo sortedRow in sortedRows)
		{
			int targetIndex = sortedRow.TargetIndex;
			if (num > targetIndex)
			{
				return true;
			}
			num = targetIndex;
		}
		return false;
	}
}
