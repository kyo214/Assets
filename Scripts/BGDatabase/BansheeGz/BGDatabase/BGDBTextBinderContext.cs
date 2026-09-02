using System.Text;

namespace BansheeGz.BGDatabase;

public class BGDBTextBinderContext
{
	private readonly BGDBTextBinderRoot root;

	private readonly StringBuilder result = new StringBuilder();

	public string Result => result.ToString();

	public BGDBTextBinderRoot Root => root;

	public BGDBTextBinderContext(BGDBTextBinderRoot root)
	{
		this.root = root;
	}

	public void Add(string text)
	{
		result.Append(text);
	}

	public void Add(BGField field, BGEntity entity)
	{
		root.Fields.Add(new BGDBTextBinderRoot.DBFieldInfo
		{
			EntityId = entity.Id,
			FieldId = field.Id,
			MetaId = field.MetaId
		});
	}
}
