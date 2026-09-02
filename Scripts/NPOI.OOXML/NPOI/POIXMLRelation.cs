using System;
using System.Text.RegularExpressions;

namespace NPOI;

public abstract class POIXMLRelation
{
	protected string _type;

	protected string _relation;

	protected string _defaultName;

	private Type _cls;

	public string ContentType => _type;

	public string Relation => _relation;

	public string DefaultFileName => _defaultName;

	public Type RelationClass => _cls;

	public POIXMLRelation(string type, string rel, string defaultName, Type cls)
	{
		_type = type;
		_relation = rel;
		_defaultName = defaultName;
		_cls = cls;
	}

	public POIXMLRelation(string type, string rel, string defaultName)
		: this(type, rel, defaultName, null)
	{
	}

	public string GetFileName(int index)
	{
		if (_defaultName.IndexOf("#") == -1)
		{
			return DefaultFileName;
		}
		return _defaultName.Replace("#", index.ToString());
	}

	public int GetFileNameIndex(POIXMLDocumentPart part)
	{
		return int.Parse(new Regex(_defaultName.Replace("#", "(\\d+)")).Match(part.GetPackagePart().PartName.Name).Value);
	}
}
