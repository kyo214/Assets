using System;
using System.Collections;
using System.IO;
using System.Text;
using NPOI.POIFS.FileSystem;

namespace NPOI.POIFS.Dev;

public class POIFSViewEngine
{
	public static IList InspectViewable(object viewable, bool drilldown, int indentLevel, string indentString)
	{
		IList list = new ArrayList();
		if (viewable is DictionaryEntry dictionaryEntry)
		{
			ProcessViewable(dictionaryEntry.Value, drilldown, indentLevel, indentString, list);
		}
		else if (viewable is POIFSViewable)
		{
			ProcessViewable(viewable, drilldown, indentLevel, indentString, list);
		}
		else
		{
			list.Add(Indent(indentLevel, indentString, viewable.ToString()));
		}
		return list;
	}

	internal static void ProcessViewable(object viewable, bool drilldown, int indentLevel, string indentString, IList objects)
	{
		POIFSViewable pOIFSViewable = (POIFSViewable)viewable;
		objects.Add(Indent(indentLevel, indentString, pOIFSViewable.ShortDescription));
		if (!drilldown)
		{
			return;
		}
		if (pOIFSViewable is OPOIFSDocument)
		{
			((ArrayList)objects).AddRange(InspectViewable("POIFSDocument content is too long so ignored", drilldown, indentLevel + 1, indentString));
		}
		else if (pOIFSViewable.PreferArray)
		{
			Array viewableArray = pOIFSViewable.ViewableArray;
			for (int i = 0; i < viewableArray.Length; i++)
			{
				((ArrayList)objects).AddRange(InspectViewable(viewableArray.GetValue(i), drilldown, indentLevel + 1, indentString));
			}
		}
		else
		{
			IEnumerator viewableIterator = pOIFSViewable.ViewableIterator;
			while (viewableIterator.MoveNext())
			{
				((ArrayList)objects).AddRange(InspectViewable(viewableIterator.Current, drilldown, indentLevel + 1, indentString));
			}
		}
	}

	private static string Indent(int indentLevel, string indentString, string data)
	{
		StringBuilder stringBuilder = new StringBuilder();
		StringBuilder stringBuilder2 = new StringBuilder();
		for (int i = 0; i < indentLevel; i++)
		{
			stringBuilder2.Append(indentString);
		}
		using StringReader stringReader = new StringReader(data);
		for (string text = stringReader.ReadLine(); text != null; text = stringReader.ReadLine())
		{
			stringBuilder.Append((object)stringBuilder2).Append(text).Append(Environment.NewLine);
		}
		return stringBuilder.ToString();
	}
}
