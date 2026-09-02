using System;

namespace BansheeGz.BGDatabase;

public abstract class BGCalcUnitDbRowBasedA : BGCalcUnitDbMetaBasedA
{
	public static readonly byte SourceVarId = 2;

	public Action OnSourceChange;

	public const string IndexId = "a";

	public const string IdId = "b";

	public const string NameId = "c";

	public const string EntityId = "d";

	private BGCalcValueInput indexInput;

	private BGCalcValueInput idInput;

	private BGCalcValueInput nameInput;

	private BGCalcValueInput entityInput;

	public BGCalcValueInput IndexInput => indexInput;

	public BGCalcValueInput IdInput => idInput;

	public BGCalcValueInput NameInput => nameInput;

	public BGCalcValueInput EntityInput => entityInput;

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
				OnSourceChange?.Invoke();
			}
		}
	}

	public override string Title
	{
		get
		{
			BGMetaEntity meta = base.Meta;
			if (meta == null)
			{
				return "DB " + Operation + " [ERROR:meta not found]";
			}
			return "DB " + Operation + " [" + meta.Name + "]";
		}
	}

	protected abstract string Operation { get; }

	public override void Definition()
	{
		BGMetaEntity meta = base.Meta;
		if (meta == null)
		{
			throw new Exception("Meta is not found! id=" + base.MetaId.ToString());
		}
		AddRefPort(meta);
		ValueOutput(new BGCalcTypeCodeEntityRuntime(base.Meta), "entity", "e", GetEntity);
		ValueOutput(BGCalcTypeCodeRegistry.Int, "index", "f", (BGCalcFlowI flow) => GetEntity(flow).Index);
		ValueOutput(BGCalcTypeCodeRegistry.BGId, "id", "g", (BGCalcFlowI flow) => GetEntity(flow).Id);
		GetVar(SourceVarId).OnValueChange += OnSourceChanged;
	}

	public override void Init(BGId metaId)
	{
		base.Init(metaId);
		BGCalcVarLite bGCalcVarLite = BGCalcVarLite.Create(this, SourceVarId, BGCalcTypeCodeRegistry.EntitySource);
		bGCalcVarLite.Value = BGCalcUnitSourceEnum.DB_Object;
	}

	public override string GetPublicVarLabel(byte varId)
	{
		if (varId != SourceVarId)
		{
			return null;
		}
		return "source";
	}

	private void OnSourceChanged()
	{
		BGMetaEntity meta = base.Meta;
		if (meta == null)
		{
			throw new Exception("Meta is not found! id=" + base.MetaId.ToString());
		}
		Remove(indexInput);
		Remove(idInput);
		Remove(nameInput);
		Remove(entityInput);
		indexInput = null;
		idInput = null;
		nameInput = null;
		entityInput = null;
		AddRefPort(meta);
		OnSourceChange?.Invoke();
	}

	private void AddRefPort(BGMetaEntity meta)
	{
		switch (Source)
		{
		case BGCalcUnitSourceEnum.DB_Object:
			entityInput = ValueInput(new BGCalcTypeCodeEntityRuntime(meta), "entity", "d");
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

	private void Remove(BGCalcValueInput port)
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

	protected BGEntity GetEntity(BGCalcFlowI flow)
	{
		BGMetaEntity meta = base.Meta;
		if (meta == null)
		{
			throw new Exception("Meta is not found! id=" + base.MetaId.ToString());
		}
		BGEntity bGEntity;
		switch (Source)
		{
		case BGCalcUnitSourceEnum.DB_Object:
			bGEntity = flow.GetValue<BGEntity>(entityInput);
			if (bGEntity == null)
			{
				throw new Exception("Entity is not set!");
			}
			break;
		case BGCalcUnitSourceEnum.Index:
		{
			int value2 = flow.GetValue<int>(indexInput);
			bGEntity = meta.GetEntity(value2);
			if (bGEntity == null)
			{
				throw new Exception($"Can not find an [{meta.Name}] entity using index={value2}!");
			}
			break;
		}
		case BGCalcUnitSourceEnum.Id:
		{
			BGId value3 = flow.GetValue<BGId>(idInput);
			bGEntity = meta.GetEntity(value3);
			if (bGEntity == null)
			{
				throw new Exception($"Can not find an [{meta.Name}] entity using id={value3}!");
			}
			break;
		}
		case BGCalcUnitSourceEnum.Name:
		{
			string value = flow.GetValue<string>(nameInput);
			bGEntity = meta.GetEntity(value);
			if (bGEntity == null)
			{
				throw new Exception("Can not find an [" + meta.Name + "] entity using name=" + value + "!");
			}
			break;
		}
		default:
			throw new ArgumentOutOfRangeException();
		}
		return bGEntity;
	}
}
