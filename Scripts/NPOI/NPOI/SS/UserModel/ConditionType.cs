using System.Collections.Generic;

namespace NPOI.SS.UserModel;

public class ConditionType
{
	private static Dictionary<int, ConditionType> lookup = new Dictionary<int, ConditionType>();

	public static ConditionType CellValueIs = new ConditionType(1, "cellIs");

	public static ConditionType Formula = new ConditionType(2, "expression");

	public static ConditionType ColorScale = new ConditionType(3, "colorScale");

	public static ConditionType DataBar = new ConditionType(4, "dataBar");

	public static ConditionType Filter = new ConditionType(5, null);

	public static ConditionType IconSet = new ConditionType(6, "iconSet");

	public byte Id { get; set; }

	public string Type { get; set; }

	public override string ToString()
	{
		return Id + " - " + Type;
	}

	public override int GetHashCode()
	{
		return Id.GetHashCode() ^ Type.GetHashCode();
	}

	public override bool Equals(object obj)
	{
		if (obj == null || !(obj is ConditionType))
		{
			return false;
		}
		ConditionType conditionType = obj as ConditionType;
		if (Id == conditionType.Id)
		{
			return Type == conditionType.Type;
		}
		return false;
	}

	public static ConditionType ForId(byte id)
	{
		return ForId((int)id);
	}

	public static ConditionType ForId(int id)
	{
		return lookup[id];
	}

	private ConditionType(int id, string type)
	{
		Id = (byte)id;
		Type = type;
		lookup.Add(id, this);
	}
}
