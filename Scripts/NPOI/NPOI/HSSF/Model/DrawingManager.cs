using System;
using System.Collections;
using NPOI.DDF;

namespace NPOI.HSSF.Model;

[Obsolete("deprecated in POI 3.15-beta2, scheduled for removal in 3.17, use DrawingManager2 instead")]
public class DrawingManager
{
	private EscherDggRecord dgg;

	private Hashtable dgMap = new Hashtable();

	public EscherDggRecord Dgg => dgg;

	public DrawingManager(EscherDggRecord dgg)
	{
		this.dgg = dgg;
	}

	public EscherDgRecord CreateDgRecord()
	{
		EscherDgRecord escherDgRecord = new EscherDgRecord();
		escherDgRecord.RecordId = -4088;
		short num = FindNewDrawingGroupId();
		escherDgRecord.Options = (short)(num << 4);
		escherDgRecord.NumShapes = 0;
		escherDgRecord.LastMSOSPID = -1;
		dgg.AddCluster(num, 0);
		dgg.DrawingsSaved += 1;
		dgMap[num] = escherDgRecord;
		return escherDgRecord;
	}

	public int AllocateShapeId(short drawingGroupId)
	{
		EscherDgRecord escherDgRecord = (EscherDgRecord)dgMap[drawingGroupId];
		int lastMSOSPID = escherDgRecord.LastMSOSPID;
		int num = 0;
		if (lastMSOSPID % 1024 == 1023)
		{
			num = FindFreeSPIDBlock();
			dgg.AddCluster(drawingGroupId, 1);
		}
		else
		{
			for (int i = 0; i < dgg.FileIdClusters.Length; i++)
			{
				EscherDggRecord.FileIdCluster fileIdCluster = dgg.FileIdClusters[i];
				if (fileIdCluster.DrawingGroupId == drawingGroupId && fileIdCluster.NumShapeIdsUsed != 1024)
				{
					fileIdCluster.IncrementShapeId();
				}
				num = ((escherDgRecord.LastMSOSPID != -1) ? (escherDgRecord.LastMSOSPID + 1) : FindFreeSPIDBlock());
			}
		}
		dgg.NumShapesSaved += 1;
		if (num >= dgg.ShapeIdMax)
		{
			dgg.ShapeIdMax = num + 1;
		}
		escherDgRecord.LastMSOSPID = num;
		escherDgRecord.IncrementShapeCount();
		return num;
	}

	public short FindNewDrawingGroupId()
	{
		short num = 1;
		while (DrawingGroupExists(num))
		{
			num++;
		}
		return num;
	}

	public bool DrawingGroupExists(short dgId)
	{
		for (int i = 0; i < dgg.FileIdClusters.Length; i++)
		{
			if (dgg.FileIdClusters[i].DrawingGroupId == dgId)
			{
				return true;
			}
		}
		return false;
	}

	public int FindFreeSPIDBlock()
	{
		return (dgg.ShapeIdMax / 1024 + 1) * 1024;
	}
}
