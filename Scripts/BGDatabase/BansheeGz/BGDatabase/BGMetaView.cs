using System;
using System.Collections.Generic;

namespace BansheeGz.BGDatabase;

public class BGMetaView : BGMetaObject
{
	private readonly BGMetaViewMappings mappings;

	private BGMetaRow delegateMeta;

	public BGRepo Repo { get; private set; }

	public override int Index => Repo.GetViewIndex(base.Id);

	public override string Comment
	{
		set
		{
			string text = base.Comment;
			if (!string.Equals(value, text))
			{
				bool flag = string.IsNullOrEmpty(value);
				bool flag2 = string.IsNullOrEmpty(text);
				if (!(flag & flag2))
				{
					base.Comment = value;
					FireViewChanged();
				}
			}
		}
	}

	public override string ControllerType
	{
		set
		{
			string text = base.ControllerType;
			if (!string.Equals(value, text))
			{
				bool flag = string.IsNullOrEmpty(value);
				bool flag2 = string.IsNullOrEmpty(text);
				if (!(flag & flag2))
				{
					base.ControllerType = (flag ? null : value);
					FireViewChanged();
				}
			}
		}
	}

	public override string Name
	{
		set
		{
			if (!string.Equals(Name, value))
			{
				Repo.ErrorIfMetaNameIsNotUnique(value);
				string oldName = Name;
				base.Name = value;
				Repo.ViewNameWasChanged(oldName, Name);
			}
		}
	}

	public BGMetaViewMappings Mappings => mappings;

	public BGMetaRow DelegateMeta
	{
		get
		{
			return delegateMeta;
		}
		internal set
		{
			if (delegateMeta != value)
			{
				if (delegateMeta != null)
				{
					delegateMeta.Repo.Events.OnAnyChange -= DelegateMetaChanged;
				}
				delegateMeta = value;
				delegateMeta.Repo.Events.On = true;
				delegateMeta.Repo.Events.OnAnyChange -= DelegateMetaChanged;
				delegateMeta.Repo.Events.OnAnyChange += DelegateMetaChanged;
			}
		}
	}

	public List<BGAbstractRelationI> RelationsInbound
	{
		get
		{
			List<BGAbstractRelationI> relationsInbound = new List<BGAbstractRelationI>();
			Repo.ForEachMeta((BGMetaEntity meta) =>
			{
				List<BGField> list = meta.FindFields();
				foreach (BGField item in list)
				{
					if (item is BGAbstractRelationI bGAbstractRelationI)
					{
						if (!(bGAbstractRelationI is BGFieldViewRelationSingle { View: var view }))
						{
							if (bGAbstractRelationI is BGFieldViewRelationMultiple { View: { } view2 } && object.Equals(view2, this))
							{
								relationsInbound.Add(bGAbstractRelationI);
							}
						}
						else if (view != null && object.Equals(view, this))
						{
							relationsInbound.Add(bGAbstractRelationI);
						}
					}
				}
			});
			return relationsInbound;
		}
	}

	public List<BGMetaEntity> Metas
	{
		get
		{
			List<BGMetaEntity> list = new List<BGMetaEntity>();
			BGId[] includedMetas = mappings.IncludedMetas;
			foreach (BGId bGId in includedMetas)
			{
				BGMetaEntity meta = Repo.GetMeta(bGId);
				if (meta != null)
				{
					list.Add(meta);
				}
			}
			return list;
		}
	}

	public BGMetaView(BGRepo repo, string name)
		: this(repo, repo.NewViewId, name)
	{
		DelegateMeta = new BGMetaRow(new BGRepo(), base.Id, name);
		new BGFieldEntityName(delegateMeta, null).System = true;
	}

	private BGMetaView(BGRepo repo, BGId id, string name)
		: base(id, name)
	{
		Repo = repo ?? throw new BGException("Repo can not be null");
		Repo.Register(this);
		mappings = new BGMetaViewMappings(this);
	}

	internal static BGMetaView Create(BGRepo repo, BGId id, string name)
	{
		return new BGMetaView(repo, id, name);
	}

	public void CheckStatus()
	{
		Repo.ForEachMeta((BGMetaEntity meta) =>
		{
			Mappings.CheckStatus(meta);
		});
	}

	public override void Delete()
	{
		if (base.IsDeleted)
		{
			return;
		}
		base.Delete();
		Repo.Events.Batch(() =>
		{
			List<BGAbstractRelationI> relationsInbound = RelationsInbound;
			if (!BGUtil.IsEmpty(relationsInbound))
			{
				foreach (BGAbstractRelationI item in relationsInbound)
				{
					((BGField)item).Delete();
				}
			}
			Unregister();
		});
		Unload();
		Repo = null;
	}

	protected void Unregister()
	{
		Repo?.Unregister(this);
	}

	public override string ConfigToString()
	{
		return null;
	}

	public override void ConfigFromString(string config)
	{
	}

	public override byte[] ConfigToBytes()
	{
		return null;
	}

	public override void ConfigFromBytes(ArraySegment<byte> config)
	{
	}

