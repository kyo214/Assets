namespace BansheeGz.BGDatabase;

public class BGDBTextBinderField : BGDBTextBinder
{
	public class Pointer
	{
		public BGId MetaId = BGId.Empty;

		public string MetaName;

		public BGId EntityId = BGId.Empty;

		public BGId FieldId = BGId.Empty;

		public string FieldName;
	}

	public abstract class BGDBTextBinderFieldSpecial : BGDBTextBinder
	{
		public abstract Pointer Pointer { set; }

		public abstract BGDBTextBinder Create(Pointer pointer);
	}

	private const string ERROR_CANNOT_FIND_META = "Can not find meta with specified id/name";

	private const string ERROR_CANNOT_FIND_ENTITY = "Can not find entity with specified id";

	private const string ERROR_CANNOT_FIND_FIELD = "Can not find field with specified id/name";

	private readonly Pointer pointer;

	public BGDBTextBinderField(Pointer pointer)
	{
		this.pointer = pointer;
	}

	public override void Bind(BGDBTextBinderContext context)
	{
		BGRepo i = BGRepo.I;
		BGEntity entity;
		if (pointer.MetaId.IsEmpty && pointer.MetaName == null)
		{
			entity = i.GetEntity(pointer.EntityId);
		}
		else
		{
			BGMetaEntity bGMetaEntity = (pointer.MetaId.IsEmpty ? i.GetMeta(pointer.MetaName) : i.GetMeta(pointer.MetaId));
			Assert(bGMetaEntity != null, "Can not find meta with specified id/name");
			entity = bGMetaEntity.GetEntity(pointer.EntityId);
		}
		Assert(entity != null, "Can not find entity with specified id");
		BGField bGField = (pointer.FieldId.IsEmpty ? entity.Meta.GetField(pointer.FieldName, errorIfNotFound: false) : entity.Meta.GetField(pointer.FieldId, errorIfNotFound: false));
		Assert(bGField != null, "Can not find field with specified id/name");
		if (bGField.ValueType == typeof(string))
		{
			context.Add(entity.Get<string>(bGField));
		}
		else
		{
			context.Add(bGField.ToString(entity.Index));
		}
		context.Add(bGField, entity);
	}

	private void Assert(bool condition, string message)
	{
		if (condition)
		{
			return;
		}
		throw new BGDBTextBinderRoot.BindingException(message);
	}
}
