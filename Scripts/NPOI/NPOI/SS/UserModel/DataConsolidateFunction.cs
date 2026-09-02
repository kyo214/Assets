namespace NPOI.SS.UserModel;

public class DataConsolidateFunction
{
	public static DataConsolidateFunction AVERAGE = new DataConsolidateFunction(0, "Average");

	public static DataConsolidateFunction COUNT = new DataConsolidateFunction(1, "Count");

	public static DataConsolidateFunction COUNT_NUMS = new DataConsolidateFunction(2, "Count");

	public static DataConsolidateFunction MAX = new DataConsolidateFunction(3, "Max");

	public static DataConsolidateFunction MIN = new DataConsolidateFunction(4, "Min");

	public static DataConsolidateFunction PRODUCT = new DataConsolidateFunction(5, "Product");

	public static DataConsolidateFunction STD_DEV = new DataConsolidateFunction(6, "StdDev");

	public static DataConsolidateFunction STD_DEVP = new DataConsolidateFunction(7, "StdDevp");

	public static DataConsolidateFunction SUM = new DataConsolidateFunction(8, "Sum");

	public static DataConsolidateFunction VAR = new DataConsolidateFunction(9, "Var");

	public static DataConsolidateFunction VARP = new DataConsolidateFunction(10, "Varp");

	private int value;

	private string name;

	public string Name => name;

	public int Value => value;

	public DataConsolidateFunction(int value, string name)
	{
		this.value = value;
		this.name = name;
	}
}
