using System;
using System.Collections.Generic;

namespace BansheeGz.BGDatabase;

[Serializable]
public class BGJsonRepoModel
{
	[Serializable]
	public class Addon
	{
		public string Type;

		public string Config;
	}

	[Serializable]
	public class Obj
	{
		public string Id;
	}

	[Serializable]
	public class ObjMeta : Obj
	{
		public string Name;

		public string Addon;

		public bool IsSystem;

		public string Type;

		public string Config;

		public string Comment;

		public string ControllerType;

		public bool UserDefinedReadonly;
	}

	[Serializable]
	public class Meta : ObjMeta
	{
		public bool UniqueName;

		public bool Singleton;

		public bool EmptyName;

		public int RowsCount;

		public List<Field> Fields = new List<Field>();

		public List<Key> Keys = new List<Key>();

		public List<Entity> Entities = new List<Entity>();

		public List<Index> Indexes = new List<Index>();
	}

	[Serializable]
	public class Field : ObjMeta
	{
		public string DefaultValue;

		public bool Required;

		public string StringFormatter;

		public string CustomEditor;
	}

	[Serializable]
	public class Entity : Obj
	{
		public List<FieldValue> Values = new List<FieldValue>();
	}

	[Serializable]
	public class FieldValue
	{
		public string Name;

		public string Value;
	}

	[Serializable]
	public class Key : Obj
	{
		public string Name;

		public bool Unique;

		public string Comment;

		public string ControllerType;

		public List<string> FieldIds = new List<string>();
	}

	[Serializable]
	public class Index : Obj
	{
		public string Name;

		public string FieldId;

		public string Comment;

		public string ControllerType;
	}

	[Serializable]
	public class View : Obj
	{
		public string Name;

		public string Addon;

		public bool IsSystem;

		public string Config;

		public string Comment;

		public string ControllerType;

		public BGJsonRepoModel Repo;

		public List<MetaMapping> MetaMappings = new List<MetaMapping>();
	}

	[Serializable]
	public class MetaMapping
	{
		public string MetaId;
	}

	public string ProducedBy = "BGDatabase";

	public string DbVersion = "1.8.9";

	public string DbBuild = "2024.02.09";

	public BGJsonFormatEnum Format;

	public List<Addon> Addons = new List<Addon>();

	public List<Meta> Metas = new List<Meta>();

	public List<View> Views = new List<View>();
}
