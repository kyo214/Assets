using System;
using System.Collections.Generic;

namespace BansheeGz.BGDatabase;

public class BGDBTextBinderRoot
{
	public class BindingException : Exception
	{
		public BindingException(string message)
			: base(message)
		{
		}
	}

	public class DBFieldInfo
	{
		public BGId MetaId;

		public BGId EntityId;

		public BGId FieldId;

		public string SpecialField;
	}

	private readonly List<BGDBTextBinder> children = new List<BGDBTextBinder>();

	private readonly List<DBFieldInfo> fields = new List<DBFieldInfo>();

	private readonly string template;

	public string Error { get; set; }

	public List<DBFieldInfo> Fields => fields;

	public string Template => template;

	public BGDBTextBinderRoot(string template)
	{
		this.template = template;
	}

	public string Bind()
	{
		if (Error != null)
		{
			return Error;
		}
		fields.Clear();
		try
		{
			BGDBTextBinderContext bGDBTextBinderContext = new BGDBTextBinderContext(this);
			foreach (BGDBTextBinder child in children)
			{
				child.Bind(bGDBTextBinderContext);
			}
			return bGDBTextBinderContext.Result;
		}
		catch (BindingException ex)
		{
			Error = ex.Message;
			return Error;
		}
	}

	public void Add(BGDBTextBinder binder)
	{
		if (Error == null)
		{
			children.Add(binder);
		}
	}
}
