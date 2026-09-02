namespace BansheeGz.BGDatabase;

public class BGEventArgsMeta : BGEventArgsA
{
	public enum OperationEnum
	{
		Update = 0,
		Add = 1,
		Delete = 2
	}

	private static readonly BGObjectPoolNTS<BGEventArgsMeta> pool = new BGObjectPoolNTS<BGEventArgsMeta>(() => new BGEventArgsMeta());

	protected override BGObjectPool Pool => pool;

	public BGMetaEntity Meta { get; private set; }

	public BGMetaView View { get; private set; }

	public OperationEnum Operation { get; private set; }

	private BGEventArgsMeta()
	{
	}

	public static BGEventArgsMeta GetInstance(OperationEnum operation, BGMetaEntity meta)
	{
		BGEventArgsMeta bGEventArgsMeta = pool.Get();
		bGEventArgsMeta.Meta = meta;
		bGEventArgsMeta.View = null;
		bGEventArgsMeta.Operation = operation;
		return bGEventArgsMeta;
	}

	public static BGEventArgsMeta GetInstance(OperationEnum operation, BGMetaView view)
	{
		BGEventArgsMeta bGEventArgsMeta = pool.Get();
		bGEventArgsMeta.Meta = null;
		bGEventArgsMeta.View = view;
		bGEventArgsMeta.Operation = operation;
		return bGEventArgsMeta;
	}

	public override void Clear()
	{
		Meta = null;
		View = null;
		Operation = OperationEnum.Update;
	}

	public override string ToString()
	{
		return $"BGEventArgsMeta: meta [{Meta}], view [{View}], operation [{Operation}]";
	}
}
