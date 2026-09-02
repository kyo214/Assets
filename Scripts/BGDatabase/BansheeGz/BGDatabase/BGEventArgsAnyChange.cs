namespace BansheeGz.BGDatabase;

public class BGEventArgsAnyChange : BGEventArgsA
{
	private static readonly BGObjectPoolNTS<BGEventArgsAnyChange> pool = new BGObjectPoolNTS<BGEventArgsAnyChange>(() => new BGEventArgsAnyChange());

	protected override BGObjectPool Pool => pool;

	public BGRepo Repo { get; private set; }

	private BGEventArgsAnyChange()
	{
	}

	public static BGEventArgsAnyChange GetInstance(BGRepo repo)
	{
		BGEventArgsAnyChange bGEventArgsAnyChange = pool.Get();
		bGEventArgsAnyChange.Repo = repo;
		return bGEventArgsAnyChange;
	}

	public override void Clear()
	{
		Repo = null;
	}

	public override string ToString()
	{
		return "BGEventArgsAnyChange";
	}
}
