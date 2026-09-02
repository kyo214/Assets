using System;
using System.Collections.Generic;
using UnityEngine;

namespace BansheeGz.BGDatabase;

public abstract class BGCalcUnitWithInPortsCount : BGCalcUnit, BGCalcUnitInitializable
{
	public const byte CountVarId = 1;

	private const int MaxCount = 250;

	protected readonly List<BGCalcValueInput> inputs = new List<BGCalcValueInput>();

	private BGCalcValueOutput outValue;

	public int Count
	{
		get
		{
			int value = (byte)GetVar(1).Value;
			return Mathf.Clamp(value, Min, 250);
		}
		set
		{
			int num = Mathf.Clamp(value, Min, 250);
			GetVar(1).Value = (byte)num;
		}
	}

	protected virtual int Min => 0;

	protected abstract BGCalcTypeCode InPortType { get; }

	public event Action OnCountChange;

	public override string GetPublicVarLabel(byte varId)
	{
		if (1 != varId)
		{
			return null;
		}
		return "count";
	}

	public void Init()
	{
		BGCalcVarLite bGCalcVarLite = BGCalcVarLite.Create(this, 1, BGCalcTypeCodeRegistry.Byte);
		bGCalcVarLite.Value = (byte)2;
	}

	public BGCalcValueInput GetInput(int index)
	{
		return inputs[index];
	}

	public override void Definition()
	{
		Rebuild();
	}

	private void Rebuild()
	{
		BGCalcVarLite var = GetVar(1);
		var.OnValueChange -= Rebuild;
		var.OnValueChange += Rebuild;
		int count = Count;
		if (count != inputs.Count)
		{
			if (count > inputs.Count)
			{
				for (int i = inputs.Count; i < count; i++)
				{
					inputs.Add(ValueInput(InPortType, i.ToString() ?? "", i.ToString() ?? ""));
				}
			}
			else
			{
				for (int num = inputs.Count - 1; num >= count; num--)
				{
					BGCalcValueInput bGCalcValueInput = inputs[num];
					bGCalcValueInput.DisconnectAll();
					RemovePort(bGCalcValueInput);
					inputs.RemoveAt(num);
				}
			}
		}
		if (outValue == null)
		{
			outValue = CreateOutputPort();
		}
		OnCountChange?.Invoke();
	}

	protected abstract BGCalcValueOutput CreateOutputPort();
}
