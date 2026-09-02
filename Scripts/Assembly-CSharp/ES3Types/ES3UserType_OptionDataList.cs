using System.Collections.Generic;
using UnityEngine.Scripting;
using UnityEngine.UI;

namespace ES3Types;

[Preserve]
[ES3Properties(new string[] { "m_Options" })]
public class ES3UserType_OptionDataList : ES3ObjectType
{
	public static ES3Type Instance;

	public ES3UserType_OptionDataList()
		: base(typeof(Dropdown.OptionDataList))
	{
		Instance = this;
		priority = 1;
	}

	protected override void WriteObject(object obj, ES3Writer writer)
	{
		Dropdown.OptionDataList objectContainingField = (Dropdown.OptionDataList)obj;
		writer.WritePrivateField("m_Options", objectContainingField);
	}

	protected override void ReadObject<T>(ES3Reader reader, object obj)
	{
		Dropdown.OptionDataList objectContainingField = (Dropdown.OptionDataList)obj;
		foreach (string property in reader.Properties)
		{
			if (property == "m_Options")
			{
				reader.SetPrivateField("m_Options", reader.Read<List<Dropdown.OptionData>>(), objectContainingField);
			}
			else
			{
				reader.Skip();
			}
		}
	}

	protected override object ReadObject<T>(ES3Reader reader)
	{
		Dropdown.OptionDataList optionDataList = new Dropdown.OptionDataList();
		ReadObject<T>(reader, optionDataList);
		return optionDataList;
	}
}
