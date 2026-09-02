namespace BansheeGz.BGDatabase;

public class BGDBTextBinderStatic : BGDBTextBinder
{
	private readonly string text;

	public BGDBTextBinderStatic(string text)
	{
		this.text = text;
	}

	public override void Bind(BGDBTextBinderContext context)
	{
		context.Add(text);
	}
}
