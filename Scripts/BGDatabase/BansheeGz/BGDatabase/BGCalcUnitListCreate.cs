using System.Collections;

namespace BansheeGz.BGDatabase;

[BGCalcUnitDefinition("By type/list/List create")]
public class BGCalcUnitListCreate : BGCalcUnitWithInPortsCount
{
	public const int Code = 60;

	public override ushort TypeCode => 60;

	protected override BGCalcTypeCode InPortType => BGCalcTypeCodeRegistry.Object;

	protected override BGCalcValueOutput CreateOutputPort()
	{
		return ValueOutput(BGCalcTypeCodeRegistry.List, "list", "r", CreateList);
	}

	private IList CreateList(BGCalcFlowI flow)
	{
		ArrayList arrayList = new ArrayList();
		int count = base.Count;
		for (int i = 0; i < count; i++)
		{
			arrayList.Add(flow.GetValue(inputs[i]));
		}
		return arrayList;
	}
}
