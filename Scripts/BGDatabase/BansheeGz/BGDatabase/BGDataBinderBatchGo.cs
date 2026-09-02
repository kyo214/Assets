using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace BansheeGz.BGDatabase;

[AddComponentMenu("BansheeGz/BGDataBinderBatchGo")]
public class BGDataBinderBatchGo : BGDataBinderGoA
{
	private static readonly StringBuilder error = new StringBuilder();

	[SerializeField]
	[HideInInspector]
	private bool showResults;

	[SerializeField]
	[HideInInspector]
	private List<BGDBField> fieldBinders;

	[SerializeField]
	[HideInInspector]
	private List<BGDBTemplate> templateBinders;

	[SerializeField]
	[HideInInspector]
	private List<BGDBGraph> graphBinders;

	private int listenersCount;

	public List<BGDBField> FieldBinders => fieldBinders;

	public List<BGDBTemplate> TemplateBinders => templateBinders;

	public List<BGDBGraph> GraphBinders => graphBinders;

	public bool ShowResults => showResults;

	public override string Error
	{
		get
		{
			error.Length = 0;
			int num = 0;
			if (fieldBinders != null)
			{
				foreach (BGDBField fieldBinder in fieldBinders)
				{
					string value = fieldBinder.Error;
					if (!string.IsNullOrEmpty(value))
					{
						if (num != 0)
						{
							error.Append(Environment.NewLine);
						}
						error.Append(++num).Append(") ").Append(value);
					}
				}
			}
			if (templateBinders != null)
			{
				foreach (BGDBTemplate templateBinder in templateBinders)
				{
					string value2 = templateBinder.Error;
					if (!string.IsNullOrEmpty(value2))
					{
						if (num != 0)
						{
							error.Append(Environment.NewLine);
						}
						error.Append(++num).Append(") ").Append(value2);
					}
				}
			}
			if (error.Length != 0)
			{
				return error.ToString();
			}
			return null;
		}
	}

	public override void Bind()
	{
		if (!bindedOnce)
		{
			bindedOnce = true;
			FirstBind();
		}
		else
		{
			if (fieldBinders != null)
			{
				foreach (BGDBField fieldBinder in fieldBinders)
				{
					LogError(fieldBinder.Bind());
				}
			}
			if (templateBinders != null)
			{
				foreach (BGDBTemplate templateBinder in templateBinders)
				{
					LogError(templateBinder.Bind());
				}
			}
			if (graphBinders != null)
			{
				foreach (BGDBGraph graphBinder in graphBinders)
				{
					LogError(graphBinder.Bind());
				}
			}
		}
		FireOnBind();
	}

	public override void ReverseBind()
	{
		if (fieldBinders == null)
		{
			return;
		}
		foreach (BGDBField fieldBinder in fieldBinders)
		{
			fieldBinder.ReverseBind();
		}
	}

	protected override void FirstBind()
	{
		bool flag = Application.isPlaying || BGUtil.TestIsRunning;
		listenersCount = 0;
		if (fieldBinders != null)
		{
			foreach (BGDBField binder in fieldBinders)
			{
				string text = binder.Bind();
				LogError(text);
				if ((binder.LiveUpdate && text == null) & flag)
				{
					listenersCount += binder.AddFieldsListeners(() =>
					{
						binder.Bind();
					});
				}
			}
		}
		if (templateBinders != null)
		{
			foreach (BGDBTemplate binder2 in templateBinders)
			{
				string text2 = binder2.Bind();
				LogError(text2);
				if ((binder2.LiveUpdate && text2 == null) & flag)
				{
					listenersCount += binder2.AddFieldsListeners(() =>
					{
						binder2.Bind();
					});
				}
			}
		}
		if (graphBinders != null)
		{
			foreach (BGDBGraph binder3 in graphBinders)
			{
				binder3.SetContext(base.gameObject, listenersCount == 0);
				string text3 = binder3.Bind();
				LogError(text3);
				if ((binder3.LiveUpdate && text3 == null) & flag)
				{
					listenersCount += binder3.AddFieldsListeners(() =>
					{
						binder3.Bind();
					});
				}
			}
		}
		if (listenersCount > 0)
		{
			BGRepo.OnLoad += OnLoad;
			BGRepo.I.Events.OnBatchUpdate += OnBatch;
		}
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
		Bind();
	}

	protected override void OnDestroy()
	{
		if (listenersCount == 0 || (!Application.isPlaying && !BGUtil.TestIsRunning))
		{
			return;
		}
		BGRepo.OnLoad -= OnLoad;
		BGRepo.I.Events.OnBatchUpdate -= OnBatch;
		if (fieldBinders != null)
		{
			foreach (BGDBField fieldBinder in fieldBinders)
			{
				if (fieldBinder.LiveUpdate)
				{
					fieldBinder.RemoveFieldsListeners();
				}
			}
		}
		if (templateBinders != null)
		{
			foreach (BGDBTemplate templateBinder in templateBinders)
			{
				if (templateBinder.LiveUpdate)
				{
					templateBinder.RemoveFieldsListeners();
				}
			}
		}
		if (graphBinders != null)
		{
			foreach (BGDBGraph graphBinder in graphBinders)
			{
				if (graphBinder.LiveUpdate)
				{
					graphBinder.RemoveFieldsListeners();
				}
			}
		}
		listenersCount = 0;
	}

	public List<BGDBField> EnsureFieldBinders()
	{
		return fieldBinders ?? (fieldBinders = new List<BGDBField>());
	}

	public List<BGDBTemplate> EnsureTemplateBinders()
	{
		return templateBinders ?? (templateBinders = new List<BGDBTemplate>());
	}

	public List<BGDBGraph> EnsureGraphBinders()
	{
		return graphBinders ?? (graphBinders = new List<BGDBGraph>());
	}
}
