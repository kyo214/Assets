namespace NPOI.HPSF;

public class CustomProperty : MutableProperty
{
	private string name;

	public string Name
	{
		get
		{
			return name;
		}
		set
		{
			name = value;
		}
	}

	public CustomProperty()
	{
		name = null;
	}

	public CustomProperty(Property property)
		: this(property, "")
	{
	}

	public CustomProperty(Property property, string name)
		: base(property)
	{
		this.name = name;
	}

	public bool EqualsContents(object o)
	{
		CustomProperty customProperty = (CustomProperty)o;
		string text = customProperty.Name;
		string text2 = Name;
		bool flag = true;
		flag = text?.Equals(text2) ?? (text2 == null);
		if (flag && customProperty.ID == ID && customProperty.Type == Type)
		{
			return customProperty.Value.Equals(Value);
		}
		return false;
	}

	public override int GetHashCode()
	{
		return (int)ID;
	}
}
