using System;

namespace BansheeGz.BGDatabase;

[BGCalcUnitDefinition("By type/enum/Enum literal")]
public class BGCalcUnitEnumLiteral : BGCalcUnit
{
	public static readonly byte TypeVarId = 1;

	public static readonly byte ValueVarId = 2;

	private BGCalcVarLite typeVar;

	private BGCalcVarLite valueVar;

	private BGCalcValueOutputI resultPort;

	public const int Code = 93;

	public override ushort TypeCode => 93;

	public BGCalcValueOutputI ResultPort => resultPort;

	private Type EnumType
	{
		get
		{
			if (typeVar == null)
			{
				return null;
			}
			string text = typeVar.Value as string;
			if (string.IsNullOrEmpty(text))
			{
				return null;
			}
			Type type = BGUtil.GetType(text);
			if (type == null || !type.IsEnum)
			{
				return null;
			}
			return type;
		}
	}

	public event Action OnEnumTypeChange;

	public override void Definition()
	{
		typeVar = GetOrAddVar(TypeVarId, BGCalcTypeCodeRegistry.String);
		CheckOutputPort();
		typeVar.OnValueChange += CheckOutputPort;
	}

	public override string GetPublicVarLabel(byte varId)
	{
		if (TypeVarId == varId)
		{
			return "type";
		}
		if (ValueVarId == varId)
		{
			return "value";
		}
		return null;
	}

	private void CheckOutputPort()
	{
		Type enumType = EnumType;
		if (enumType != null)
		{
			BGCalcTypeCodeEnum bGCalcTypeCodeEnum = new BGCalcTypeCodeEnum(enumType);
			valueVar = GetVar(ValueVarId);
			if (valueVar == null || !object.Equals(valueVar.TypeCode, bGCalcTypeCodeEnum))
			{
				RemoveValueVar();
				valueVar = BGCalcVarLite.Create(this, ValueVarId, bGCalcTypeCodeEnum);
			}
			RemoveResultPort();
			resultPort = ValueOutput(bGCalcTypeCodeEnum, "result", "v", (BGCalcFlowI flow) => (Enum)valueVar.Value);
		}
		else
		{
			RemoveValueVar();
			RemoveResultPort();
		}
		OnEnumTypeChange?.Invoke();
	}

	private void RemoveResultPort()
	{
		if (resultPort != null)
		{
			resultPort.DisconnectAll();
			RemovePort(resultPort);
			resultPort = null;
		}
	}

	private void RemoveValueVar()
	{
		if (valueVar != null)
		{
			GetVars(createIfMissing: true).RemoveVar(valueVar.Id);
			valueVar = null;
		}
	}
}
