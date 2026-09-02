using System;

namespace BansheeGz.BGDatabase;

[FieldDescriptor(Name = "action", Folder = "Calculated", ManagerType = "BansheeGz.BGDatabase.Editor.BGFieldManagerCalcAction")]
public class BGFieldCalcAction : BGFieldCalcA<BGFieldCalcActionValue>
{
	public const ushort CodeType = 2;

	public override ushort TypeCode => 2;

	public override BGCalcTypeCode ResultCode => BGCalcTypeCodeRegistry.CalcAction;

	public BGFieldCalcAction(BGMetaEntity meta, string name)
		: base(meta, name)
	{
	}

	protected internal BGFieldCalcAction(BGMetaEntity meta, BGId id, string name)
		: base(meta, id, name)
	{
	}

	protected override Func<BGMetaEntity, BGId, string, BGField> CreateFieldFactory()
	{
		return (BGMetaEntity meta, BGId id, string name) => new BGFieldCalcAction(meta, id, name);
	}

	protected override BGFieldCalcActionValue Convert(BGEntity entity, BGFieldCalcValue value)
	{
		BGCalcGraph graphToExecute = null;
		if (value?.Graph != null)
		{
			graphToExecute = value.Graph;
		}
		else if (base.Graph != null)
		{
			graphToExecute = base.Graph;
		}
		if (graphToExecute == null)
		{
			return default;
		}
		return new BGFieldCalcActionValue(() =>
		{
			BGCalcFlowContext bGCalcFlowContext = BGCalcFlowContext.Get();
			try
			{
				bGCalcFlowContext.Graph = graphToExecute;
				bGCalcFlowContext.CurrentEntity = entity;
				bGCalcFlowContext.VarsOverrides = value;
				bGCalcFlowContext.GraphType = BGCalcGraphTypeEnum.Action;
				graphToExecute.Execute(bGCalcFlowContext);
			}
			finally
			{
				BGCalcFlowContext.Return(bGCalcFlowContext);
			}
		});
	}
}
