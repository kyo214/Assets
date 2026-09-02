namespace BansheeGz.BGDatabase;

public class BGDBTextProcessorContext
{
	private readonly BGDBTextProcessor processor;

	private readonly string template;

	private readonly BGDBTextBinderRoot root;

	public BGDBTextProcessor Processor => processor;

	public string Template => template;

	public BGDBTextBinderRoot Root => root;

	public BGDBTextProcessorContext(BGDBTextProcessor processor, string template, BGDBTextBinderRoot root)
	{
		this.processor = processor;
		this.template = template;
		this.root = root;
	}
}
