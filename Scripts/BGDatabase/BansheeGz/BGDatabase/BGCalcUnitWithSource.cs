using System;

namespace BansheeGz.BGDatabase;

public abstract class BGCalcUnitWithSource : BGCalcUnit
{
	public static readonly byte SourceVarId = 1;

	public Action OnSourceChange;

	public const string InIndex = "a";

	public const string InId = "b";

	public const string InName = "c";

	public const string InObject = "d";

	public const string OutIndex = "k";

	public const string OutId = "l";

	public const string OutName = "m";

	public const string OutObject = "n";

	private BGCalcValueInput indexInput;

	private BGCalcValueInput idInput;

	private BGCalcValueInput nameInput;

	private BGCalcValueInput objInput;

	public BGCalcValueInput IndexInput => indexInput;

	public BGCalcValueInput IdInput => idInput;

	public BGCalcValueInput NameInput => nameInput;

	public BGCalcValueInput DbObjectInput => objInput;

	public BGCalcUnitSourceEnum Source
	{
		get
		{
			BGCalcVarLite var = GetVar(SourceVarId);
			return (BGCalcUnitSourceEnum)var.Value;
		}
		set
		{
			if (Source != value)
			{
				GetVar(SourceVarId).Value = value;
			}
		}
	}

	protected abstract BGCalcTypeCode ObjectTypeCode { get; }

	public override void Definition()
	{
		BGCalcVarLite orAddVar = GetOrAddVar(SourceVarId, BGCalcTypeCodeRegistry.EntitySource);
		orAddVar.OnValueChange += OnSourceChanged;
		AddRefPort();
		ValueOutput(ObjectTypeCode, ObjectTypeCode.Name, "n", GetObject);
		ValueOutput(BGCalcTypeCodeRegistry.Int, "index", "k", GetIndex);
		ValueOutput(BGCalcTypeCodeRegistry.BGId, "id", "l", GetId);
		ValueOutput(BGCalcTypeCodeRegistry.String, "name", "m", GetName);
	}

	public override string GetPublicVarLabel(byte varId)
	{
		if (varId != SourceVarId)
		{
			return null;
		}
		return "source";
	}

	private BGObject FetchObject(BGCalcFlowI flow)
	{
		return Source switch
		{
			BGCalcUnitSourceEnum.DB_Object => (BGObject)flow.GetValue(objInput), 
			BGCalcUnitSourceEnum.Index => FetchObjectByIndex(flow, flow.GetValue<int>(indexInput)), 
			BGCalcUnitSourceEnum.Id => FetchObjectById(flow, flow.GetValue<BGId>(idInput)), 
			BGCalcUnitSourceEnum.Name => FetchObjectByName(flow, flow.GetValue<string>(nameInput)), 
			_ => throw new ArgumentOutOfRangeException(), 
		};
	}

	private object GetObject(BGCalcFlowI flow)
	{
		return FetchObject(flow);
	}

	private int GetIndex(BGCalcFlowI flow)
	{
		BGObject bGObject = FetchObject(flow);
		if (bGObject == null)
		{
			throw new Exception("Can not get object index cause object is not found!");
		}
		return ((BGIndexableI)bGObject).Index;
	}

	private BGId GetId(BGCalcFlowI flow)
	{
		BGObject bGObject = FetchObject(flow);
		if (bGObject == null)
		{
			throw new Exception("Can not get object Id cause object is not found!");
		}
		return bGObject.Id;
	}

	private string GetName(BGCalcFlowI flow)
	{
		BGObject bGObject = FetchObject(flow);
		if (bGObject == null)
		{
			throw new Exception("Can not get object name cause object is not found!");
		}
		return ((BGObjectWithNameI)bGObject).Name;
	}

	private void OnSourceChanged()
	{
		Remove(indexInput);
		Remove(idInput);
		Remove(nameInput);
		Remove(objInput);
		indexInput = null;
		idInput = null;
		nameInput = null;
		objInput = null;
		AddRefPort();
		OnSourceChange?.Invoke();
	}

	protected void AddRefPort()
	{
		switch (Source)
		{
		case BGCalcUnitSourceEnum.DB_Object:
			objInput = ValueInput(ObjectTypeCode, ObjectTypeCode.Name, "d");
			break;
		case BGCalcUnitSourceEnum.Index:
			indexInput = ValueInput(BGCalcTypeCodeRegistry.Int, "index", "a");
			break;
		case BGCalcUnitSourceEnum.Id:
			idInput = ValueInput(BGCalcTypeCodeRegistry.BGId, "id", "b");
			break;
		case BGCalcUnitSourceEnum.Name:
			nameInput = ValueInput(BGCalcTypeCodeRegistry.String, "name", "c");
			break;
		default:
			throw new ArgumentOutOfRangeException();
		}
	}

	protected void Remove(BGCalcValueInput port)
	{
		if (port != null)
		{
			if (port.IsConnected)
			{
				port.DisconnectAll();
			}
			RemovePort(port);
		}
	}

	protected abstract BGObject FetchObjectByName(BGCalcFlowI flow, string name);

	protected abstract BGObject FetchObjectById(BGCalcFlowI flow, BGId id);

	protected abstract BGObject FetchObjectByIndex(BGCalcFlowI flow, int index);
}
