using System.Collections.Generic;
using UnityEngine;

namespace BansheeGz.BGDatabase;

public class BGCalcFlowContext
{
	private static readonly BGObjectPool<BGCalcFlowContext> contextPool = new BGObjectPool<BGCalcFlowContext>(() => new BGCalcFlowContext(), OnContextReturn);

	private static readonly BGObjectPool<BGCellPointer> cellsPool = new BGObjectPool<BGCellPointer>(() => new BGCellPointer(), (BGCellPointer pointer) =>
	{
		pointer.Reset();
	});

	private readonly List<BGCellPointer> cells = new List<BGCellPointer>();

	public BGCalcGraph Graph;

	public BGCalcVarsProvider VarsOverrides;

	public BGEntity CurrentEntity;

	public GameObject CurrentGameObject;

	public BGCalcGraphTypeEnum GraphType;

	public BGCalcFlowEvents Events;

	private BGCalcFlowContext()
	{
	}

	public void AddCell(BGField field, BGEntity entity)
	{
		BGCellPointer bGCellPointer = cellsPool.Get();
		bGCellPointer.Reset(field, entity);
		cells.Add(bGCellPointer);
	}

	public bool ContainsCell(BGField field, BGEntity entity)
	{
		BGCellPointer bGCellPointer = cellsPool.Get();
		try
		{
			bGCellPointer.Reset(field, entity);
			return cells.Contains(bGCellPointer);
		}
		finally
		{
			cellsPool.Return(bGCellPointer);
		}
	}

	public void Reset()
	{
		foreach (BGCellPointer cell in cells)
		{
			cellsPool.Return(cell);
		}
		Graph = null;
		CurrentEntity = null;
		VarsOverrides = null;
		GraphType = BGCalcGraphTypeEnum.CalculatedField;
		cells.Clear();
	}

	public void CopyCellsFrom(BGCalcFlowContext context)
	{
		foreach (BGCellPointer cell in context.cells)
		{
			BGCellPointer bGCellPointer = cellsPool.Get();
			bGCellPointer.MetaId = cell.MetaId;
			bGCellPointer.FieldId = cell.FieldId;
			bGCellPointer.EntityId = cell.EntityId;
			cells.Add(bGCellPointer);
		}
	}

	private static void OnContextReturn(BGCalcFlowContext context)
	{
		context.Reset();
	}

	public static BGCalcFlowContext Get()
	{
		return contextPool.Get();
	}

	public static void Return(BGCalcFlowContext context)
	{
		contextPool.Return(context);
	}
}
