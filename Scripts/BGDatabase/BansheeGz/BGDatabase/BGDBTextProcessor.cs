using System;
using System.Collections.Generic;

namespace BansheeGz.BGDatabase;

public class BGDBTextProcessor
{
	public class ExitException : Exception
	{
	}

	private static readonly Dictionary<string, BGDBTextTagProcessor> Tag2Processor = new Dictionary<string, BGDBTextTagProcessor>();

	private static BGDBTextTagProcessor GetProcessor(string name)
	{
		if (Tag2Processor.Count == 0)
		{
			Register(new BGDBTextTagProcessorField());
		}
		return BGUtil.Get(Tag2Processor, name);
	}

	private static void Register(BGDBTextTagProcessor processor)
	{
		Tag2Processor[processor.Tag] = processor;
	}

	public BGDBTextBinderRoot Process(string template)
	{
		BGDBTextBinderRoot bGDBTextBinderRoot = new BGDBTextBinderRoot(template);
		BGDBTextProcessorContext context = new BGDBTextProcessorContext(this, template, bGDBTextBinderRoot);
		Process(context);
		return bGDBTextBinderRoot;
	}

	private void Process(BGDBTextProcessorContext context)
	{
		try
		{
			string template = context.Template;
			if (template == null)
			{
				return;
			}
			int length = template.Length;
			int num = 0;
			int num2 = 0;
			while (num < length)
			{
				int num3 = template.IndexOf('#', num);
				if (num3 == -1)
				{
					break;
				}
				num = num3 + 1;
				char c = 'a';
				int num4 = num3;
				int num5 = length - 1;
				while (num4 < num5 && char.IsLetter(c))
				{
					c = template[++num4];
				}
				if (num4 > length - 3)
				{
					break;
				}
				if (c != '(')
				{
					continue;
				}
				int num6 = num4 - num3 - 1;
				if (num6 == 0)
				{
					continue;
				}
				string name = template.Substring(num3 + 1, num6);
				BGDBTextTagProcessor processor = GetProcessor(name);
				if (processor == null)
				{
					continue;
				}
				int num7 = template.IndexOf(')', num4);
				if (num7 != -1)
				{
					if (num2 < num3)
					{
						context.Root.Add(new BGDBTextBinderStatic(template.Substring(num2, num3 - num2)));
					}
					num2 = num7 + 1;
					processor.Process(context, template.Substring(num4 + 1, num7 - num4 - 1));
				}
			}
			if (num2 < length)
			{
				context.Root.Add(new BGDBTextBinderStatic(template.Substring(num2, length - num2)));
			}
		}
		catch (ExitException)
		{
		}
	}
}
