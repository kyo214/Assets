using System.Text;

namespace BansheeGz.BGDatabase;

[BGCalcUnitDefinition("By type/string/String join2")]
public class BGCalcUnitStringJoin2 : BGCalcUnitWithInPortsCount
{
	public const int Code = 139;

	public override ushort TypeCode => 139;

	protected override BGCalcTypeCode InPortType => BGCalcTypeCodeRegistry.String;

	protected override BGCalcValueOutput CreateOutputPort()
	{
		return ValueOutput(BGCalcTypeCodeRegistry.String, "result", "r", CreateString);
	}

	private string CreateString(BGCalcFlowI flow)
	{
		StringBuilder stringBuilder = new StringBuilder();
		int count = base.Count;
		for (int i = 0; i < count; i++)
		{
			object value = flow.GetValue(inputs[i]);
			if (value != null)
			{
				stringBuilder.Append(value);
			}
		}
		return stringBuilder.ToString();
	}
}
