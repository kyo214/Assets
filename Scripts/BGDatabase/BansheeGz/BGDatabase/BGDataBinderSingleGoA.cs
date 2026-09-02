using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace BansheeGz.BGDatabase;

public abstract class BGDataBinderSingleGoA<T> : BGDataBinderGoA where T : BGDBA, new()
{
	[SerializeField]
	[HideInInspector]
	private Component targetComponent;

	[SerializeField]
	[HideInInspector]
	private string targetFieldName;

	[SerializeField]
	[HideInInspector]
	private bool isTargetProperty;

	[SerializeField]
	[HideInInspector]
	private List<PathItem> path = new List<PathItem>();

	[SerializeField]
	[HideInInspector]
	private bool includePrivate;

	[NonSerialized]
	protected readonly T BindDelegate = new T();

	public override string Error
	{
		get
		{
			InjectToDelegate();
			return BindDelegate.Error;
		}
	}

	public List<PathItem> Path
	{
		get
		{
			return path;
		}
		set
		{
			path = value;
		}
	}

	public string TargetAsString
	{
		get
		{
			InjectToDelegate();
			return BindDelegate.TargetAsString;
		}
	}

	public MemberInfo TargetAsMember
	{
		get
		{
			InjectToDelegate();
			return BindDelegate.TargetAsMember;
		}
	}

	public Component TargetComponent
	{
		get
		{
			return targetComponent;
		}
		set
		{
			targetComponent = value;
			InjectToDelegate();
		}
	}

	public string TargetFieldName
	{
		get
		{
			return targetFieldName;
		}
		set
		{
			targetFieldName = value;
			InjectToDelegate();
		}
	}

	public bool IsTargetProperty
	{
		get
		{
			return isTargetProperty;
		}
		set
		{
			isTargetProperty = value;
			InjectToDelegate();
		}
	}

	public bool IncludePrivate
	{
		get
		{
			return includePrivate;
		}
		set
		{
			includePrivate = value;
		}
	}

	public abstract Type TargetType { get; set; }

	public object ValueToBind => BindDelegate.ValueToBind;

	public override bool SupportReverseBinding => BindDelegate.SupportReverseBinding;

	protected override void FirstBind()
	{
		Bind();
		AddListeners();
	}

	protected abstract void AddListeners();

	public override void Bind()
	{
		if (!bindedOnce)
		{
			bindedOnce = true;
			FirstBind();
		}
		else
		{
			InjectToDelegate();
			LogError(BindDelegate.Bind());
		}
		FireOnBind();
	}

	public override void ReverseBind()
	{
		if (BindDelegate.SupportReverseBinding)
		{
			InjectToDelegate();
			BindDelegate.ReverseBind();
		}
	}

	protected virtual void InjectToDelegate()
	{
		BindDelegate.TargetComponent = TargetComponent;
		BindDelegate.TargetFieldName = TargetFieldName;
		BindDelegate.IsTargetProperty = IsTargetProperty;
		BindDelegate.Path = Path;
	}
}
