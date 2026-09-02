using System.Collections;
using System.Text;

namespace BansheeGz.BGDatabase;

[BGCalcUnitDefinition("By type/string/String join")]
public class BGCalcUnitStringJoin : BGCalcUnit
{
	private BGCalcValueInput a;

	private BGCalcValueInput b;

	public const int Code = 69;

	public override ushort TypeCode => 69;

	public override void Definition()
	{
		a = ValueInput(BGCalcTypeCodeRegistry.List, "list", "a");
		b = ValueInput(BGCalcTypeCodeRegistry.String, "separator", "b");
		ValueOutput(BGCalcTypeCodeRegistry.String, "result", "r", GetValue);
	}

	private string GetValue(BGCalcFlowI flow)
	{
		IList value = flow.GetValue<IList>(a);
		string value2 = flow.GetValue<string>(b);
		StringBuilder stringBuilder = new StringBuilder();
		bool flag = !string.IsNullOrEmpty(value2);
		for (int i = 0; i < value.Count; i++)
		{
			if (flag && i != 0)
			{
				stringBuilder.Append(value2);
			}
			object value3 = value[i];
			stringBuilder.Append(value3);
		}
		return stringBuilder.ToString();
	}
}
