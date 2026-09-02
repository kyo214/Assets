using System;
using System.Collections.Generic;
using UnityEngine;

namespace BansheeGz.BGDatabase;

[AddComponentMenu("BansheeGz/BGDataBinderTemplateGo")]
public class BGDataBinderTemplateGo : BGDataBinderSingleGoA<BGDBTemplate>
{
	[SerializeField]
	[HideInInspector]
	private string template;

	[SerializeField]
	[HideInInspector]
	private bool liveUpdate;

	private bool listenersWasAdded;

	public string Template
	{
		get
		{
			return template;
		}
		set
		{
			if (!string.Equals(template, value))
			{
				template = value;
				InjectToDelegate();
			}
		}
	}

	public bool LiveUpdate
	{
		get
		{
			return liveUpdate;
		}
		set
		{
			liveUpdate = value;
		}
	}

	public BGDBTextBinderRoot Binder => BindDelegate.Binder;

	public override Type TargetType
	{
		get
		{
			return typeof(string);
		}
		set
		{
			throw new Exception("Target Type can not be changed for BGDataBinderTemplateGo component- it's always string type");
		}
	}

	protected bool IsLiveUpdateOn
	{
		get
		{
			if (liveUpdate && (Application.isPlaying || BGUtil.TestIsRunning))
			{
				return BindDelegate.Error == null;
			}
			return false;
		}
	}

	protected override void OnDestroy()
	{
		if (listenersWasAdded)
		{
			BGRepo.OnLoad -= OnLoad;
			BGRepo.I.Events.OnBatchUpdate -= OnBatch;
			BindDelegate.RemoveFieldsListeners();
		}
	}

	protected override void AddListeners()
	{
		if (IsLiveUpdateOn && !listenersWasAdded)
		{
			listenersWasAdded = true;
			BGRepo.OnLoad += OnLoad;
			BGRepo.I.Events.OnBatchUpdate += OnBatch;
			BindDelegate.AddFieldsListeners(Bind);
		}
	}

	protected override void InjectToDelegate()
	{
		base.InjectToDelegate();
		BindDelegate.Template = Template;
	}

	private void OnLoad(bool loaded)
	{
		if (loaded)
		{
			Bind();
		}
	}

	private void OnBatch(object sender, BGEventArgsBatch e)
	{
		bool flag = false;
		List<BGDBTextBinderRoot.DBFieldInfo> fields = BindDelegate.Binder.Fields;
		foreach (BGDBTextBinderRoot.DBFieldInfo item in fields)
		{
			if (e.WasEntitiesUpdated(item.MetaId))
			{
				flag = true;
				break;
			}
		}
		if (flag)
		{
			Bind();
		}
	}
}
