using System;
using System.Collections.Generic;
using UnityEngine;

namespace BansheeGz.BGDatabase;

[Serializable]
public class BGDBTemplate : BGDBA
{
	private static readonly BGDBTextProcessor TextProcessor = new BGDBTextProcessor();

	[SerializeField]
	private string template;

	private BGDBTextBinderRoot binder;

	public string Template
	{
		get
		{
			return template;
		}
		set
		{
			template = value;
		}
	}

	public BGDBTextBinderRoot Binder => binder;

	public override bool SupportReverseBinding => false;

	public override Type TargetType => typeof(string);

	public override object ValueToBind
	{
		get
		{
			error = null;
			EnsureTarget();
			if (error != null)
			{
				return null;
			}
			return GetValue();
		}
	}

	public override object GetValue()
	{
		if (binder == null || !string.Equals(binder.Template, template))
		{
			binder = TextProcessor.Process(template);
		}
		if (binder.Error != null)
		{
			error = binder.Error;
			return null;
		}
		string result = binder.Bind();
		if (binder.Error != null)
		{
			error = binder.Error;
		}
		return result;
	}

	public override int AddFieldsListeners(Action action)
	{
		RemoveFieldsListeners();
		List<BGDBTextBinderRoot.DBFieldInfo> fields = Binder.Fields;
		fields.ForEach((BGDBTextBinderRoot.DBFieldInfo info) =>
		{
			eventHandlers.Add(new FieldEventHandler(info.MetaId, info.FieldId, info.EntityId, action));
		});
		return eventHandlers.Count;
	}
}
