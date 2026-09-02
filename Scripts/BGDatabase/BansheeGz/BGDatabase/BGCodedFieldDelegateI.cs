namespace BansheeGz.BGDatabase;

public interface BGCodedFieldDelegateI
{
}
public interface BGCodedFieldDelegateI<T> : BGCodedFieldDelegateI
{
	T Get(BGCodedFieldContext context);
}