	internal static BGMetaView FromBinary(BGBinaryReader binder, BGRepo repo)
	{
		int num = binder.ReadInt();
		if ((uint)(num - 1) <= 1u)
		{
			BGId bGId = binder.ReadId();
			string text = binder.ReadString();
			BGMetaView view = Create(repo, bGId, text);
			view.ConfigFromBytes(binder.ReadByteArray());
			view.Comment = binder.ReadString();
			view.Addon = binder.ReadString();
			view.System = binder.ReadBool();
			BGRepo bGRepo = new BGRepo();
			bGRepo.Load(BGUtil.ToArray(binder.ReadByteArray()));
			view.DelegateMeta = (BGMetaRow)bGRepo.GetMeta(bGId);
			binder.ReadArray(() =>
			{
				view.Mappings.Add(binder.ReadId());
			});
			if (num >= 2)
			{
				view.ControllerType = binder.ReadString();
			}
			return view;
		}
		throw new BGException("Can not read view from binary array: unsupported version $", num);
	}

	internal static void ToBinary(BGBinaryWriter builder, BGMetaView view)
	{
		builder.AddInt(2);
		builder.AddId(view.Id);
		builder.AddString(view.Name);
		builder.AddByteArray(view.ConfigToBytes());
		builder.AddString(view.Comment);
		builder.AddString(view.Addon);
		builder.AddBool(view.System);
		BGRepo repo = view.delegateMeta.Repo;
		for (int num = repo.CountViews - 1; num >= 0; num--)
		{
			repo.GetView(num).Delete();
		}
		builder.AddByteArray(repo.Save());
		view.Mappings.Trim();
		builder.AddArray(() =>
		{
			BGId[] includedMetas = view.Mappings.IncludedMetas;
			foreach (BGId value in includedMetas)
			{
				builder.AddId(value);
			}
		}, view.Mappings.MappingsCount);
		builder.AddString(view.ControllerType);
	}

	public static bool IsFieldTypeSupported(Type fieldType)
	{
		if (BGLocalizationUglyHacks.IsLocaleField(fieldType))
		{
			return false;
		}
		if (typeof(BGAbstractRelationI).IsAssignableFrom(fieldType))
		{
			return false;
		}
		return true;
	}

	public BGMetaView CloneTo(BGRepo repo)
	{
		BGMetaView bGMetaView = Create(repo, base.Id, Name);
		bGMetaView.Addon = base.Addon;
		bGMetaView.Comment = Comment;
		bGMetaView.ControllerType = ControllerType;
		BGRepo bGRepo = new BGRepo();
		delegateMeta.Repo.CloneTo(bGRepo, null, null, copyValues: false);
		bGMetaView.DelegateMeta = bGRepo.GetMeta<BGMetaRow>(delegateMeta.Id);
		Mappings.CloneTo(bGMetaView.Mappings);
		return bGMetaView;
	}

	public void FireViewChanged()
	{
		Repo.Events.ViewWasChanged(this);
	}

	private void DelegateMetaChanged(object sender, BGEventArgsAnyChange e)
	{
		Repo.Events.ViewWasChanged(this);
	}

	public void ComplyFields(BGMetaView view2)
	{
		if (view2.Id != base.Id)
		{
			throw new Exception($"View IDs mismatch, {view2.Id}!={base.Id}");
		}
		BGRepo bGRepo = new BGRepo();
		view2.DelegateMeta.Repo.CloneTo(bGRepo, null, null, copyValues: false);
		DelegateMeta = bGRepo.GetMeta<BGMetaRow>(base.Id);
	}

	internal void SwitchTo(BGRepo repo)
	{
		Repo = repo;
		Repo.Register(this);
	}

	public bool DeepEqual(BGMetaView t2)
	{
		if (!string.Equals(Name, t2.Name))
		{
			return false;
		}
		if (!string.Equals(Comment, t2.Comment))
		{
			return false;
		}
		if (!string.Equals(ControllerType, t2.ControllerType))
		{
			return false;
		}
		if (!Mappings.DeepEqual(t2.Mappings))
		{
			return false;
		}
		if (DelegateMeta.CountFields != t2.DelegateMeta.CountFields)
		{
			return false;
		}
		for (int i = 0; i < DelegateMeta.CountFields; i++)
		{
			BGField field = DelegateMeta.GetField(i);
			BGField field2 = t2.DelegateMeta.GetField(i);
			if (field.Name != field2.Name)
			{
				return false;
			}
			if (field.GetType() != field2.GetType())
			{
				return false;
			}
		}
		return true;
	}

	public int CountRelatedEntities(BGId metaId)
	{
		List<BGAbstractRelationI> relationsInbound = RelationsInbound;
		int result = 0;
		foreach (BGAbstractRelationI relation in relationsInbound)
		{
			BGField bGField = (BGField)relation;
			bGField.Meta.ForEachEntity((BGEntity entity) =>
			{
				if (!(relation is BGFieldViewRelationSingle bGFieldViewRelationSingle))
				{
					if (relation is BGFieldViewRelationMultiple bGFieldViewRelationMultiple)
					{
						List<BGRowRef> storedValue = bGFieldViewRelationMultiple.GetStoredValue(entity.Index);
						if (storedValue != null && storedValue.Count != 0 && !(storedValue.Find((BGRowRef rowRef) => (object)rowRef != null && rowRef.MetaId == metaId) == null))
						{
							int num = result;
							result = num + 1;
						}
					}
				}
				else
				{
					BGRowRef storedValue2 = bGFieldViewRelationSingle.GetStoredValue(entity.Index);
					if (!(storedValue2 == null) && !(storedValue2.MetaId != metaId))
					{
						int num = result;
						result = num + 1;
					}
				}
			});
		}
		return result;
	}
}
