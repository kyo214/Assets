namespace BansheeGz.BGDatabase;

public abstract class BGDBTextTagProcessor
{
	public abstract string Tag { get; }

	public abstract void Process(BGDBTextProcessorContext context, string parameter);
}
