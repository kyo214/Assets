namespace BansheeGz.BGDatabase;

public class BGDBTextTagProcessorField : BGDBTextTagProcessor
{
	public override string Tag => "FIELD";

	public override void Process(BGDBTextProcessorContext context, string parameter)
	{
		if (!string.IsNullOrEmpty(parameter))
		{
			string[] array = parameter.Split(new char[1] { '@' });
			if (array.Length == 2 || array.Length == 3)
			{
				string text = array[0];
				string text2 = array[1];
				string text3 = null;
				if (array.Length == 3)
				{
					text3 = array[2];
				}
				BGId fieldId = BGId.Empty;
				string text4 = null;
				if (text != null && "$locale".Equals(text))
				{
					text4 = text;
				}
				else
				{
					fieldId = BGId.Parse(text);
					if (fieldId.IsEmpty)
					{
						text4 = text;
						Assert(context, BGMetaObject.CheckName(text4) == null, "#FIELD tag parameter contains invalid Field name:[ " + text4 + "]");
					}
				}
				BGId entityId = BGId.Parse(text2);
				Assert(context, !entityId.IsEmpty, "#FIELD tag parameter contains invalid Entity id:[ " + text2 + "]");
				BGId metaId = BGId.Empty;
				string text5 = null;
				if (text3 != null)
				{
					metaId = BGId.Parse(text);
					if (metaId.IsEmpty)
					{
						text5 = text3;
						Assert(context, BGMetaObject.CheckName(text5) == null, "#FIELD tag parameter contains invalid Meta name:[ " + text5 + "]");
					}
				}
				BGDBTextBinderField.Pointer pointer = new BGDBTextBinderField.Pointer
				{
					FieldId = fieldId,
					FieldName = text4,
					EntityId = entityId,
					MetaId = metaId,
					MetaName = text5
				};
				if (!BGLocalizationUglyHacks.DataBindingBind(text4, context.Root, pointer))
				{
					context.Root.Add(new BGDBTextBinderField(pointer));
				}
				return;
			}
		}
		Assert(context, condition: false, "#FIELD tag parameter is invalid:[" + parameter + "]");
	}

	private void Assert(BGDBTextProcessorContext context, bool condition, string reason)
	{
		if (condition)
		{
			return;
		}
		context.Root.Error = reason;
		throw new BGDBTextProcessor.ExitException();
	}
}
