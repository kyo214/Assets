using System;
using UnityEngine.Scripting;

namespace ES3Types;

[Preserve]
[ES3Properties(new string[] { "inext", "inextp", "SeedArray" })]
public class ES3Type_Random : ES3ObjectType
{
	public static ES3Type Instance;

	public ES3Type_Random()
		: base(typeof(Random))
	{
		Instance = this;
	}

	protected override void WriteObject(object obj, ES3Writer writer)
	{
		Random objectContainingField = (Random)obj;
		writer.WritePrivateField("inext", objectContainingField);
		writer.WritePrivateField("inextp", objectContainingField);
		writer.WritePrivateField("SeedArray", objectContainingField);
	}

	protected override void ReadObject<T>(ES3Reader reader, object obj)
	{
		Random objectContainingField = (Random)obj;
		foreach (string property in reader.Properties)
		{
			switch (property)
			{
			case "inext":
				reader.SetPrivateField("inext", reader.Read<int>(), objectContainingField);
				break;
			case "inextp":
				reader.SetPrivateField("inextp", reader.Read<int>(), objectContainingField);
				break;
			case "SeedArray":
				reader.SetPrivateField("SeedArray", reader.Read<int[]>(), objectContainingField);
				break;
			default:
				reader.Skip();
				break;
			}
		}
	}

	protected override object ReadObject<T>(ES3Reader reader)
	{
		Random random = new Random();
		ReadObject<T>(reader, random);
		return random;
	}
}
